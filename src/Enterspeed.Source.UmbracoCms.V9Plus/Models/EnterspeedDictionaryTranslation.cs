using Enterspeed.Source.UmbracoCms.Base.Models;
using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V9Plus.Models
{
    public class EnterspeedDictionaryTranslation : IEnterspeedDictionaryTranslation
    {
        public string GetIsoCode(IDictionaryTranslation translation)
        {
            return translation.Language?.IsoCode;
        }
    }
}
