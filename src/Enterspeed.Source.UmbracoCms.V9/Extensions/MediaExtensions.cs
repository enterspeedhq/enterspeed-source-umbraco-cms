using Newtonsoft.Json;
using Enterspeed.Source.UmbracoCms.V9.Models.Configuration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.V9.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaUrl(this IMedia media, EnterspeedUmbracoConfiguration configuration)
        {

            var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
            if (umbracoFile != null && umbracoFile.Contains("src"))
            {
                var umbFile = JsonConvert.DeserializeObject<ImageCropperValue>(umbracoFile);
                return umbFile != null ? configuration.MediaDomain + umbFile.Src : string.Empty;
            }

            // Should be a complex type, but is sometimes only a string. I don't know why.
            // Might be some behaviour in Umbraco
            return umbracoFile;
        }
    }
}
