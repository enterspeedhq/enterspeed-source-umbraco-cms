using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Source.UmbracoCms.Shared.EnterspeedPropertyValidation
{
    public interface IEnterspeedPropertyValue : IEnterspeedProperty
    {
        object Value { get; }
    }
}
