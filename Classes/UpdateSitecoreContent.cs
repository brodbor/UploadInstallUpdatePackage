using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using log4net;
using log4net.Config;
using Sitecore.SecurityModel;
using Sitecore.Update;
using Sitecore.Update.Installer;
using Sitecore.Update.Installer.Utils;
using Sitecore.Update.Utils;
using Sitecore.Diagnostics;
using Sitecore.Update.Installer.Exceptions;
using Sitecore.Update.Installer.Messages;


namespace UploadInstallUpdatePackage.models
{
    public class UpdateSitecoreContent : IContent
    {

        public List<InstalledItems> installSitecoreUpdatePackage(string path)
        {
            checkFile(path);

            using (new SecurityDisabler())
            {
                var installer = new DiffInstaller(UpgradeAction.Upgrade);
                var view = UpdateHelper.LoadMetadata(path);

                //Get the package entries
                bool hasPostAction;
                string historyPath;
                try
                {
                    var entries = installer.InstallPackage(path, InstallMode.Install, log(), out hasPostAction,
                        out historyPath);

                    installer.ExecutePostInstallationInstructions(path, historyPath, InstallMode.Install, view, log(),
                        ref entries);


                    var installedItems = new List<InstalledItems>();
                    installedItems =
                        entries.Select(
                            x =>
                                new InstalledItems
                                {
                                    Action = x.Action,
                                    ItemPath = _split(x.LongDescription, "Item path"),
                                    ItemGUID = _split(x.LongDescription, "Item ID"),
                                    MessageType = x.MessageGroupDescription
                                }).ToList();


                    return installedItems;
                }
                catch (PostStepInstallerException exception)
                {
                    foreach (var entry in exception.Entries)
                        log().Info(entry.Message);
                    throw exception;
                }
            }
        }

        private static string _split(string input, string pos)
        {
            var testString = string.Format(@"{0}: \'\[s\](.*?)\[\/s\]\'", pos);

            var match = Regex.Match(input, testString, RegexOptions.Singleline);

            if (match.Success)
            {
                // Finally, we get the Group value and display it.
                var key = match.Groups[1].Value;
                return key;
            }
            return "Failed to parse Path GUID from Sitore response";
        }

        private void checkFile(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists)
            {
                log().Error(string.Format("Cannot access path '{0}'.", path));
                throw new ApplicationException(string.Format("Cannot access path '{0}'.", path));
            }
        }

        public static ILog log()
        {
            return LoggerFactory.GetLogger("InstallerLogFileAppender");
        }
    }
}