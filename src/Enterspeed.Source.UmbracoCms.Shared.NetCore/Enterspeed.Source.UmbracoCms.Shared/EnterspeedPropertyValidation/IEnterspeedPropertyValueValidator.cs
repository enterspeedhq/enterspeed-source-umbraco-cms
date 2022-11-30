using Enterspeed.Source.Sdk.Api.Models.Properties;
using System.Collections.Generic;

namespace Enterspeed.Source.UmbracoCms.Shared.EnterspeedPropertyValidation
{
    public interface IEnterspeedPropertyValueValidator
    {
        /// <summary>
        /// Call to validate Enterspeed property
        /// </summary>
        /// <param name="property"></param>
        /// <returns>A single property validation response</returns>
        IEnterspeedValidationResponse Validate(IEnterspeedProperty property);

        /// <summary>
        /// Call to validate Enterspeed properties
        /// </summary>
        /// <param name="enterspeedProperty"></param>
        /// <returns>A list of property validation responses</returns>
        List<IEnterspeedValidationResponse> Validate(IEnumerable<IEnterspeedProperty> enterspeedProperty);
    }
}