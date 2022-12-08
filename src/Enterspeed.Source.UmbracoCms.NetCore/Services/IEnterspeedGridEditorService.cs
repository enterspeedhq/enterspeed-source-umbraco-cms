using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.NetCore.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
{
    public interface IEnterspeedGridEditorService
    {
        IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null);
    }
}
