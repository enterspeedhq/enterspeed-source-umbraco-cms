using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public interface IEnterspeedMediaHandlingGuard
    {
        /// <summary>
        /// Validates if dictionary item can be ingested.
        /// </summary>
        /// <param name="media">Dictionary item for ingest.</param>
        /// <param name="culture">Culture of dictionary item.</param>
        /// <returns>True or false, if is valid for ingest or not.</returns>
        bool CanIngest(IMedia media, string culture);
    }
}