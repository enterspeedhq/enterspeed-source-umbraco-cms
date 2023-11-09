using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public sealed class EnterspeedPropertyMetaDataMapperCollectionBuilder :
        OrderedCollectionBuilderBase<EnterspeedPropertyMetaDataMapperCollectionBuilder, EnterspeedPropertyMetaDataMapperCollection, IEnterspeedPropertyMetaDataMapper>
    {
        public EnterspeedPropertyMetaDataMapperCollectionBuilder()
        {
        }

        protected override EnterspeedPropertyMetaDataMapperCollectionBuilder This => this;
    }
}