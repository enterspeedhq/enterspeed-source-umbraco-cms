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

        public UmbracoMediaEntity(
            IMedia media,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            IUmbracoUrlService urlService)
        {
            _media = media;
            _entityIdentityService = entityIdentityService;

            Url = urlService.GetMediaSrc(media);
            Properties = propertyService.GetProperties(_media);
        }

        public string Id => _entityIdentityService.GetId(_media);
        public string Type => "umbMedia";
        public string Url { get; set; }
        public string[] Redirects => null;
        public string ParentId => _entityIdentityService.GetId(_media.ParentId.ToString());
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
