using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
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