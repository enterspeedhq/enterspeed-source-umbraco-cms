using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Models
{
    public interface IEnterspeedDictionaryTranslation
    {
        string GetIsoCode(IDictionaryTranslation translation);
    }
}
