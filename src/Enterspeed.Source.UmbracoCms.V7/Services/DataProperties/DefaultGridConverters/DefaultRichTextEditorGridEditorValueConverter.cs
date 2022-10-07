using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Models.Grid;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using HtmlAgilityPack;
using Umbraco.Core;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultGridConverters
{
    public class DefaultRichTextEditorGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultRichTextEditorGridEditorValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("rte");
        }

        public IEnterspeedProperty Convert(GridControl editor)
        {
            var value = PrefixRelativeImagesWithDomain(editor.Value.ToString());
            return new StringEnterspeedProperty(value);
        }

        private string PrefixRelativeImagesWithDomain(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html;
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var imageNodes = htmlDocument.DocumentNode.SelectNodes("//img");
            if (imageNodes == null || !imageNodes.Any())
            {
                return html;
            }

            foreach (var imageNode in imageNodes)
            {
                var src = imageNode.GetAttributeValue("src", string.Empty);
                src = _mediaUrlProvider.GetUrl(src);
                imageNode.SetAttributeValue("src", src);
            }

            return htmlDocument.DocumentNode.InnerHtml;
        }
    }
}
