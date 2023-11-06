using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Umbraco.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.V8.Models.Api
{
    public class EnterspeedUmbracoConfigurationResponse : EnterspeedUmbracoConfiguration
    {
        public string ServerRole { get; }
        public EnterspeedUmbracoConfiguration Configuration { get; }
        public bool RunJobsOnServer { get; }

        public EnterspeedUmbracoConfigurationResponse(EnterspeedUmbracoConfiguration enterspeedUmbracoConfiguration, ServerRole serverRole, bool runJobsOnServer)
        {
            Configuration = enterspeedUmbracoConfiguration;
            RunJobsOnServer = runJobsOnServer;
            ServerRole = serverRole.ToString();
        }
    }
}