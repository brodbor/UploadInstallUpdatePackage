
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace TriggerGoal.Controllers
{
  
    public class ApiController : Sitecore.Services.Infrastructure.Web.Http.ServicesApiController
    {
   [HttpGet]
        public string Index(string id)
        {
            try
            {
                if (Sitecore.Analytics.Tracker.IsActive)
                    Sitecore.Analytics.Tracker.Current.CurrentPage.Cancel();


                var profile = Sitecore.Analytics.Tracker.Current.Interaction.Profiles["Body Style"];
                var scores = new Dictionary<string, float>();
                scores.Add(id, 5);
                profile.Score(scores);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}