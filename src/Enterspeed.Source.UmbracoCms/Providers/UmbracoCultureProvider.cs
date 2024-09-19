using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
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

        private string GetDefaultCulture(IUmbracoContext umbracoContext)
        {
            return umbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }
    }
}