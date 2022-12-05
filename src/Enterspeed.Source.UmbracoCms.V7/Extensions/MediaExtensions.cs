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
            var src = configuration.MediaDomain;

            switch (media.ContentType.Alias)
            {
                case Constants.Conventions.MediaTypes.File:
                    src += media.GetValue<string>(Constants.Conventions.Media.File);
                    break;
                case Constants.Conventions.MediaTypes.Image:
                    var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
                    if (umbracoFile != null && umbracoFile.Contains("src"))
                    {
                        var umbFile = JsonConvert.DeserializeObject<UmbFile>(umbracoFile);
                        src += umbFile != null ? umbFile.Src : string.Empty;
                    }

                    break;
            }

            return src;
        }
    }
}
