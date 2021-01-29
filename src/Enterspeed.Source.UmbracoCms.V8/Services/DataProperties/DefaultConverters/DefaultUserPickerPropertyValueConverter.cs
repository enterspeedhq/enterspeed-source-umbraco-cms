using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters
{
    public class DefaultUserPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IUserService _userService;

        public DefaultUserPickerPropertyValueConverter(IUserService userService)
        {
            _userService = userService;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.UserPicker");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<int?>(culture);

            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                var user = _userService.GetUserById(value.Value);
                properties = new Dictionary<string, IEnterspeedProperty>
                {
                    { "Id", new NumberEnterspeedProperty(user.Id) },
                    { "Name", new StringEnterspeedProperty(user.Name) },
                };
            }

            return new ObjectEnterspeedProperty(property.Alias, properties);
        }
    }
}
