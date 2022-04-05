using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public class EnterspeedJobHandlerCollectionBuilder : OrderedCollectionBuilderBase<EnterspeedJobHandlerCollectionBuilder, EnterspeedJobHandlerCollection,
        IEnterspeedJobHandler>
    {
        public EnterspeedJobHandlerCollectionBuilder()
        {
        }

        protected override EnterspeedJobHandlerCollectionBuilder This => this;
    }
}
