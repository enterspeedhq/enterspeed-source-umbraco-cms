using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultContentPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IContentService _contentService;

        public DefaultContentPickerPropertyValueConverter(
            IEntityIdentityService entityIdentityService,
            IContentService contentService)
        {
            _entityIdentityService = entityIdentityService;
            _contentService = contentService;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.ContentPicker");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<object>(culture);
            if (value == null)
            {
                // nothing selected
                return null;
            }

            string contentId;
            if (value is GuidUdi)
            {
                // selected draft content
                var content = _contentService.GetById((value as GuidUdi).Guid);
                if (content == null)
                {
                    return null;
                }
                contentId = _entityIdentityService.GetId(content.Id, culture);
            }
            else if (value is IPublishedContent)
            {
                // selected published content
                contentId = _entityIdentityService.GetId(value as IPublishedContent, culture);
            }
            else
            {
                return null;
            }
            return new StringEnterspeedProperty(property.Alias, contentId);
        }
    }
}
