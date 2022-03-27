using System.Collections.Generic;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public class EnterspeedJobHandlerCollection : BuilderCollectionBase<IEnterspeedJobHandler>
    {
        public EnterspeedJobHandlerCollection(IEnumerable<IEnterspeedJobHandler> items)
            : base(items)
        {
        }
    }
}
