namespace Enterspeed.Source.UmbracoCms.Shared.EnterspeedPropertyValidation
{
    public class PropertyValidationResponse : IEnterspeedValidationResponse
    {
        public PropertyValidationResponse(string validationError, bool valid)
        {
            ValidationMessage = validationError;
            Valid = valid;
        }

        public bool Valid { get; }
        public string ValidationMessage { get; }
    }

    public interface IEnterspeedValidationResponse
    {
        bool Valid { get; }
        string ValidationMessage { get; }
    }
}