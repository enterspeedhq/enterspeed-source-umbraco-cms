using System;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Providers
{
    public class UmbracoMediaUrlProvider : IUmbracoMediaUrlProvider
    {
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly ILogger<UmbracoMediaUrlProvider> _logger;

        public UmbracoMediaUrlProvider(
            IEnterspeedConfigurationService enterspeedConfigurationService,
            ILogger<UmbracoMediaUrlProvider> logger)
        {
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _logger = logger;
        }

        public string GetUrl(IPublishedContent media)
        {
            var relativeUrl = media.GetUrl(_logger, null, UrlMode.Relative);

            var enterspeedMediaDomain = _enterspeedConfigurationService.GetConfiguration().MediaDomain;
            if (!string.IsNullOrWhiteSpace(enterspeedMediaDomain))
            {
                var mediaDomainUrl = new Uri(enterspeedMediaDomain);
                return new Uri(mediaDomainUrl, relativeUrl).ToString();
            }

            return relativeUrl;
        }
    }
}
