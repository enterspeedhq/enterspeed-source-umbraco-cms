using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V10.Models.Grid;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V10.Services.DataProperties.DefaultGridConverters
{
    public class DefaultRichTextEditorGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("rte");
        }

        public virtual IEnterspeedProperty Convert(GridControl editor, string culture)
        {
            return new StringEnterspeedProperty(editor.Value.ToString());
        }
    }
}
