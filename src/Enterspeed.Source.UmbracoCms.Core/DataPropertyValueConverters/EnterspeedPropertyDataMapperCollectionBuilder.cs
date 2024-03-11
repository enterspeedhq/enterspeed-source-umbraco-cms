using Enterspeed.Source.UmbracoCms.Core.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Core.DataPropertyValueConverters
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