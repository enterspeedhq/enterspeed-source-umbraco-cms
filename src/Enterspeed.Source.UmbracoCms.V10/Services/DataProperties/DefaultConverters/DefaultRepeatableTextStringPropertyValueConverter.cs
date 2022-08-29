﻿using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V10.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V10.Services.DataProperties.DefaultConverters
{
    public class DefaultRepeatableTextStringPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MultipleTextstring");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var items = property.GetValue<string[]>(culture);
            var arrayItems = new List<IEnterspeedProperty>();
            foreach (var item in items)
            {
                arrayItems.Add(new StringEnterspeedProperty(item));
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }
    }
}
