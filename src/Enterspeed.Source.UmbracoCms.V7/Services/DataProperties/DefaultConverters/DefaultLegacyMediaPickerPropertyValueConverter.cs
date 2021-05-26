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
    public class DefaultLegacyMediaPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultLegacyMediaPickerPropertyValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.MediaPickerAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();

            ObjectEnterspeedProperty mediaValue = null;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out var mediaId))
            {
                var media = UmbracoContextHelper.GetUmbracoHelper().TypedMedia(mediaId);
                mediaValue = ConvertToEnterspeedProperty(media);
            }

            return new ObjectEnterspeedProperty(property.PropertyTypeAlias, mediaValue?.Properties);
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
