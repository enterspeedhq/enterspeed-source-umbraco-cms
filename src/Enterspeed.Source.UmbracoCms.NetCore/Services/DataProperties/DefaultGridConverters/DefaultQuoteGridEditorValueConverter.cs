using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.NetCore.Models.Grid;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties.DefaultGridConverters
{
    public class DefaultQuoteGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("quote");
        }

        public IEnterspeedProperty Convert(GridControl editor, string culture)
        {
            var value = editor.Value?.ToString();
            return new StringEnterspeedProperty(value);
        }
    }
}