using Enterspeed.Source.UmbracoCms.Core.Models.Configuration;
using Umbraco.Cms.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.Core.Models.Api
{
    public class EnterspeedUmbracoConfigurationResponse : EnterspeedUmbracoConfiguration
    {
        public ServerRole ServerRole { get; }
        public bool RunJobsOnServer { get; }
        public EnterspeedUmbracoConfiguration Configuration { get; }

        public EnterspeedUmbracoConfigurationResponse(EnterspeedUmbracoConfiguration enterspeedUmbracoConfiguration, ServerRole serverRole, bool runJobsOnServer)
        {
            Configuration = enterspeedUmbracoConfiguration;
            ServerRole = serverRole;
            RunJobsOnServer = runJobsOnServer;
        }
    }
}