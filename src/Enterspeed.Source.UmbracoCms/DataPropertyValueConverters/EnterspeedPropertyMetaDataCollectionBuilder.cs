using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyMetaDataCollectionBuilder :
        OrderedCollectionBuilderBase<EnterspeedPropertyMetaDataCollectionBuilder, EnterspeedPropertyMetaDataCollection, IEnterspeedPropertyMetaDataService>
    {
        public EnterspeedPropertyMetaDataCollectionBuilder()
        {
        }

        protected override EnterspeedPropertyMetaDataCollectionBuilder This => this;
    }
}