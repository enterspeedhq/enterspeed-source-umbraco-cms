using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Umbraco.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.V8.Models.Api
{
    public class EnterspeedUmbracoConfigurationResponse : EnterspeedUmbracoConfiguration
    {
        public string ServerRole { get; }
        public EnterspeedUmbracoConfiguration Configuration { get; }

        public EnterspeedUmbracoConfigurationResponse(EnterspeedUmbracoConfiguration enterspeedUmbracoConfiguration, ServerRole serverRole)
        {
            Configuration = enterspeedUmbracoConfiguration;
            ServerRole = serverRole.ToString();
        }
    }
}