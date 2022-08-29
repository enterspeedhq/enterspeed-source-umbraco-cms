using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V10.Guards
{
    public class EnterspeedContentHandlingGuardCollectionBuilder
        : OrderedCollectionBuilderBase<EnterspeedContentHandlingGuardCollectionBuilder, EnterspeedContentHandlingGuardCollection,
            IEnterspeedContentHandlingGuard>
    {
        public EnterspeedContentHandlingGuardCollectionBuilder()
        {
        }

        protected override EnterspeedContentHandlingGuardCollectionBuilder This => this;
    }
}