using Enterspeed.Source.UmbracoCms.V8.Factories;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class ContentCultureUrlRequiredGuard : IEnterspeedContentHandlingGuard
    {
        private readonly IUrlFactory _urlFactory;
        public ContentCultureUrlRequiredGuard(
            IUrlFactory urlFactory)
        {
            _urlFactory = urlFactory;
        }

        public bool CanIngest(IPublishedContent content, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return true;
            }

            var url = _urlFactory.GetUrl(content, content.IsDraft(), culture);
            return !string.IsNullOrWhiteSpace(url) && !url.Equals("#");
        }
    }
}