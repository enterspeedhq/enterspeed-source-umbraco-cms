using System;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Providers
{
    public class UmbracoMediaUrlProvider
    {
        private readonly EnterspeedConfigurationService _enterspeedConfigurationService;

        public UmbracoMediaUrlProvider()
        {
            _enterspeedConfigurationService = EnterspeedContext.Current.Services.ConfigurationService;
        }

        public string GetUrl(IPublishedContent media)
        {
            return GetUrl(media.Url());
        }

        public string GetUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return url;
            }

            if (!url.StartsWith("/media/"))
            {
                return url;
            }

            var enterspeedMediaDomain = _enterspeedConfigurationService.GetConfiguration().MediaDomain;
            if (!string.IsNullOrWhiteSpace(enterspeedMediaDomain))
            {
                var mediaDomainUrl = new Uri(enterspeedMediaDomain);
                return new Uri(Combine(mediaDomainUrl.ToString(), url)).ToString();
            }

            return url;
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