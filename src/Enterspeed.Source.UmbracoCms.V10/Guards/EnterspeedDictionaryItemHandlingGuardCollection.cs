using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V10.Guards
{
    public class
        EnterspeedDictionaryItemHandlingGuardCollection : BuilderCollectionBase<IEnterspeedDictionaryItemHandlingGuard>
    {
        public EnterspeedDictionaryItemHandlingGuardCollection(
            Func<IEnumerable<IEnterspeedDictionaryItemHandlingGuard>> items)
            : base(items)
        {
        }
    }
}