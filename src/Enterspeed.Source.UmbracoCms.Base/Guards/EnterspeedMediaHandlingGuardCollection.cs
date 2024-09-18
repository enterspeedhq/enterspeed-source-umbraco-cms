using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Guards
{
    public class
        EnterspeedMediaHandlingGuardCollection : BuilderCollectionBase<IEnterspeedMediaHandlingGuard>
    {
        public EnterspeedMediaHandlingGuardCollection(Func<IEnumerable<IEnterspeedMediaHandlingGuard>> items)
            : base(items)
        {
        }
    }
}