using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties
{
    public interface IEnterspeedGridEditorValueConverter
    {
        bool IsConverter(string alias);
        IEnterspeedProperty Convert(GridControl editor, string culture);
    }
}
