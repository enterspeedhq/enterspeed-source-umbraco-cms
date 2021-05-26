using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Models
{
    public class UmbracoContentEntity : IEnterspeedEntity
    {
        private readonly IPublishedContent _content;
        private readonly EntityIdentityService _entityIdentityService;

        public UmbracoContentEntity(IPublishedContent content)
        {
            _content = content;
            _entityIdentityService = EnterspeedContext.Current.Services.EntityIdentityService;
            var propertyService = EnterspeedContext.Current.Services.PropertyService;
            Properties = propertyService.GetProperties(_content);
        }

        public string Id => _entityIdentityService.GetId(_content);
        public string Type => _content.ContentType.Alias;
        public string Url => _content.Url;
        public string[] Redirects { get; }
        public string ParentId => _entityIdentityService.GetId(_content.Parent);
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
