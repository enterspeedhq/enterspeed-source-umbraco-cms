using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.Base.Services.DataProperties.DefaultConverters
{
    public class DefaultImageCropperPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.ImageCropper");
        }

        public virtual IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var value = property.GetValue<ImageCropperValue>(culture);
            Dictionary<string, IEnterspeedProperty> properties = null;
            if (value != null)
            {
                properties = new Dictionary<string, IEnterspeedProperty>();
                // Crops
                var crops = GetCropsProperty(value);
                properties.Add("crops", crops.Any() ? new ArrayEnterspeedProperty("crops", crops.ToArray()) : null);

                // Src
                properties.Add("src", new StringEnterspeedProperty(value.Src));

                // FocalPoint
                properties.Add("focalPoint", GetFocalPoint(value));
            }

            return new ObjectEnterspeedProperty(property.Alias, properties);
        }

        private static List<IEnterspeedProperty> GetCropsProperty(ImageCropperValue value)
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

                    ObjectEnterspeedProperty cropCoordinatesProperty = null;
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

        private static ObjectEnterspeedProperty GetFocalPoint(ImageCropperValue value)
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
