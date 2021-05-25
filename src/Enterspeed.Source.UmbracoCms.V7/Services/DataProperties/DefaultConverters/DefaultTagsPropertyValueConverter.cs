using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultTagsPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.TagsAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            var arrayItems = new List<IEnterspeedProperty>();
            if (value != null)
            {
                var isJson = value.DetectIsJson();
                var values = isJson
                    ? JsonConvert.DeserializeObject<List<string>>(value)
                    : value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var item in values)
                {
                    arrayItems.Add(new StringEnterspeedProperty(item));
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }
    }
}
