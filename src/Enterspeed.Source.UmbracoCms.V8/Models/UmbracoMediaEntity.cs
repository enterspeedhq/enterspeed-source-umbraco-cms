using System.Collections.Generic;
using System.Web;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.V8.Models
{
    public class UmbracoMediaEntity : IEnterspeedEntity
    {
        private readonly IMedia _media;
        private readonly IEntityIdentityService _entityIdentityService;

        public UmbracoMediaEntity(
            IMedia media,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService)
        {
            _media = media;
            _entityIdentityService = entityIdentityService;
            var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
            var url = JsonConvert.DeserializeObject<ImageCropperValue>(umbracoFile).Src;
            Url = url;
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
