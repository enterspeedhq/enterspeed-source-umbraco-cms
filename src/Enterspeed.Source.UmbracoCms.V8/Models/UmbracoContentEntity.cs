using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Models
{
    public class UmbracoContentEntity : IEnterspeedEntity<IDictionary<string, IEnterspeedProperty>>
    {
        private readonly IPublishedContent _content;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IUrlFactory _urlFactory;
        private readonly string _culture;

        public UmbracoContentEntity(
            IPublishedContent content,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            IUrlFactory urlFactory,
            string[] redirects,
            string culture = null)
        {
            _content = content;
            _culture = culture;
            _entityIdentityService = entityIdentityService;
            Redirects = redirects;
            _urlFactory = urlFactory;
            Properties = propertyService.GetProperties(_content, _culture);
        }

        public string Id => _entityIdentityService.GetId(_content, _culture);
        public string Type => _content.ContentType.Alias;
        public string Url => _urlFactory.GetUrl(_content, _content.IsDraft(_culture), _culture);
        public string[] Redirects { get; }
        public string ParentId => _entityIdentityService.GetId(_content.Parent, _culture);
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
