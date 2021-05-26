using System;
using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.UmbracoCms.V7.Providers;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedProviderContext
    {
        private readonly Lazy<IEnterspeedConfigurationProvider> _configurationProvider;

        public EnterspeedProviderContext()
        {
            _configurationProvider = new Lazy<IEnterspeedConfigurationProvider>(() => new EnterspeedUmbracoConfigurationProvider());
        }

        public IEnterspeedConfigurationProvider ConfigurationProvider => _configurationProvider.Value;
        public UmbracoMediaUrlProvider MediaUrlProvider => new UmbracoMediaUrlProvider();
    }
}
