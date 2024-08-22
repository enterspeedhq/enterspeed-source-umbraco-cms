using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.DataPropertyValueConverters
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