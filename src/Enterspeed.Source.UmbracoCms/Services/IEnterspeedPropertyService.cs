using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedPropertyService
    {
        IDictionary<string, IEnterspeedProperty> GetProperties(IPublishedContent content, string culture = null);
        IDictionary<string, IEnterspeedProperty> GetMasterContentProperties(IPublishedContent content);
        IDictionary<string, IEnterspeedProperty> ConvertProperties(IEnumerable<IPublishedProperty> properties, string culture = null);
        IDictionary<string, IEnterspeedProperty> GetProperties(IDictionaryItem dictionaryItem, string culture);
        IDictionary<string, IEnterspeedProperty> GetProperties(IMedia media);
    }
}
