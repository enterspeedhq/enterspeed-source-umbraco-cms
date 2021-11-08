using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V9.Services
{
    public interface IEnterspeedGuardService
    {
        bool CanIngest(IPublishedContent content, string culture);
        bool CanIngest(IDictionaryItem dictionaryItem, string culture);
    }
}