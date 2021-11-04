using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class ContentCultureUrlRequiredGuard : IEnterspeedContentHandlingGuard
    {
        public bool CanPublish(IPublishedContent content, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return true;
            }

            var url = content.Url(culture);
            return !string.IsNullOrWhiteSpace(url) && !url.Equals("#");
        }
    }
}