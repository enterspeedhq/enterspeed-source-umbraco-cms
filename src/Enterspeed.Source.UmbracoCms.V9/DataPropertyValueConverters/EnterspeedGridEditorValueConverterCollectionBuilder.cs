using Enterspeed.Source.UmbracoCms.V9.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.DataPropertyValueConverters
{
    public class EnterspeedGridEditorValueConverterCollectionBuilder
        : OrderedCollectionBuilderBase<EnterspeedGridEditorValueConverterCollectionBuilder, EnterspeedGridEditorValueConverterCollection, IEnterspeedGridEditorValueConverter>
    {
        public EnterspeedGridEditorValueConverterCollectionBuilder()
        {
        }

        protected override EnterspeedGridEditorValueConverterCollectionBuilder This => this;
    }
}
