using Enterspeed.Source.UmbracoCms.Models.Configuration;
using Umbraco.Cms.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.Models.Api
{
    public class EnterspeedUmbracoConfigurationResponse : EnterspeedUmbracoConfiguration
    {
        public ServerRole ServerRole { get; }
        public EnterspeedUmbracoConfiguration Configuration { get; }

        public EnterspeedUmbracoConfigurationResponse(EnterspeedUmbracoConfiguration enterspeedUmbracoConfiguration, ServerRole serverRole)
        {
            Configuration = enterspeedUmbracoConfiguration;
            ServerRole = serverRole;
        }
    }
}