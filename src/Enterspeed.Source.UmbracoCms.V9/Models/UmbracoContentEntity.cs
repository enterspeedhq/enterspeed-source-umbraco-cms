using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Models
{
    public class UmbracoContentEntity : IEnterspeedEntity
    {
        private readonly IPublishedContent _content;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly string _culture;

        public UmbracoContentEntity(
            IPublishedContent content,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            string[] redirects,
            string culture = null)
        {
            _content = content;
            _culture = culture;
            _entityIdentityService = entityIdentityService;
            Redirects = redirects;
            Properties = propertyService.GetProperties(_content, _culture);
        }

        public string Id => _entityIdentityService.GetId(_content, _culture);
        public string Type => _content.ContentType.Alias;
        public string Url => _culture != null ? _content.GetUrl(_culture) : _content.GetUrl(null, UrlMode.Absolute);
        public string[] Redirects { get; }
        public string ParentId => _entityIdentityService.GetId(_content.Parent, _culture);
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
