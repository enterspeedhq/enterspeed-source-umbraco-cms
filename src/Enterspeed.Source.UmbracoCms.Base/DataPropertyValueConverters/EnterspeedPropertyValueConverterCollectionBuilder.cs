using Enterspeed.Source.UmbracoCms.Base.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.DataPropertyValueConverters
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
