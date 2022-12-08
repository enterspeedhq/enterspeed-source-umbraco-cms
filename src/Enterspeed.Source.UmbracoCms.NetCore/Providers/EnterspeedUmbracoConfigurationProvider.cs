using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.UmbracoCms.NetCore.Services;

namespace Enterspeed.Source.UmbracoCms.NetCore.Providers
{
    public class EnterspeedUmbracoConfigurationProvider : IEnterspeedConfigurationProvider
    {
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public EnterspeedUmbracoConfigurationProvider(IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public EnterspeedConfiguration Configuration => _enterspeedConfigurationService.GetConfiguration();
    }
}