#if UMBRACO_18_OR_GREATER
using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultElementPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultElementPickerPropertyValueConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.ElementPicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            // The core value converter resolves the stored element keys against the element
            // cache and silently filters out deleted and unpublished elements, so for the
            // publish content state the resolved list never contains draft element content.
            var elements = property.GetValue<IEnumerable<IPublishedElement>>(culture);
            var dataPropertyService = _serviceProvider.GetRequiredService<IEnterspeedPropertyService>();

            var arrayItems = new List<IEnterspeedProperty>();
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    var elementObject = MapElement(element, culture, dataPropertyService);
                    if (elementObject != null)
                    {
                        arrayItems.Add(elementObject);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }

        protected IEnterspeedProperty MapElement(IPublishedElement element, string culture, IEnterspeedPropertyService dataPropertyService)
        {
            if (element == null)
            {
                return null;
            }

            var properties = new Dictionary<string, IEnterspeedProperty>
            {
                { "key", new StringEnterspeedProperty(element.Key.ToString()) },
                { "contentType", new StringEnterspeedProperty(element.ContentType.Alias) }
            };

            if (element.Properties != null)
            {
                var contentProperties = dataPropertyService.ConvertProperties(element.Properties, culture);
                properties.Add("content", new ObjectEnterspeedProperty(contentProperties));
            }

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
#endif
