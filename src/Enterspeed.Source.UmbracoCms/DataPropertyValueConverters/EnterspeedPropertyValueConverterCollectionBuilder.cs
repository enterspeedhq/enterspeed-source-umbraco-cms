using Enterspeed.Source.UmbracoCms.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.DataPropertyValueConverters
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
