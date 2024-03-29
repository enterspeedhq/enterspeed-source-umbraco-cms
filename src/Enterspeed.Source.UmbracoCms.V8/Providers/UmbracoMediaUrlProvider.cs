﻿using System;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Providers
{
    public class UmbracoMediaUrlProvider : IUmbracoMediaUrlProvider
    {
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public UmbracoMediaUrlProvider(
            IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public string GetUrl(IPublishedContent media)
        {
            var relativeUrl = media.Url(null, UrlMode.Relative);

            var enterspeedMediaDomain = _enterspeedConfigurationService.GetConfiguration().MediaDomain;
            if (!string.IsNullOrWhiteSpace(enterspeedMediaDomain))
            {
                var mediaDomainUrl = new Uri(enterspeedMediaDomain);
                return mediaDomainUrl.AppendPath(relativeUrl).ToString();
            }

            return relativeUrl;
        }
    }
}