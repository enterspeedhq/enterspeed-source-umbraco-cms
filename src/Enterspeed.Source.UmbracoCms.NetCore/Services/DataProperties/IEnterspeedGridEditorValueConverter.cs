using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.NetCore.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties
{
    public interface IEnterspeedGridEditorValueConverter
    {
        bool IsConverter(string alias);
        IEnterspeedProperty Convert(GridControl editor, string culture);
    }
}
