using System.Web.Http;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Umbraco.Web.WebApi;

namespace Enterspeed.Source.UmbracoCms.V7.Controllers.Api
{
    public class EnterspeedTasksApiController : UmbracoApiController
    {
        [HttpGet]
        public void HandleJobs()
        {
            // Do not run if Enterspeed is not configured in Umbraco
            if (EnterspeedContext.Current?.Configuration == null
                || !EnterspeedContext.Current.Configuration.IsConfigured)
            {
                return;
            }

            EnterspeedContext.Current.Handlers.JobHandler.HandlePendingJobs(50);
        }

        [HttpGet]
        public void InvalidateOldProcessingJobs()
        {
            // Do not run if Enterspeed is not configured in Umbraco
            if (EnterspeedContext.Current?.Configuration == null
                || !EnterspeedContext.Current.Configuration.IsConfigured)
            {
                return;
            }

            EnterspeedContext.Current.Handlers.JobHandler.InvalidateOldProcessingJobs();
        }
    }
}
