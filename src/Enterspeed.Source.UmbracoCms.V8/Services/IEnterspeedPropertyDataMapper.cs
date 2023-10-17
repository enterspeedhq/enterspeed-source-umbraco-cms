using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public interface IEnterspeedPropertyDataMapper
    {
        bool IsMapper(IPublishedContent content);
        void MapAdditionalData(IDictionary<string, IEnterspeedProperty> data, IPublishedContent content, string culture);
    }
}