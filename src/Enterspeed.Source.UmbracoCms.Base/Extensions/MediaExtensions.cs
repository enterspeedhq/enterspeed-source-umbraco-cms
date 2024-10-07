using Enterspeed.Source.UmbracoCms.Base.Models.Configuration;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.Base.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaUrl(this IMedia media, EnterspeedUmbracoConfiguration configuration)
        {
            var src = configuration.MediaDomain;

            switch (media.ContentType.Alias)
            {
                case Constants.Conventions.MediaTypes.VectorGraphicsAlias:
                case Constants.Conventions.MediaTypes.ArticleAlias:
                case Constants.Conventions.MediaTypes.File:
                case Constants.Conventions.MediaTypes.VideoAlias:
                case Constants.Conventions.MediaTypes.AudioAlias:
                    src += media.GetValue<string>(Constants.Conventions.Media.File);
                    break;
                case Constants.Conventions.MediaTypes.Image:
                    var umbracoFile = media.GetValue<string>(Constants.Conventions.Media.File);
                    if (umbracoFile != null && umbracoFile.Contains("src"))
                    {
                        var umbFile = JsonConvert.DeserializeObject<ImageCropperValue>(umbracoFile);
                        src += umbFile != null ? umbFile.Src : string.Empty;
                    }

                    break;
            }

            return src;
        }
    }
}
