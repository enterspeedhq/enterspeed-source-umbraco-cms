using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedGridEditorService
    {
        IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null);
    }
}
