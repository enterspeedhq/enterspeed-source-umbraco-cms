﻿using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Providers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.Services.DataProperties.DefaultConverters
{
    public class DefaultMediaPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IUmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultMediaPickerPropertyValueConverter(IUmbracoMediaUrlProvider mediaUrlProvider)
        {
            _mediaUrlProvider = mediaUrlProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MediaPicker3");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var isMultiple = property.PropertyType.DataType.ConfigurationAs<MediaPicker3Configuration>().Multiple;
            var arrayItems = new List<IEnterspeedProperty>();
            if (isMultiple)
            {
                var items = property.GetValue<IEnumerable<IPublishedContent>>(culture);
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
                var item = property.GetValue<IPublishedContent>(culture);
                var mediaObject = ConvertToEnterspeedProperty(item);
                if (mediaObject != null)
                {
                    arrayItems.Add(mediaObject);
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
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
