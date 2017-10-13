using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadInstallUpdatePackage.models
{

    public class ServiceResponse
    {
        public string FileName { get; set; }
        public string Owner { get; set; }
        public string ExecutionTime { get; set; }
        public List<InstalledItems> InstalledItems { get; set; }
    }
    public class InstalledItems
    {
        public string Action { get; set; }
        public string ItemPath { get; set; }
        public string ItemGUID { get; set; }
        public string MessageType { get; set; }
    }
}