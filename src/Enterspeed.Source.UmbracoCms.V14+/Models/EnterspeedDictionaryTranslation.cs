using Enterspeed.Source.UmbracoCms.Models;
using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCmsV14.Models
{
    public class EnterspeedDictionaryTranslation : IEnterspeedDictionaryTranslation
    {
        public string GetIsoCode(IDictionaryTranslation translation)
        {
            return translation.LanguageIsoCode;
        }
    }
}
