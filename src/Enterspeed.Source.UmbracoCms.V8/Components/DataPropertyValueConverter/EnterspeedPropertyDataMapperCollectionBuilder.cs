using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
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