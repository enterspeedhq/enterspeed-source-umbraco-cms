using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultConverters
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
                    { "id", new NumberEnterspeedProperty(user.Id) },
                    { "name", new StringEnterspeedProperty(user.Name) },
                };
            }

            return new ObjectEnterspeedProperty(property.Alias, properties);
        }
    }
}
