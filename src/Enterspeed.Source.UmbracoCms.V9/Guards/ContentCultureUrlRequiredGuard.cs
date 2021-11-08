using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Guards
{
    public class ContentCultureUrlRequiredGuard : IEnterspeedContentHandlingGuard
    {
        public bool CanIngest(IPublishedContent content, string culture)
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