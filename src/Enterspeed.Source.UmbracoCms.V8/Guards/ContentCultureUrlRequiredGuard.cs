using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class ContentCultureUrlRequiredGuard : IEnterspeedContentHandlingGuard
    {
        private readonly ILogger _logger;

        public ContentCultureUrlRequiredGuard(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanIngest(IPublishedContent content, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return true;
            }

            var url = content.Url(culture);
            if (!string.IsNullOrWhiteSpace(url) && !url.Equals("#"))
            {
                return true;
            }

            _logger.Info<ContentCultureUrlRequiredGuard>(
                "Content '{contentId}' does not have available url '{contentUrl}' for '{culture}' culture.",
                content.Id,
                url,
                culture);
            return false;
        }
    }
}