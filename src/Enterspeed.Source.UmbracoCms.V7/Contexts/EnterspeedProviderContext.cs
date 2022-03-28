using System;
using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.UmbracoCms.V7.Providers;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedProviderContext
    {
        private readonly Lazy<IEnterspeedConfigurationProvider> _publishConfigurationProvider;
        private readonly Lazy<IEnterspeedConfigurationProvider> _previewConfigurationProvider;

        public EnterspeedProviderContext()
        {
            _publishConfigurationProvider = new Lazy<IEnterspeedConfigurationProvider>(() => new EnterspeedUmbracoConfigurationProvider(false));
            _previewConfigurationProvider = new Lazy<IEnterspeedConfigurationProvider>(() =>
            {
                var enterspeedUmbracoConfigurationProvider = new EnterspeedUmbracoConfigurationProvider(true);

                return enterspeedUmbracoConfigurationProvider.Configuration == null ? null : enterspeedUmbracoConfigurationProvider;
            });
        }

        public IEnterspeedConfigurationProvider PublishConfigurationProvider => _publishConfigurationProvider.Value;
        public IEnterspeedConfigurationProvider PreviewConfigurationProvider => _previewConfigurationProvider.Value;
        public UmbracoMediaUrlProvider MediaUrlProvider => new UmbracoMediaUrlProvider();
    }
}
