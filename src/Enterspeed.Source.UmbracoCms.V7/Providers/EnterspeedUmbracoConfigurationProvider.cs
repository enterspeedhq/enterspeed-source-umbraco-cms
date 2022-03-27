using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Extensions;

namespace Enterspeed.Source.UmbracoCms.V7.Providers
{
    public class EnterspeedUmbracoConfigurationProvider : IEnterspeedConfigurationProvider
    {
        public EnterspeedUmbracoConfigurationProvider(bool preview)
        {
            var enterspeedConfigurationService = EnterspeedContext.Current.Services.ConfigurationService;
            var configuration = enterspeedConfigurationService.GetConfiguration();
            Configuration = preview ? configuration.GetPreviewConfiguration() : configuration.GetPublishConfiguration();
        }

        public EnterspeedConfiguration Configuration { get; }
    }
}