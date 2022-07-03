using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.V8.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaUrl(this IMedia media, EnterspeedUmbracoConfiguration configuration)
        {
            var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
            var url = string.Empty;

            if (umbracoFile != null)
            {
                url = configuration.MediaDomain + JsonConvert.DeserializeObject<ImageCropperValue>(umbracoFile).Src;
            }

            return url;
        }
    }
}
