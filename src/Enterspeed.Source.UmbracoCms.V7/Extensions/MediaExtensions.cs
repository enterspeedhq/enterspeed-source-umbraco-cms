using Enterspeed.Source.UmbracoCms.V7.Models;
using Enterspeed.Source.UmbracoCms.V7.Models.Configuration;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaUrl(this IMedia media, EnterspeedUmbracoConfiguration configuration)
        {
            var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
            if (umbracoFile != null && umbracoFile.Contains("src"))
            {
                var umbFile = JsonConvert.DeserializeObject<UmbFile>(umbracoFile);
                return umbFile != null ? configuration.MediaDomain + umbFile.Src : string.Empty;
            }

            // Should be a complex type, but is sometimes only a string. I don't know why.
            // Might be some behaviour in Umbraco
            return umbracoFile;
        }
    }
}
