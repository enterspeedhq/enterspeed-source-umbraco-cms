using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Models
{
    public class UmbracoMediaEntity : IEnterspeedEntity
    {
        private readonly IMedia _media;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly string _culture;

        public UmbracoMediaEntity(
            IMedia media,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            string culture)
        {
            _media = media;
            _entityIdentityService = entityIdentityService;
            _culture = culture;
            Properties = propertyService.GetProperties(_media, _culture);
        }

        public string Id => _entityIdentityService.GetId(_media);
        public string Type => "umbMedia";
        public string Url => null;
        public string[] Redirects => null;
        public string ParentId => _entityIdentityService.GetId(_media.ParentId, _culture);
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
