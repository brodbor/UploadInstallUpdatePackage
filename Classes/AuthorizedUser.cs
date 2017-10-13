
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sitecore;
using Sitecore.StringExtensions;

namespace UploadInstallUpdatePackage.models
{
    public class AuthorizedUser : AuthorizationFilterAttribute
    {


        private string _localName;
        public string LocalName {
            get {return _localName; }
            set
            {
                string localNameFromConfig = Sitecore.Configuration.Settings.GetSetting(value);
                _localName = localNameFromConfig;
            }            
        }


        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Context.User.IsAuthenticated &&  (!LocalName.IsNullOrEmpty() && !Context.User.LocalName.Equals(LocalName)))
                HandleUnauthorized(actionContext, "User does not have access");

            if (!Context.User.IsAuthenticated && (!LocalName.IsNullOrEmpty() && !Context.User.LocalName.Equals(LocalName)))
                HandleUnauthorized(actionContext, "Not Authorized");

            return;
        }

        private void HandleUnauthorized(HttpActionContext actionContext, string error)
        {
            actionContext.Response = actionContext.Request.CreateResponse();
            actionContext.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            actionContext.Response.Content = new StringContent(error);
        }
    }
}