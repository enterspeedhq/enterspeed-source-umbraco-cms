using System;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class EntityIdentityService
    {
        public string GetId(IPublishedContent content)
        {
            if (content == null)
            {
                return null;
            }

            return GetId(content.Id);
        }

        public string GetId(int contentId)
        {
            return contentId.ToString();
        }

        public string GetId(string contentId)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                return null;
            }

            if (int.TryParse(contentId, out var id))
            {
                return GetId(id);
            }

            return null;
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

            return GetId(id.Value.ToString(), culture);
        }

        public string GetId(string contentId, string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                return $"{contentId}-{culture}";
            }

            return contentId;
        }

        public string GetId(IMedia mediaItem)
        {
            if (mediaItem == null)
            {
                return null;
            }

            return GetId(mediaItem.Id.ToString());
        }
    }
}
