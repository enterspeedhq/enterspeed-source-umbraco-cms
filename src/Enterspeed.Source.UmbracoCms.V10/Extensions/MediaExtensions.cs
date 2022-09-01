using Newtonsoft.Json;
using Enterspeed.Source.UmbracoCms.V10.Models.Configuration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.V10.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaUrl(this IMedia media, EnterspeedUmbracoConfiguration configuration)
        {
            var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
            var url = string.Empty;

            if (umbracoFile != null)
            {
                url = configuration.MediaDomain + JsonConvert.DeserializeObject<ImageCropperValue>(umbracoFile)?.Src;
            }

            return url;
        }
    }
}
