﻿using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultNumericPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.Integer");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<int>(culture);
            return new NumberEnterspeedProperty(property.Alias, value);
        }
    }
}
