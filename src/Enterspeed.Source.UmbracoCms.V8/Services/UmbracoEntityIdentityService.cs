using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class UmbracoEntityIdentityService : IEntityIdentityService
    {
        public string GetId(IPublishedContent content, string culture)
        {
            if (content == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(culture) || !content.ContentType.VariesByCulture())
            {
                culture = GetDefaultCulture();
            }

            return GetId(content.Id, culture);
        }

        public string GetId(int contentId, string culture)
        {
            return $"{contentId}-{culture}";
        }

        private static string GetDefaultCulture()
        {
            var contextFactory = Current.Factory.GetInstance<IUmbracoContextFactory>();
            using (var context = contextFactory.EnsureUmbracoContext())
            {
                return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
            }
        }
    }
}
