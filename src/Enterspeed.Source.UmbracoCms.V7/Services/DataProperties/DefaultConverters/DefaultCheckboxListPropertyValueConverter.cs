using System;
using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultCheckboxListPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.CheckBoxListAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>()?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var arrayItems = new List<IEnterspeedProperty>();
            if (value != null)
            {
                foreach (var item in value)
                {
                    arrayItems.Add(new StringEnterspeedProperty(item));
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }
    }
}
