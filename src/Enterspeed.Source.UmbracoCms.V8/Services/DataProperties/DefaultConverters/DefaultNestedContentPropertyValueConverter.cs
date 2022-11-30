using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultNestedContentPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.NestedContent");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var elementItems = new List<IPublishedElement>();

            // Nested content be both single element, Enumerable or null
            if (property.GetValue() is IPublishedElement)
            {
                elementItems.Add(property.GetValue<IPublishedElement>(culture));
            }
            else if (property.GetValue() is IEnumerable<IPublishedElement>)
            {
                elementItems.AddRange(property.GetValue<IEnumerable<IPublishedElement>>(culture));
            }

            var arrayItems = new List<IEnterspeedProperty>();
            if (elementItems.Any())
            {
                // NOTE: This needs to be resolved manually, since it would cause a circular dependency if injected through constructor
                var dataPropertyService = Current.Factory.GetInstance<IEnterspeedPropertyService>();
                foreach (var item in elementItems)
                {
                    var properties = dataPropertyService.ConvertProperties(item.Properties, culture);
                    if (properties != null && properties.Any())
                    {
                        properties.Add("contentType", new StringEnterspeedProperty(item.ContentType.Alias));
                        arrayItems.Add(new ObjectEnterspeedProperty(properties));
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }
    }
}
