using System;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultDatePickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.DateAlias)
                   || propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.DateTimeAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            if (DateTime.TryParse(value, out var date) && date == default)
            {
                value = null;
            }

            return new StringEnterspeedProperty(property.PropertyTypeAlias, value);
        }
    }
}
