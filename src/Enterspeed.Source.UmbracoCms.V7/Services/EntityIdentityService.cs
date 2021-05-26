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
    }
}
