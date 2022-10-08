using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedPropertyService
    {
        IDictionary<string, IEnterspeedProperty> GetProperties(IPublishedContent content, string culture = null);
        IDictionary<string, IEnterspeedProperty> ConvertProperties(IEnumerable<IPublishedProperty> properties, string culture = null);
        IDictionary<string, IEnterspeedProperty> GetProperties(IDictionaryItem dictionaryItem, string culture);
        IDictionary<string, IEnterspeedProperty> GetProperties(IMedia media);
    }
}
