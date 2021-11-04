using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
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