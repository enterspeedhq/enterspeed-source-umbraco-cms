using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.Guards
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