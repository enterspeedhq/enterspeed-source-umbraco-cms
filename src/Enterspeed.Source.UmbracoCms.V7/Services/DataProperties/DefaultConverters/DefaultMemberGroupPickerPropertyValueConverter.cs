using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultMemberGroupPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IMemberGroupService _memberGroupService;

        public DefaultMemberGroupPickerPropertyValueConverter()
        {
            _memberGroupService = UmbracoContextHelper.GetUmbracoContext().Application.Services.MemberGroupService;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.MemberGroupPickerAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<string>();
            var arrayItems = new List<IEnterspeedProperty>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var memberGroupNames = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var memberGroups = memberGroupNames.Select(x => _memberGroupService.GetByName(x)).ToList();

                foreach (var memberGroup in memberGroups)
                {
                    var memberGroupObject = ConvertToEnterspeedProperty(memberGroup);
                    if (memberGroupObject != null)
                    {
                        arrayItems.Add(memberGroupObject);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.PropertyTypeAlias, arrayItems.ToArray());
        }

        private ObjectEnterspeedProperty ConvertToEnterspeedProperty(IMemberGroup memberGroup)
        {
            if (memberGroup == null)
            {
                return null;
            }

            var properties = new Dictionary<string, IEnterspeedProperty>();
            properties.Add("id", new NumberEnterspeedProperty(memberGroup.Id));
            properties.Add("name", new StringEnterspeedProperty(memberGroup.Name));

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
