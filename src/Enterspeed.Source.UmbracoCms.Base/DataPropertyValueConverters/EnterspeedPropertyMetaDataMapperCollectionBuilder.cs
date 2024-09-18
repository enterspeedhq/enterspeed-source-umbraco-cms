using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.DataPropertyValueConverters
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