using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Models
{
    public class UmbracoMasterContentEntity : IEnterspeedEntity<IDictionary<string, IEnterspeedProperty>>
    {
        private readonly IPublishedContent _content;
        private readonly IEntityIdentityService _entityIdentityService;

        public UmbracoMasterContentEntity(
            IPublishedContent content,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService)
        {
            _content = content;
            _entityIdentityService = entityIdentityService;
            Properties = propertyService.GetMasterContentProperties(_content);
        }

        public string Id => _entityIdentityService.GetId(_content.Id);
        public string Type => $"{_content.ContentType.Alias}-master";
        public string Url { get; }
        public string[] Redirects { get; }
        public string ParentId => _content.Parent != null ? _entityIdentityService.GetId(_content.Parent.Id) : null;
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}