using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Newtonsoft.Json;
using Umbraco.Core;
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

            var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
            if (umbracoFile.Contains("src"))
            {
                var umbFile = JsonConvert.DeserializeObject<UmbFile>(umbracoFile);
                if (umbFile != null)
                {
                    Url = umbFile.Src;
                }
            }
            else
            {
                // Should be a complex type, but is sometimes only a string. I don't know why.
                // Might be some behaviour in Umbraco
                Url = umbracoFile;
            }

            Properties = EnterspeedContext.Current.Services.PropertyService.GetProperties(_media);
        }

        public string Id => _entityIdentityService.GetId(_media);
        public string Type => "umbMedia";
        public string Url { get; set; }
        public string[] Redirects => null;
        public string ParentId => _entityIdentityService.GetId(_media.ParentId.ToString());
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
