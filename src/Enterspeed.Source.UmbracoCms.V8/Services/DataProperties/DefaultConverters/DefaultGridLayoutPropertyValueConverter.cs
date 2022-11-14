using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Enterspeed.Source.UmbracoCms.V8.Models.Grid;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultGridLayoutPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private IEnterspeedGridEditorService _enterspeedGridEditorService;

        private IEnterspeedGridEditorService EnterspeedGridEditorService =>
            _enterspeedGridEditorService ??
            (_enterspeedGridEditorService = Current.Factory.GetInstance<IEnterspeedGridEditorService>());

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.Grid");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<JObject>(culture);

            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                properties = new Dictionary<string, IEnterspeedProperty>();
                foreach (var prop in value)
                {
                    var enterspeedProperty = GetProperty(prop.Key, prop.Value, culture);
                    if (enterspeedProperty != null)
                    {
                        properties.Add(prop.Key, enterspeedProperty);
                    }
                }
            }

            return new ObjectEnterspeedProperty(property.Alias, properties);
        }

        private IEnterspeedProperty GetProperty(string name, JToken value, string culture)
        {
            var type = value.Type;
            if (type == JTokenType.String)
            {
                var stringValue = value.Value<string>();

                return !string.IsNullOrEmpty(name)
                    ? new StringEnterspeedProperty(name, stringValue)
                    : new StringEnterspeedProperty(stringValue);
            }

            if (type == JTokenType.Boolean)
            {
                var boolValue = value.Value<bool>();

                return !string.IsNullOrEmpty(name)
                    ? new BooleanEnterspeedProperty(name, boolValue)
                    : new BooleanEnterspeedProperty(boolValue);
            }

            if (type == JTokenType.Integer)
            {
                var numberValue = value.Value<int>();

                return !string.IsNullOrEmpty(name)
                    ? new NumberEnterspeedProperty(name, numberValue)
                    : new NumberEnterspeedProperty(numberValue);
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
                                property = EnterspeedGridEditorService.ConvertGridEditor(gridControl, culture);
                            }

                            if (property == null)
                            {
                                property = GetProperty(prop.Key, prop.Value, culture);
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

                    if (jToken is JValue itemValue)
                    {
                        var property = GetProperty(null, itemValue, culture);
                        if (property != null)
                        {
                            arrayItems.Add(property);
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
                        var property = GetProperty(prop.Key, prop.Value, culture);
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