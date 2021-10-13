using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Models.Grid;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultGridConverters
{
    public class DefaultRichTextEditorGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("rte");
        }

        public IEnterspeedProperty Convert(GridControl editor, string culture)
        {
            return new StringEnterspeedProperty(editor.Value.ToString());
        }
    }
}
