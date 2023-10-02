using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
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