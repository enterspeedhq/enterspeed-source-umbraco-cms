using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Guards
{
    public class EnterspeedDictionaryItemHandlingGuardCollectionBuilder
        : OrderedCollectionBuilderBase<EnterspeedDictionaryItemHandlingGuardCollectionBuilder,
            EnterspeedDictionaryItemHandlingGuardCollection,
            IEnterspeedDictionaryItemHandlingGuard>
    {
        public EnterspeedDictionaryItemHandlingGuardCollectionBuilder()
        {
        }

        protected override EnterspeedDictionaryItemHandlingGuardCollectionBuilder This => this;
    }
}