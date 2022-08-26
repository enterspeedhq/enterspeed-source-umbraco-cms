using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V10.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IEnterspeedGridEditorService
    {
        IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null);
    }
}
