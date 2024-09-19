using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public interface IEnterspeedValidationService
    {
        void LogValidationErrors(IDictionary<string, IEnterspeedProperty> properties);
    }
}