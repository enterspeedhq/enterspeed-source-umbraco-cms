using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V10.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V10.Services.DataProperties.DefaultConverters
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
            var elementItems = property.GetValue<IEnumerable<IPublishedElement>>(culture);
            var arrayItems = new List<IEnterspeedProperty>();

            if (elementItems != null)
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
