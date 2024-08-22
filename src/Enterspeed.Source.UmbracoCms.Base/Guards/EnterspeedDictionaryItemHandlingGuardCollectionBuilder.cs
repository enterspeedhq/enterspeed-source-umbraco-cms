using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.Guards
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