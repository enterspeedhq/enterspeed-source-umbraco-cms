using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public class EnterspeedJobHandlerCollection : BuilderCollectionBase<IEnterspeedJobHandler>
    {
        public EnterspeedJobHandlerCollection(Func<IEnumerable<IEnterspeedJobHandler>> items) : base(items)
        {
        }
    }
}
