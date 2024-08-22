using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public class EnterspeedValidationService : IEnterspeedValidationService
    {
        private readonly ILogger<EnterspeedValidationService> _logger;

        public EnterspeedValidationService(ILogger<EnterspeedValidationService> logger)
        {
            _logger = logger;
        }

        public void LogValidationError(KeyValuePair<string, IEnterspeedProperty> property)
        {
            if (property.Value == null)
            {
                _logger.LogError($"You cannot assign null as a property on {property.Key}");
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
