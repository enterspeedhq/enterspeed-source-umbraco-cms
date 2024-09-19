using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.DataPropertyValueConverters
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