using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Models.Grid;
using Umbraco.Core;

namespace Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultGridConverters
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

        public IEnterspeedProperty Convert(GridControl editor, string culture)
        {
            var parsedHtmlString = _umbracoRichTextParser.ParseInternalLink(editor.Value.ToString());
            parsedHtmlString = _umbracoRichTextParser.PrefixRelativeImagesWithDomain(parsedHtmlString, _enterspeedConfigurationService.GetConfiguration().MediaDomain);

            return new StringEnterspeedProperty(parsedHtmlString);
        }
    }
}
