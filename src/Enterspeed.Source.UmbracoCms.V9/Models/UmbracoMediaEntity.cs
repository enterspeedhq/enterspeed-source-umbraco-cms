using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V9.Models
{
    public class UmbracoMediaEntity : IEnterspeedEntity
    {
        private readonly IMedia _media;
        private readonly IEntityIdentityService _entityIdentityService;

        public UmbracoMediaEntity(
            IMedia media,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _media = media;
            _entityIdentityService = entityIdentityService;

            Url = _media.GetMediaUrl(enterspeedConfigurationService.GetConfiguration());
            Properties = propertyService.GetProperties(_media);
        }

        public string Id => _entityIdentityService.GetId(_media);
        public string Type => _media.ContentType.Name == "Folder" ? "umbMediaFolder" : "umbMedia";
        public string Url { get; set; }
        public string[] Redirects => null;
        public string ParentId => _entityIdentityService.GetId(_media.ParentId.ToString());
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
