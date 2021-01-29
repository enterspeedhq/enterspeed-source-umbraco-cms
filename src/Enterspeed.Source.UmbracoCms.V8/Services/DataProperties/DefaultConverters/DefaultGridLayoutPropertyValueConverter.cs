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
