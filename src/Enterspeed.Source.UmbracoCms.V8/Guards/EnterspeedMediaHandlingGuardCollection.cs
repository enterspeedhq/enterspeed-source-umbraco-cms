using System.Collections.Generic;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class
        EnterspeedMediaHandlingGuardCollection : BuilderCollectionBase<IEnterspeedMediaHandlingGuard>
    {
        public EnterspeedMediaHandlingGuardCollection(
            IEnumerable<IEnterspeedMediaHandlingGuard> items)
            : base(items)
        {
        }
    }
}