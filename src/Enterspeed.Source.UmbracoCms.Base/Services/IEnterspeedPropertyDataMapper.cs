using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedPropertyDataMapper
    {
        bool IsMapper(IPublishedContent content);
        void MapAdditionalData(IDictionary<string, IEnterspeedProperty> data, IPublishedContent content, string culture);
    }
}