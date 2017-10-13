using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace UploadInstallUpdatePackage.models
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {

        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            string fname = secondsSinceEpoch + "_" + headers.ContentDisposition.FileName.Replace("\"", "");


            return fname;
        }

    }
}