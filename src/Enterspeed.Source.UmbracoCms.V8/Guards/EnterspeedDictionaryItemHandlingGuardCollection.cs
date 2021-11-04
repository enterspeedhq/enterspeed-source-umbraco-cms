using System.Collections.Generic;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class
        EnterspeedDictionaryItemHandlingGuardCollection : BuilderCollectionBase<IEnterspeedDictionaryItemHandlingGuard>
    {
        public EnterspeedDictionaryItemHandlingGuardCollection(
            IEnumerable<IEnterspeedDictionaryItemHandlingGuard> items)
            : base(items)
        {
        }
    }
}