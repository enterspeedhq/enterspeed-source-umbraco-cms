using Enterspeed.Source.UmbracoCms.Base.Factories;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Guards
{
    public class ContentCultureUrlRequiredGuard : IEnterspeedContentHandlingGuard
    {
        private readonly ILogger<ContentCultureUrlRequiredGuard> _logger;
        private readonly IUrlFactory _urlFactory;


        public ContentCultureUrlRequiredGuard(
            ILogger<ContentCultureUrlRequiredGuard> logger,
            IUrlFactory urlFactory)
        {
            _logger = logger;
            _urlFactory = urlFactory;
        }

        public bool CanIngest(IPublishedContent content, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return true;
            }

            var url = _urlFactory.GetUrl(content, content.IsDraft(), culture);
            if (!string.IsNullOrWhiteSpace(url) && !url.Equals("#"))
            {
                return true;
            }

            _logger.LogInformation("Content '{contentId}' does not have available url '{contentUrl}' for '{culture}' culture.", content.Id, url, culture);
            return false;
        }
    }
}