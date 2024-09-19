using System.Globalization;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.Extensions
{
    public static class PublishedContentExtensions
    {
        public static string GetUrl(this IPublishedContent content, ILogger logger, string culture = null, UrlMode mode = UrlMode.Default)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return content.Url(mode: mode);
            }

            try
            {
                // We want to make sure they are correctly cased, as 'en-us' will become 'en-US'
                var normalizedCultureName = CultureInfo.GetCultureInfo(culture).Name;
                return content.Url(normalizedCultureName, mode);
            }
            catch (CultureNotFoundException exception)
            {
                logger?.LogError(exception, "Error to get culture info for {contentId} {culture}", content.Id, culture);
            }

            return content.Url(culture, mode);
        }
    }
}