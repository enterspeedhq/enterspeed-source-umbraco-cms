using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IEnterspeedGuardService
    {
        bool CanPublish(IPublishedContent content, string culture);
        bool CanPublish(IDictionaryItem dictionaryItem, string culture);
    }
}