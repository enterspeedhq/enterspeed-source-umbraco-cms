using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedGuardService
    {
        bool CanIngest(IPublishedContent content, string culture);
        bool CanIngest(IDictionaryItem dictionaryItem, string culture);
        bool CanIngest(IMedia media, string culture);
    }
}