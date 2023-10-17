using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedPropertyMetaDataService
    {
        bool IsAllowed(IPublishedContent content);
        void MapAdditionalMetaData(IDictionary<string, IEnterspeedProperty> metaData, IPublishedContent content, string culture);
    }
}