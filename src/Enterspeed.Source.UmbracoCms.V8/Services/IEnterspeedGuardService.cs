using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedGuardService
    {
        bool CanPublish(IPublishedContent content, string culture);
        bool CanPublish(IDictionaryItem dictionaryItem, string culture);
    }
}