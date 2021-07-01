using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters
{
    public class DefaultImageCropperPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        private readonly UmbracoMediaUrlProvider _mediaUrlProvider;

        public DefaultImageCropperPropertyValueConverter()
        {
            _mediaUrlProvider = EnterspeedContext.Current.Providers.MediaUrlProvider;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.ImageCropperAlias);
        }

        public IEnterspeedProperty Convert(IPublishedProperty property)
        {
            var value = property.GetValue<ImageCropDataSet>();
            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                properties = new Dictionary<string, IEnterspeedProperty>();
                // Crops
                var crops = GetCropsProperty(value);
                properties.Add("crops", crops.Any() ? new ArrayEnterspeedProperty("crops", crops.ToArray()) : null);

                // Src
                var src = _mediaUrlProvider.GetUrl(value.Src);
                properties.Add("src", new StringEnterspeedProperty(src));

                // FocalPoint
                properties.Add("focalPoint", GetFocalPoint(value));
            }

            return new ObjectEnterspeedProperty(property.PropertyTypeAlias, properties);
        }

        private static List<IEnterspeedProperty> GetCropsProperty(ImageCropDataSet value)
        {
            var crops = new List<IEnterspeedProperty>();
            if (value != null && value.Crops != null)
            {
                foreach (var crop in value.Crops)
                {
                    var cropProperties = new Dictionary<string, IEnterspeedProperty>
                    {
                        { "alias", new StringEnterspeedProperty(crop.Alias) },
                        { "height", new NumberEnterspeedProperty(crop.Height) },
                        { "width", new NumberEnterspeedProperty(crop.Width) }
                    };

                    var cropCoordinatesProperty = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>());
                    if (crop.Coordinates != null)
                    {
                        var cropCoordinatesProperties = new Dictionary<string, IEnterspeedProperty>
                        {
                            {
                                "X1", new NumberEnterspeedProperty(double.Parse(crop.Coordinates.X1.ToString(CultureInfo.InvariantCulture)))
                            },
                            {
                                "Y1", new NumberEnterspeedProperty(double.Parse(crop.Coordinates.Y1.ToString(CultureInfo.InvariantCulture)))
                            },
                            {
                                "X2", new NumberEnterspeedProperty(double.Parse(crop.Coordinates.X2.ToString(CultureInfo.InvariantCulture)))
                            },
                            {
                                "Y2", new NumberEnterspeedProperty(double.Parse(crop.Coordinates.Y2.ToString(CultureInfo.InvariantCulture)))
                            }
                        };
                        cropCoordinatesProperty = new ObjectEnterspeedProperty(cropCoordinatesProperties);
                    }

                    cropProperties.Add("coordinates", cropCoordinatesProperty);
                    crops.Add(new ObjectEnterspeedProperty(cropProperties));
                }
            }

            return crops;
        }

        private static ObjectEnterspeedProperty GetFocalPoint(ImageCropDataSet value)
        {
            if (value == null || value.FocalPoint == null)
            {
                return null;
            }

            var focalPointProperties = new Dictionary<string, IEnterspeedProperty>
            {
                {
                    "left", new NumberEnterspeedProperty(
                        double.Parse(value.FocalPoint.Left.ToString(CultureInfo.InvariantCulture)))
                },
                {
                    "top", new NumberEnterspeedProperty(
                        double.Parse(value.FocalPoint.Top.ToString(CultureInfo.InvariantCulture)))
                }
            };

            return new ObjectEnterspeedProperty(focalPointProperties);
        }
    }
}
