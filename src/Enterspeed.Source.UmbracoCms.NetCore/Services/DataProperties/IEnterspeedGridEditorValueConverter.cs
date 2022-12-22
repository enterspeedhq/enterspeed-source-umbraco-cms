using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties
{
    public interface IEnterspeedGridEditorValueConverter
    {
        bool IsConverter(string alias);
        IEnterspeedProperty Convert(GridControl editor, string culture);
    }
}
