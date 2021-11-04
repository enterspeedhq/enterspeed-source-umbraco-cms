using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V9.Guards
{
    public interface IEnterspeedDictionaryItemHandlingGuard
    {
        /// <summary>
        /// Validates if dictionary item can be published.
        /// </summary>
        /// <param name="dictionaryItem">Dictionary item for publishing.</param>
        /// <param name="culture">Culture of dictionary item.</param>
        /// <returns>True or false, if is valid for publishing or not.</returns>
        bool CanPublish(IDictionaryItem dictionaryItem, string culture);
    }
}