using Enterspeed.Source.UmbracoCms.V8.Services.DataProperties;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
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
