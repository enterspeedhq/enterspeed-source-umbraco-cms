using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultDropdownListMultiplePropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.DropDownListMultipleAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var values = property.GetValue<string>()?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var arrayItems = new List<IEnterspeedProperty>();

            if (values != null)
            {
                foreach (var value in values)
                {
                    arrayItems.Add(new StringEnterspeedProperty(value));
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }
    }
}
