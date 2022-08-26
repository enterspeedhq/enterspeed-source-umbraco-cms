using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public interface IEntityIdentityService
    {
        string GetId(IPublishedContent content, string culture);
        string GetId(int contentId, string culture);
        string GetId(string contentId, string culture);
        string GetId(IDictionaryItem dictionaryItem, string culture);
        string GetId(Guid? id, string culture);
    }
}
