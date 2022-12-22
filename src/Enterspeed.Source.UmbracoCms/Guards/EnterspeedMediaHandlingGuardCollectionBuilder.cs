using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Guards
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