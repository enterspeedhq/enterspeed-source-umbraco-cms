using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Models
{
    public interface IEnterspeedDictionaryTranslation
    {
        string GetIsoCode(IDictionaryTranslation translation);
    }
}
