using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Models.Grid;
using Umbraco.Core;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultGridConverters
{
    public class DefaultHeadlineGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("headline");
        }

        public IEnterspeedProperty Convert(GridControl editor, string culture)
        {
            var value = editor.Value?.ToString();
            return new StringEnterspeedProperty(value);
        }
    }
}
