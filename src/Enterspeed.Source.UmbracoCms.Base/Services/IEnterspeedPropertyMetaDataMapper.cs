using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedPropertyMetaDataMapper
    {
        bool IsMapper(IPublishedContent content);
        void MapAdditionalMetaData(IDictionary<string, IEnterspeedProperty> metaData, IPublishedContent content, string culture);
    }
}