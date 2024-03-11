using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Core.Guards
{
    public class EnterspeedMediaHandlingGuardCollectionBuilder
        : OrderedCollectionBuilderBase<EnterspeedMediaHandlingGuardCollectionBuilder,
            EnterspeedMediaHandlingGuardCollection,
            IEnterspeedMediaHandlingGuard>
    {
        public EnterspeedMediaHandlingGuardCollectionBuilder()
        {
        }

        protected override EnterspeedMediaHandlingGuardCollectionBuilder This => this;
    }
}