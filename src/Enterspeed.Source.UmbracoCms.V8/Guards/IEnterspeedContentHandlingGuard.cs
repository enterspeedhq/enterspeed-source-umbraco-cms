using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public interface IEnterspeedContentHandlingGuard
    {
        /// <summary>
        /// Validates if content can be published.
        /// </summary>
        /// <param name="content">Content for publishing.</param>
        /// <param name="culture">Culture of content.</param>
        /// <returns>True or false, if is valid for publishing or not.</returns>
        bool CanPublish(IPublishedContent content, string culture);
    }
}