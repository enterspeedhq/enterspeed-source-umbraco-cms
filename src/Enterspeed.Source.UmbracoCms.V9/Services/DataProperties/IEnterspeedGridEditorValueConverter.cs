using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties
{
    public interface IEnterspeedGridEditorValueConverter
    {
        bool IsConverter(string alias);
        IEnterspeedProperty Convert(GridControl editor, string culture);
    }
}
