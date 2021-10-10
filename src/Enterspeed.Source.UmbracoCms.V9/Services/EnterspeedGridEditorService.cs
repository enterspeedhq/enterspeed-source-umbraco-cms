using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Components.DataPropertyValueConverter;
using Enterspeed.Source.UmbracoCms.V9.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public class EnterspeedGridEditorService : IEnterspeedGridEditorService
    {
        private readonly EnterspeedGridEditorValueConverterCollection _converterCollection;

        public EnterspeedGridEditorService(EnterspeedGridEditorValueConverterCollection converterCollection)
        {
            _converterCollection = converterCollection;
        }

        public IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null)
        {
            var converter = _converterCollection.FirstOrDefault(x => x.IsConverter(control.Alias));
            return converter?.Convert(control, culture);
        }
    }
}
