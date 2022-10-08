using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Models
{
    public class UmbracoMediaEntity : IEnterspeedEntity
    {
        private readonly IMedia _media;
        private readonly EntityIdentityService _entityIdentityService;

        public UmbracoMediaEntity(IMedia media)
        {
            _media = media;
            _entityIdentityService = EnterspeedContext.Current.Services.EntityIdentityService;
            var configurationService = EnterspeedContext.Current.Services.ConfigurationService;

            Url = media.GetMediaUrl(configurationService.GetConfiguration());
            Properties = EnterspeedContext.Current.Services.PropertyService.GetProperties(_media);
        }

        public string Id => _entityIdentityService.GetId(_media);
        public string Type => _media.ContentType.Name == "Folder" ? "umbMediaFolder" : "umbMedia";
        public string Url { get; set; }
        public string[] Redirects => null;
        public string ParentId => _entityIdentityService.GetId(_media.ParentId.ToString());
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
