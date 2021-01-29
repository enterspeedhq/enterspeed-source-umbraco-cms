using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedGridEditorService
    {
        IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null);
    }
}
