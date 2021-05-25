using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultUserPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IUserService _userService;

        public DefaultUserPickerPropertyValueConverter()
        {
            _userService = UmbracoContextHelper.GetUmbracoContext().Application.Services.UserService;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.UserPickerAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<int?>();

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

            return new ObjectEnterspeedProperty(property.PropertyTypeAlias, properties);
        }
    }
}
