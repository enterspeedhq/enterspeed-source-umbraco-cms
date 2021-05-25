using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultMemberPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IMemberService _memberService;

        public DefaultMemberPickerPropertyValueConverter()
        {
            _memberService = UmbracoContextHelper.GetUmbracoContext().Application.Services.MemberService;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.MemberPickerAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<int?>();
            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value.HasValue)
            {
                var member = _memberService.GetById(value.Value);
                if (member != null)
                {
                    properties = new Dictionary<string, IEnterspeedProperty>
                    {
                        { "id", new NumberEnterspeedProperty(member.Id) },
                        { "name", new StringEnterspeedProperty(member.Name) },
                        { "memberType", new StringEnterspeedProperty(member.ContentType.Alias) }
                    };
                }
            }

            return new ObjectEnterspeedProperty(property.PropertyTypeAlias, properties);
        }
    }
}
