using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEntityIdentityService
    {
        string GetId(IPublishedContent content, string culture);
        string GetId(int contentId, string culture);
        string GetId(string contentId, string culture);
        string GetId(IDictionaryItem dictionaryItem, string culture);
        string GetId(Guid? id, string culture);
        string GetId(IMedia mediaItem);
        string GetId(string id);
        string GetId(int contentId);
    }
}
