using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultRelatedLinksPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.RelatedLinksAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<JArray>();
            var arrayItems = new List<IEnterspeedProperty>();

            if (value != null)
            {
                foreach (var item in value)
                {
                    var convertedItem = ConvertToEnterspeedProperty(item);
                    if (convertedItem != null)
                    {
                        arrayItems.Add(convertedItem);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }

        private static ObjectEnterspeedProperty ConvertToEnterspeedProperty(JToken item)
        {
            if (item == null)
            {
                return null;
            }

            var properties = new Dictionary<string, IEnterspeedProperty>();

            var caption = item.Value<string>("caption");
            var title = item.Value<string>("title");
            var newWindow = item.Value<bool>("newWindow");
            var isInternal = item.Value<bool>("isInternal");

            string url;
            if (isInternal)
            {
                var nodeId = item.Value<int>("internal");
                url = UmbracoContextHelper.GetUmbracoHelper().UrlAbsolute(nodeId);
            }
            else
            {
                url = item.Value<string>("link");
            }

            properties.Add("caption", new StringEnterspeedProperty(caption));
            properties.Add("title", new StringEnterspeedProperty(title));
            properties.Add("newWindow", new BooleanEnterspeedProperty(newWindow));
            properties.Add("url", new StringEnterspeedProperty(url));

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
