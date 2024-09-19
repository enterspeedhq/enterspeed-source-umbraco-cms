using System;
using System.Linq;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public class UmbracoRedirectsService : IUmbracoRedirectsService
    {
        private readonly IRedirectUrlService _redirectUrlService;
        private readonly IUmbracoUrlService _umbracoUrlService;

        public UmbracoRedirectsService(IRedirectUrlService redirectUrlService, IUmbracoUrlService umbracoUrlService)
        {
            _redirectUrlService = redirectUrlService;
            _umbracoUrlService = umbracoUrlService;
        }

        public virtual string[] GetRedirects(Guid contentKey, string culture)
        {
            var redirects = _redirectUrlService.GetContentRedirectUrls(contentKey)
                ?.Where(redirect => string.IsNullOrWhiteSpace(redirect.Culture) ||
                                    redirect.Culture.ToLowerInvariant().Equals(culture.ToLowerInvariant()));

            if (redirects == null)
            {
                return new string[0];
            }

            return redirects
                .Select(redirect => _umbracoUrlService.GetUrlFromIdUrl(redirect.Url, redirect.Culture))
                .ToArray();
        }
    }
}