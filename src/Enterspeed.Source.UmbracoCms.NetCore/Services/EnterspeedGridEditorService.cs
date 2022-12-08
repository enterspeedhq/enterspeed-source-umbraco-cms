using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.NetCore.DataPropertyValueConverters;
using Enterspeed.Source.UmbracoCms.NetCore.Models.Grid;

namespace Enterspeed.Source.UmbracoCms.NetCore.Services
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
