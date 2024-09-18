using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyDataMapperCollectionBuilder :
        OrderedCollectionBuilderBase<EnterspeedPropertyDataMapperCollectionBuilder, EnterspeedPropertyDataMapperCollection, IEnterspeedPropertyDataMapper>
    {
        public EnterspeedPropertyDataMapperCollectionBuilder()
        {
        }

        protected override EnterspeedPropertyDataMapperCollectionBuilder This => this;
    }
}