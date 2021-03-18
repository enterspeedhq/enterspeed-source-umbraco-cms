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
            var elementItems = property.GetValue<IEnumerable<PublishedElementModel>>(culture);
            var arrayItems = new List<IEnterspeedProperty>();

            if (elementItems != null)
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
