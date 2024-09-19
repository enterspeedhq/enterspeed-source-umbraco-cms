using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultNestedContentPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultNestedContentPropertyValueConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.NestedContent");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var elementItems = new List<IPublishedElement>();

            if (property.GetValue() != null)
            {
                // Nested content be both single element, Enumerable or null
                if (property.GetValue() is IPublishedElement)
                {
                    elementItems.Add(property.GetValue<IPublishedElement>(culture));
                }
                else if (property.GetValue() is IEnumerable<IPublishedElement>)
                {
                    elementItems.AddRange(property.GetValue<IEnumerable<IPublishedElement>>(culture));
                }
            }

            var arrayItems = new List<IEnterspeedProperty>();

            if (elementItems.Any())
            {
                // NOTE: This needs to be resolved manually, since it would cause a circular dependency if injected through constructor
                var dataPropertyService = _serviceProvider.GetRequiredService<IEnterspeedPropertyService>();
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
