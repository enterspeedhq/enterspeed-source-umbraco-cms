using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultMediaPicker2PropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultMediaPicker2PropertyValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Umbraco.MediaPicker2");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var isMultiple = property.GetValue<IEnumerable<IPublishedContent>>() != null;
            var arrayItems = new List<IEnterspeedProperty>();
            if (isMultiple)
            {
                var items = property.GetValue<IEnumerable<IPublishedContent>>();
                foreach (var item in items)
                {
                    var mediaObject = ConvertToEnterspeedProperty(item);
                    if (mediaObject != null)
                    {
                        arrayItems.Add(mediaObject);
                    }
                }
            }
            else
            {
                var item = property.GetValue<IPublishedContent>();
                var mediaObject = ConvertToEnterspeedProperty(item);
                if (mediaObject != null)
                {
                    arrayItems.Add(mediaObject);
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