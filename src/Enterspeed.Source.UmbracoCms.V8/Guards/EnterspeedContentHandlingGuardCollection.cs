using System;
using System.Collections.Generic;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class EnterspeedContentHandlingGuardCollection : BuilderCollectionBase<IEnterspeedContentHandlingGuard>
    {
        public EnterspeedContentHandlingGuardCollection(IEnumerable<IEnterspeedContentHandlingGuard> items)
            : base(items)
        {
        }
    }
}