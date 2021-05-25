using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class EnterspeedGridEditorService
    {
        public IEnterspeedProperty ConvertGridEditor(GridControl control)
        {
            var converter = EnterspeedContext.Current.EnterspeedGridEditorValueConverters.FirstOrDefault(x => x.IsConverter(control.Alias));
            return converter?.Convert(control);
        }
    }
}
