using Enterspeed.Source.Sdk.Api.Models.Properties;
using System.Collections.Generic;
using System.Linq;

namespace Enterspeed.Source.UmbracoCms.Shared.EnterspeedPropertyValidation
{
    public class EnterspeedPropertyValueValidator : IEnterspeedPropertyValueValidator
    {
        private bool Valid(IEnterspeedProperty property)
        {
            var propertyValue = (IEnterspeedPropertyValue)property;
            return propertyValue?.Value != null;
        }

        /// <summary>
        /// Call to validate Enterspeed property
        /// </summary>
        /// <param name="property"></param>
        /// <returns>A single property validation response</returns>
        public IEnterspeedValidationResponse Validate(IEnterspeedProperty property)
        {
            var isValid = Valid(property);

            var validationMessage = isValid ? "valid" : $"It is not allowed to a assign null value to {property.GetType()}. Please make sur that {property.Name} has a value";
            return new PropertyValidationResponse(validationMessage, isValid);
        }

        /// <summary>
        /// Call to validate Enterspeed properties
        /// </summary>
        /// <param name="enterspeedProperty"></param>
        /// <returns>A list of property validation responses</returns>
        public List<IEnterspeedValidationResponse> Validate(IEnumerable<IEnterspeedProperty> enterspeedProperty)
        {
            return enterspeedProperty.Select(Validate).ToList();
        }
    }
}