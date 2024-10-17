using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Guards
{
    public interface IEnterspeedDictionaryItemHandlingGuard
    {
        /// <summary>
        /// Validates if dictionary item can be ingested.
        /// </summary>
        /// <param name="dictionaryItem">Dictionary item for ingest.</param>
        /// <param name="culture">Culture of dictionary item.</param>
        /// <returns>True or false, if is valid for ingest or not.</returns>
        bool CanIngest(IDictionaryItem dictionaryItem, string culture);
    }
}