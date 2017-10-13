using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;
using System.Web.Http.Cors;
using UploadInstallUpdatePackage.models;

//http://demo/sitecore/api/ssc/UploadInstallUpdatePackage-Controllers/UpdatePackage/1/AddFile

namespace UploadInstallUpdatePackage.Controllers
{
    [Sitecore.Services.Core.ServicesController]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UpdatePackageController : Sitecore.Services.Infrastructure.Web.Http.ServicesApiController
    {
        private Stopwatch stopWatch = new Stopwatch();
        //private readonly ILogger _logger;
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("service/package/test")]
        public string Test()
        {
            return "Ok";
        }

        [AuthorizedUser(LocalName= "APIUser")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("service/package/process")]
        public  Task<List<ServiceResponse>> AddFile()
        {
            UpdateSitecoreContent up = new UpdateSitecoreContent();
            try
            {
                stopWatch.Reset();
                stopWatch.Start();

                HttpRequestMessage request = this.Request;

                //check Mime multipart type
                if (!request.Content.IsMimeMultipartContent())
                {
                    throw new ApplicationException("Must be mime multipart content");
                }

                //upload package files to TEMP folder
                string root = HttpContext.Current.Server.MapPath("~/temp");
            

                var provider = new CustomMultipartFormDataStreamProvider(root);

               // Request.Content.LoadIntoBufferAsync().Wait();


                //execute upload and install of package
                var task = request.Content.ReadAsMultipartAsync(provider).
                    ContinueWith(o =>
                        {
                            if (o.IsFaulted || o.IsCanceled)
                                throw new ApplicationException("Faulted or Canceled");

                            if (provider.FileData.Count < 1)
                                throw new ApplicationException("Missing File");
                   
                            List<ServiceResponse> _sr = new List<ServiceResponse>();
                            foreach (var res in provider.FileData)
                            {
                                FileInfo finfo = new FileInfo(res.LocalFileName);

                                //install sitecore items
                                var installResponse = up.installSitecoreUpdatePackage(res.LocalFileName.Replace("\"", ""));

                                ServiceResponse dt =  new ServiceResponse
                                {
                                    Owner = Sitecore.Context.User.LocalName,
                                    FileName = res.LocalFileName,
                                    ExecutionTime = stopWatch.Elapsed.ToString(),
                                    InstalledItems = installResponse

                                };
                                _sr.Add(dt);
                            }

                            stopWatch.Stop();
                            return _sr;
                        }
                    );
                return task;
            }
            catch (Exception ex)
            {

                UpdateSitecoreContent.log().Error(ex.Message);
                throw new ApplicationException(ex.Message);

            
            }
        }

     



    }
}
