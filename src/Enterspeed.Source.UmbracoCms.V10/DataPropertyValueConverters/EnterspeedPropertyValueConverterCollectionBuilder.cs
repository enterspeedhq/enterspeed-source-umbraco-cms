using Enterspeed.Source.UmbracoCms.V10.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V10.DataPropertyValueConverters
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
