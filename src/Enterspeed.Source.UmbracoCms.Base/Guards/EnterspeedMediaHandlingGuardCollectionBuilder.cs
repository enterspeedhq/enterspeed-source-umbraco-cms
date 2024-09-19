using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.Guards
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