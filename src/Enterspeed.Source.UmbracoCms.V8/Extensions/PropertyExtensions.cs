using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Extensions
{
    public static class PropertyExtensions
    {
        public static T GetValue<T>(this IPublishedProperty property, string culture)
        {
            if (!property.PropertyType.VariesByCulture())
            {
                return (T)property.GetValue();
            }

            return (T)property.GetValue(culture);
        }
    }
}
