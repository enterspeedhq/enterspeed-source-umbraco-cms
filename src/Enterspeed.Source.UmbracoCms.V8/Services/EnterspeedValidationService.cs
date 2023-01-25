using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Core.Logging;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedValidationService : IEnterspeedValidationService
    {
        private readonly ILogger _logger;

        public EnterspeedValidationService(ILogger logger)
        {
            _logger = logger;
        }

        public void LogValidationError(KeyValuePair<string, IEnterspeedProperty> property)
        {
            if (property.Value == null)
            {
                _logger.Error<EnterspeedValidationService>($"You cannot assign null as a property on {property.Key}");
            }
        }

        public void LogValidationErrors(IDictionary<string, IEnterspeedProperty> enterspeedProperties)
        {
            foreach (var property in enterspeedProperties)
            {
                LogValidationError(property);
            }
        }
    }
}