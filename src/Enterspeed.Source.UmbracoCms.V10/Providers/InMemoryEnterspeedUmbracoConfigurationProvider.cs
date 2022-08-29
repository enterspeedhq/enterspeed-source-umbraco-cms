using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Configuration;

namespace Enterspeed.Source.UmbracoCms.V10.Providers
{
    public class InMemoryEnterspeedUmbracoConfigurationProvider : IEnterspeedConfigurationProvider
    {
        public InMemoryEnterspeedUmbracoConfigurationProvider(EnterspeedConfiguration configuration)
        {
            Configuration = configuration;
        }

        public EnterspeedConfiguration Configuration { get; }
    }
}