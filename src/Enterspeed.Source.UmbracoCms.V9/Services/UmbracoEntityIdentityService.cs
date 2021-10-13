using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public class UmbracoEntityIdentityService : IEntityIdentityService
    {
        private readonly IUmbracoContextProvider _umbracoContextProvider;

        public UmbracoEntityIdentityService(IUmbracoContextProvider umbracoContextProvider)
        {
            _umbracoContextProvider = umbracoContextProvider;
        }

        public string GetId(IPublishedContent content, string culture)
        {
            if (content == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(culture) || !content.ContentType.VariesByCulture())
            {
                culture = GetDefaultCulture();
            }

            return GetId(content.Id, culture);
        }

        public string GetId(int contentId, string culture)
        {
            return GetId(contentId.ToString(), culture);
        }

        public string GetId(string contentId, string culture)
        {
            return $"{contentId}-{culture}";
        }

        public string GetId(IDictionaryItem dictionaryItem, string culture)
        {
            if (dictionaryItem == null || string.IsNullOrWhiteSpace(culture))
            {
                return null;
            }

            return GetId(dictionaryItem.Key, culture);
        }

        public string GetId(Guid? id, string culture)
        {
            if (!id.HasValue || string.IsNullOrWhiteSpace(culture))
            {
                return null;
            }

            return GetId(id.ToString(), culture);
        }

        private string GetDefaultCulture()
        {
            return _umbracoContextProvider.GetContext().Domains.DefaultCulture.ToLowerInvariant();
        }
    }
}