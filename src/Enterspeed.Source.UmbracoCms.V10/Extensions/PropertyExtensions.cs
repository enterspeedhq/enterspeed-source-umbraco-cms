using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V10.Extensions
{
    public static class PropertyExtensions
    {
        public static T GetValue<T>(this IPublishedProperty property, string culture)
        {
            if (!property.PropertyType.VariesByCulture())
            {
                culture = null;
            }

            return (T)property.GetValue(culture);
        }
    }
}
