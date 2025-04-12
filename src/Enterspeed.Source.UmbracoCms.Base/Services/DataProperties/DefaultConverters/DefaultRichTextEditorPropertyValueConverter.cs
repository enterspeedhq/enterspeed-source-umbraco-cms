using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultRichTextEditorPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUmbracoContextFactory _umbracoContextAccessor;
        private readonly IUmbracoRichTextParser _umbracoRichTextParser;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly IEnterspeedHttpContextProvider _enterspeedHttpContextProvider;

        public DefaultRichTextEditorPropertyValueConverter(
           IHttpContextAccessor httpContextAccessor,
           IUmbracoContextFactory umbracoContextAccessor,
           IUmbracoRichTextParser umbracoRichTextParser,
           IEnterspeedConfigurationService enterspeedConfigurationService,
           IEnterspeedHttpContextProvider enterspeedHttpContextProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
            _umbracoRichTextParser = umbracoRichTextParser;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _enterspeedHttpContextProvider = enterspeedHttpContextProvider;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.TinyMCE") || propertyType.EditorAlias.Equals("Umbraco.RichText");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                using (_enterspeedHttpContextProvider.CreateFakeHttpContext())
                {
                    using (_umbracoContextAccessor.EnsureUmbracoContext())
                    {
                        return ConvertWithContext(property, culture);
                    }
                }
            }

            return ConvertWithContext(property, culture);
        }

        private IEnterspeedProperty ConvertWithContext(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<Umbraco.Cms.Core.Strings.HtmlEncodedString>(culture).ToString();
            value = _umbracoRichTextParser.PrefixRelativeImagesWithDomain(value, _enterspeedConfigurationService.GetConfiguration().MediaDomain);
            return new StringEnterspeedProperty(property.Alias, value);
        }
    }
}
