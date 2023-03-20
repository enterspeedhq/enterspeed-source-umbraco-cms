using Enterspeed.Source.UmbracoCms.V8.Services;
using System.Collections.Generic;
using System;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Providers
{
    public class UmbracoCultureProvider : IUmbracoCultureProvider
    {
        private readonly IUmbracoContextProvider _umbracoContextProvider;

        public UmbracoCultureProvider(IUmbracoContextProvider umbracoContextProvider)
        {
            _umbracoContextProvider = umbracoContextProvider;
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
            return GetDefaultCulture(umbracoContext);
        }

        public virtual string GetCultureForNonCultureVariant(IPublishedContent content)
        {
            if (content.ContentType.VariesByCulture())
            {
                throw new Exception($"Node with id '{content.Id}' varies by culture.");
            }

            var umbracoContext = _umbracoContextProvider.GetContext();
            return GetDefaultCulture(umbracoContext);
        }

        private string GetDefaultCulture(UmbracoContext umbracoContext)
        {
            return umbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }
    }
}