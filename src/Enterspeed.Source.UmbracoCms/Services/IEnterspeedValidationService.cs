using Enterspeed.Source.Sdk.Api.Models.Properties;
using System.Collections.Generic;

namespace Enterspeed.Source.UmbracoCms.Services
{
    public interface IEnterspeedValidationService
    {
        void LogValidationErrors(IDictionary<string, IEnterspeedProperty> properties);
    }
}