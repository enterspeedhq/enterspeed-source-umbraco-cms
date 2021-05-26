using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultRepeatableTextStringPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.MultipleTextstringAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var items = property.GetValue<string[]>();
            var arrayItems = new List<IEnterspeedProperty>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    arrayItems.Add(new StringEnterspeedProperty(item));
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }
    }
}
