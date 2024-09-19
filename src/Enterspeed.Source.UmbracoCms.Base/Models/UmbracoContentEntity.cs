using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Factories;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Models
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
            string[] redirects,
            IUrlFactory urlFactory,
            string culture = null)
        {
            _content = content;
            _culture = culture;
            _entityIdentityService = entityIdentityService;
            _urlFactory = urlFactory;
            Redirects = redirects;
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