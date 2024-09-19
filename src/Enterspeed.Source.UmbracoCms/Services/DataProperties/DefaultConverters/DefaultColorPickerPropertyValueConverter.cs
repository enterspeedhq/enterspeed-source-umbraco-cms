using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultColorPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IEnterspeedConfigurationEditorProvider _enterspeedConfigurationEditorProvider;

        public DefaultColorPickerPropertyValueConverter(IEnterspeedConfigurationEditorProvider enterspeedConfigurationEditorProvider)
        {
            _enterspeedConfigurationEditorProvider = enterspeedConfigurationEditorProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.ColorPicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var colorValue = string.Empty;
            var colorLabel = string.Empty;

            if (_enterspeedConfigurationEditorProvider.UseColorPickerLabel(property.PropertyType))
            {
                var colorPickerValue = property.GetValue<ColorPickerValueConverter.PickedColor>(culture);
                if (colorPickerValue is not null)
                {
                    colorValue = colorPickerValue.Color;
                    colorLabel = colorPickerValue.Label;
                }
            }
            else
            {
                colorValue = property.GetValue<string>(culture);
                colorLabel = colorValue;
            }

            var colorPickerProperties = new List<IEnterspeedProperty>
            {
                new StringEnterspeedProperty("color", colorValue),
                new StringEnterspeedProperty("label", colorLabel)
            };

            var properties = colorPickerProperties.ToDictionary(x => x.Name, x => x);
            return new ObjectEnterspeedProperty(property.Alias, properties);
        }
    }
}