using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.Guards
{
    public class EnterspeedContentHandlingGuardCollection : BuilderCollectionBase<IEnterspeedContentHandlingGuard>
    {
        public EnterspeedContentHandlingGuardCollection(Func<IEnumerable<IEnterspeedContentHandlingGuard>> items)
            : base(items)
        {
        }
    }
}