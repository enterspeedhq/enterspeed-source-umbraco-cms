using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Models.Grid;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultGridLayoutPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly EnterspeedGridEditorService _enterspeedGridEditorService;

        public DefaultGridLayoutPropertyValueConverter()
        {
            _enterspeedGridEditorService = EnterspeedContext.Current.Services.GridEditorService;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.GridAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<JObject>();

            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                properties = new Dictionary<string, IEnterspeedProperty>();
                foreach (var prop in value)
                {
                    var enterspeedProperty = GetProperty(prop.Key, prop.Value);
                    if (enterspeedProperty != null)
                    {
                        properties.Add(prop.Key, enterspeedProperty);
                    }
                }
            }

            return new ObjectEnterspeedProperty(property.PropertyTypeAlias, properties);
        }

        private IEnterspeedProperty GetProperty(string name, JToken value)
        {
            var type = value.Type;
            if (type == JTokenType.String)
            {
                return new StringEnterspeedProperty(name, value.Value<string>());
            }

            if (type == JTokenType.Boolean)
            {
                return new BooleanEnterspeedProperty(name, value.Value<bool>());
            }

            if (type == JTokenType.Integer)
            {
                return new NumberEnterspeedProperty(name, value.Value<int>());
            }

            if (type == JTokenType.Array)
            {
                var arrayItems = new List<IEnterspeedProperty>();
                foreach (var jToken in (JArray)value)
                {
                    if (jToken is JObject item)
                    {
                        var properties = new Dictionary<string, IEnterspeedProperty>();
                        foreach (var prop in item)
                        {
                            IEnterspeedProperty property = null;
                            if (name == "controls" && prop.Key == "value")
                            {
                                var gridControl = new GridControl(item);
                                property = _enterspeedGridEditorService.ConvertGridEditor(gridControl);
                            }

                            if (property == null)
                            {
                                property = GetProperty(prop.Key, prop.Value);
                            }

                            if (property != null)
                            {
                                properties.Add(prop.Key, property);
                            }
                        }

                        if (properties.Any())
                        {
                            arrayItems.Add(new ObjectEnterspeedProperty(properties));
                        }
                    }
                }

                return new ArrayEnterspeedProperty(name, arrayItems.ToArray());
            }

            if (type == JTokenType.Object)
            {
                if (value is JObject item)
                {
                    var properties = new Dictionary<string, IEnterspeedProperty>();
                    foreach (var prop in item)
                    {
                        var property = GetProperty(prop.Key, prop.Value);
                        if (property != null)
                        {
                            properties.Add(prop.Key, property);
                        }
                    }

                    if (properties.Any())
                    {
                        return new ObjectEnterspeedProperty(name, properties);
                    }
                }
            }

            return null;
        }
    }
}
