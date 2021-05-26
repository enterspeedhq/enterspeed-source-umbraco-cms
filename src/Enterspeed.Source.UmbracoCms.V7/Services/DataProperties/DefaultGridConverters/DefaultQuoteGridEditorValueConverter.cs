using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Models.Grid;
using Umbraco.Core;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultGridConverters
{
    public class DefaultQuoteGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("quote");
        }

        public IEnterspeedProperty Convert(GridControl editor)
        {
            var value = editor.Value?.ToString();
            return new StringEnterspeedProperty(value);
        }
    }
}
