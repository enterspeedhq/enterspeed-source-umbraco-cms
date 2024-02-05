using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Providers
{
    public class UmbracoCultureProvider : IUmbracoCultureProvider
    {
        private readonly IUmbracoContextProvider _umbracoContextProvider;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public UmbracoCultureProvider(IUmbracoContextProvider umbracoContextProvider,
            IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _umbracoContextProvider = umbracoContextProvider;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public virtual IEnumerable<string> GetCulturesForCultureVariant(IContent content)
        {
            if (!content.ContentType.VariesByCulture())
            {
                throw new Exception($"Node with id '{content.Id}' does not vary by culture.");
            }

            return content.AvailableCultures;
        }

        public virtual IEnumerable<string> GetCulturesForCultureVariant(IPublishedContent content)
        {
            if (!content.ContentType.VariesByCulture())
            {
                throw new Exception($"Node with id '{content.Id}' does not vary by culture.");
            }

            return content.Cultures.Keys;
        }

        public virtual string GetCultureForNonCultureVariant(IContent content)
        {
            if (content.ContentType.VariesByCulture())
            {
                throw new Exception($"Node with id '{content.Id}' varies by culture.");
            }

            var umbracoContext = _umbracoContextProvider.GetContext();
            var culture = GetDefaultCulture(umbracoContext);

            if (_enterspeedConfigurationService.IsWildcardDomainEnabled())
            {
                var wildcardCulture = GetWildcardCulture(content);
                if (!string.IsNullOrWhiteSpace(wildcardCulture))
                {
                    culture = wildcardCulture;
                }
            }

            return culture;
        }

        public virtual string GetCultureForNonCultureVariant(IPublishedContent content)
        {
            if (content.ContentType.VariesByCulture())
            {
                throw new Exception($"Node with id '{content.Id}' varies by culture.");
            }

            var umbracoContext = _umbracoContextProvider.GetContext();
            var culture = GetDefaultCulture(umbracoContext);


            if (_enterspeedConfigurationService.IsWildcardDomainEnabled())
            {
                var wildcardCulture = GetWildcardCulture(content);
                if (!string.IsNullOrWhiteSpace(wildcardCulture))
                {
                    culture = wildcardCulture;
                }
            }

            return culture;
        }

        private string GetDefaultCulture(IUmbracoContext umbracoContext)
        {
            return umbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }

        public virtual string GetWildcardCulture(IPublishedContent content)
        {
            var umbracoContext = _umbracoContextProvider.GetContext();

            var wildCardDomain = DomainUtilities.FindWildcardDomainInPath(
                umbracoContext.PublishedSnapshot.Domains?.GetAll(true), content.Path, content.Root().Id);

            if (wildCardDomain != null && !string.IsNullOrWhiteSpace(wildCardDomain.Culture))
            {
                return wildCardDomain.Culture.ToLowerInvariant();
            }

            return string.Empty;
        }

        public virtual string GetWildcardCulture(IContent content)
        {
            var umbracoContext = _umbracoContextProvider.GetContext();

            var wildCardDomain = DomainUtilities.FindWildcardDomainInPath(
                umbracoContext.PublishedSnapshot.Domains?.GetAll(true), content.Path, umbracoContext.Content?.GetById(content.Id)?.Root().Id);

            if (wildCardDomain != null && !string.IsNullOrWhiteSpace(wildCardDomain.Culture))
            {
                return wildCardDomain.Culture.ToLowerInvariant();
            }

            return string.Empty;
        }
    }
}