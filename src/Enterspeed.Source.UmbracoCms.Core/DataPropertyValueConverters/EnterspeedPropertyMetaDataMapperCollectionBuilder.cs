using Enterspeed.Source.UmbracoCms.Core.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Core.DataPropertyValueConverters
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