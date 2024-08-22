using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Models.Grid;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultGridConverters
{
    public class DefaultRichTextEditorGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        private readonly IUmbracoRichTextParser _umbracoRichTextParser;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public DefaultRichTextEditorGridEditorValueConverter(IUmbracoRichTextParser umbracoRichTextParser, IEnterspeedConfigurationService enterspeedConfigurationService)
        {
            _umbracoRichTextParser = umbracoRichTextParser;
            _enterspeedConfigurationService = enterspeedConfigurationService;
        }

        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("rte");
        }

        public virtual IEnterspeedProperty Convert(GridControl editor, string culture)
        {
            var parsedHtmlString = _umbracoRichTextParser.ParseInternalLink(editor.Value.ToString());
            parsedHtmlString = _umbracoRichTextParser.PrefixRelativeImagesWithDomain(parsedHtmlString, _enterspeedConfigurationService.GetConfiguration().MediaDomain);

            return new StringEnterspeedProperty(parsedHtmlString);
        }
    }
}