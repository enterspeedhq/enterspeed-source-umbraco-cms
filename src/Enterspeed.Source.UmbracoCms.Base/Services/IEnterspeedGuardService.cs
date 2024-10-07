using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedGuardService
    {
        bool CanIngest(IPublishedContent content, string culture);
        bool CanIngest(IDictionaryItem dictionaryItem, string culture);
        bool CanIngest(IMedia media, string culture);
    }
}