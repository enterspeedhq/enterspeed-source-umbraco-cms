using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.Guards
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