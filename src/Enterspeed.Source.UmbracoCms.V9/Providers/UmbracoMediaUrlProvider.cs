using System;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;

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
                return new Uri(Combine(mediaDomainUrl.ToString(), relativeUrl)).ToString();
            }

            return relativeUrl;
        }
        
        /// <summary>
        /// Combines the url base and the relative url into one, consolidating the '/' between them.
        /// </summary>
        /// <param name="baseUrl">Base url that will be combined.</param>
        /// <param name="relativeUrl">The relative path to combine.</param>
        /// <returns>The merged url.</returns>
        internal string Combine(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
            {
                return baseUrl;
            }

            baseUrl = baseUrl.TrimEnd('/');
            relativeUrl = relativeUrl.TrimStart('/');
            return $"{baseUrl}/{relativeUrl}";
        }
    }
}
