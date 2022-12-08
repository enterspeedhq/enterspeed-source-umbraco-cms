using Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.DataPropertyValueConverters
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
