using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
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
