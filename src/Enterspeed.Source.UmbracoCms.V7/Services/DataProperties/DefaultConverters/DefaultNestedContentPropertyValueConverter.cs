using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultNestedContentPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Umbraco.NestedContent");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var elementItems = new List<IPublishedContent>();

            // Nested content be both single element, Enumerable or null
            if (property.GetValue<IPublishedContent>() != null)
            {
                elementItems.Add(property.GetValue<IPublishedContent>());
            }
            else if (property.GetValue<IEnumerable<IPublishedContent>>() != null)
            {
                elementItems.AddRange(property.GetValue<IEnumerable<IPublishedContent>>());
            }

            var arrayItems = new List<IEnterspeedProperty>();
            if (elementItems.Any())
            {
                // NOTE: This needs to be resolved manually, since it would cause a circular dependency if injected through constructor
                var propertyService = EnterspeedContext.Current.Services.PropertyService;
                foreach (var item in elementItems)
                {
                    var properties = propertyService.ConvertProperties(item);
                    if (properties != null && properties.Any())
                    {
                        properties.Add("contentType", new StringEnterspeedProperty(item.ContentType.Alias));
                        arrayItems.Add(new ObjectEnterspeedProperty(properties));
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }
    }
}