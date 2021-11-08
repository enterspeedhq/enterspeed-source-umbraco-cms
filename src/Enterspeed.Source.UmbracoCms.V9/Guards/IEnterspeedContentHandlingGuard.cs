using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V9.Guards
{
    public interface IEnterspeedContentHandlingGuard
    {
        /// <summary>
        /// Validates if content can be ingested.
        /// </summary>
        /// <param name="content">Content for ingest.</param>
        /// <param name="culture">Culture of content.</param>
        /// <returns>True or false, if is valid for ingest or not.</returns>
        bool CanIngest(IPublishedContent content, string culture);
    }
}