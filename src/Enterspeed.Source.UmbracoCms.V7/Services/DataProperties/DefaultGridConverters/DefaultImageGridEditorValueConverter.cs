using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Models.Grid;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Newtonsoft.Json.Linq;
using Umbraco.Core;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultGridConverters
{
    public class DefaultImageGridEditorValueConverter : IEnterspeedGridEditorValueConverter
    {
        private readonly UmbracoMediaUrlProvider _umbracoMediaUrlProvider;

        public DefaultImageGridEditorValueConverter()
        {
            _umbracoMediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(string alias)
        {
            return alias.InvariantEquals("media");
        }

        public IEnterspeedProperty Convert(GridControl editor)
        {
            Dictionary<string, IEnterspeedProperty> properties = null;
            if (editor.Value != null && editor.Value.HasValues)
            {
                properties = new Dictionary<string, IEnterspeedProperty>();

                // Focalpoint
                var focalPoint = GetFocalPoint(editor.Value);
                if (focalPoint != null)
                {
                    properties.Add("focalPoint", focalPoint);
                }

                // ImageUrl
                var imageUrl = _umbracoMediaUrlProvider.GetUrl(editor.Value.Value<string>("image"));
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    properties.Add("image", new StringEnterspeedProperty(imageUrl));
                }

                // AltText
                var altText = editor.Value.Value<string>("altText");
                if (!string.IsNullOrWhiteSpace(altText))
                {
                    properties.Add("altText", new StringEnterspeedProperty(altText));
                }

                // Caption
                var caption = editor.Value.Value<string>("caption");
                if (!string.IsNullOrWhiteSpace(caption))
                {
                    properties.Add("caption", new StringEnterspeedProperty(caption));
                }
            }

            return new ObjectEnterspeedProperty(properties);
        }

        private ObjectEnterspeedProperty GetFocalPoint(JToken value)
        {
            if (value == null || !value.HasValues)
            {
                return null;
            }

            var properties = new Dictionary<string, IEnterspeedProperty>();

            var focalPointProp = value.Value<JToken>("focalPoint");
            if (focalPointProp == null)
            {
                return null;
            }

            var leftProp = focalPointProp.Value<double?>("left");
            var topProp = focalPointProp.Value<double?>("top");

            if (!leftProp.HasValue || !topProp.HasValue)
            {
                return null;
            }

            properties.Add("left", new NumberEnterspeedProperty(leftProp.Value));
            properties.Add("top", new NumberEnterspeedProperty(topProp.Value));

            return new ObjectEnterspeedProperty(properties);
        }
    }
}
