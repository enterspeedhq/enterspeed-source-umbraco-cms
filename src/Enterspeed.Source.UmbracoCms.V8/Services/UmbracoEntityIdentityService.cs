using System;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class UmbracoEntityIdentityService : IEntityIdentityService
    {
        private readonly IUmbracoCultureProvider _umbracoCultureProvider;

        public UmbracoEntityIdentityService(IUmbracoCultureProvider umbracoCultureProvider)
        {
            _umbracoCultureProvider = umbracoCultureProvider;
        }

        public string GetId(IPublishedContent content, string culture)
        {
            if (content == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(culture) || !content.ContentType.VariesByCulture())
            {
                culture = GetDefaultCulture(content);
            }

            return GetId(content.Id, culture);
        }

        public string GetId(int contentId)
        {
            return contentId.ToString();
        }

        public string GetId(int contentId, string culture)
        {
            return GetId(contentId.ToString(), culture);
        }

        public string GetId(string contentId, string culture)
        {
            if (!string.IsNullOrWhiteSpace(culture))
            {
                return $"{contentId}-{culture}";
            }

            return $"{contentId}";
        }

        public string GetId(IDictionaryItem dictionaryItem, string culture)
        {
            if (dictionaryItem == null || string.IsNullOrWhiteSpace(culture))
            {
                return null;
            }

            return GetId(dictionaryItem.Key, culture);
        }

        public string GetId(IMedia mediaItem)
        {
            if (mediaItem == null)
            {
                return null;
            }

            return GetId(mediaItem.Id.ToString());
        }

        public string GetId(Guid? id, string culture)
        {
            if (!id.HasValue || string.IsNullOrWhiteSpace(culture))
            {
                return null;
            }

            return GetId(id.ToString(), culture);
        }

        public string GetId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return GetId(id, string.Empty);
        }

        private string GetDefaultCulture(IPublishedContent content)
        {
            return _umbracoCultureProvider.GetCultureForNonCultureVariant(content);
        }
    }
}