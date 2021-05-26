using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultMediaPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultMediaPickerPropertyValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.MultipleMediaPickerAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var mediaIds = property.GetValue<string>()?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var arrayItems = new List<IEnterspeedProperty>();

            if (mediaIds != null)
            {
                var umbracoHelper = UmbracoContextHelper.GetUmbracoHelper();
                foreach (var mediaId in mediaIds)
                {
                    var media = umbracoHelper.TypedMedia(mediaId);
                    var convertedMedia = ConvertToEnterspeedProperty(media);
                    if (convertedMedia != null)
                    {
                        arrayItems.Add(convertedMedia);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }

        private ObjectEnterspeedProperty ConvertToEnterspeedProperty(IPublishedContent media)
        {
            if (media == null)
            {
                return null;
            }

            var properties = new Dictionary<string, IEnterspeedProperty>();
            properties.Add("id", new NumberEnterspeedProperty(media.Id));
            properties.Add("url", new StringEnterspeedProperty(_mediaUrlProvider.GetUrl(media)));

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
