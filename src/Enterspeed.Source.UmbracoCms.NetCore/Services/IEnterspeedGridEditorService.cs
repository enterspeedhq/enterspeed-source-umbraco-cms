using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IEnterspeedGridEditorService
    {
        IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null);
    }
}
