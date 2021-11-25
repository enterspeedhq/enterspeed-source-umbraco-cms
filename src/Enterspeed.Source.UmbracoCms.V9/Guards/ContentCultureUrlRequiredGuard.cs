using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Guards
{
    public class ContentCultureUrlRequiredGuard : IEnterspeedContentHandlingGuard
    {
        private ILogger<ContentCultureUrlRequiredGuard> _logger;

        public ContentCultureUrlRequiredGuard(ILogger<ContentCultureUrlRequiredGuard> logger)
        {
            _logger = logger;
        }

        public bool CanIngest(IPublishedContent content, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return true;
            }

            var url = content.GetUrl(_logger, culture);
            if (!string.IsNullOrWhiteSpace(url) && !url.Equals("#"))
            {
                return true;
            }
            
            _logger.LogInformation("Content '{contentId}' does not have available url '{contentUrl}' for '{culture}' culture.", content.Id, url, culture);
            return false;
        }
    }
}