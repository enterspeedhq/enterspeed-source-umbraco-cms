using Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.DataPropertyValueConverters
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
