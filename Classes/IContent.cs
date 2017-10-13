using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Update.Installer;

namespace UploadInstallUpdatePackage.models
{
    interface IContent
    {
        List<InstalledItems> installSitecoreUpdatePackage(string path);
    }
}
