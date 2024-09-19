using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultMemberGroupPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IMemberGroupService _memberGroupService;

        public DefaultMemberGroupPickerPropertyValueConverter(IMemberGroupService memberGroupService)
        {
            _memberGroupService = memberGroupService;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.MemberGroupPicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<string>(culture);
            var arrayItems = new List<IEnterspeedProperty>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var memberGroupIds = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var memberGroups = _memberGroupService.GetByIds(memberGroupIds.Select(int.Parse)).ToList();
                foreach (var memberGroup in memberGroups)
                {
                    var memberGroupObject = ConvertToEnterspeedProperty(memberGroup);
                    if (memberGroupObject != null)
                    {
                        arrayItems.Add(memberGroupObject);
                    }
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
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
