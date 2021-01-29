using Enterspeed.Source.UmbracoCms.V8.Services.DataProperties;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public class EnterspeedPropertyValueConverterCollectionBuilder
        : OrderedCollectionBuilderBase<EnterspeedPropertyValueConverterCollectionBuilder, EnterspeedPropertyValueConverterCollection, IEnterspeedPropertyValueConverter>
    {
        public EnterspeedPropertyValueConverterCollectionBuilder()
        {
        }

        protected override EnterspeedPropertyValueConverterCollectionBuilder This => this;
    }
}
