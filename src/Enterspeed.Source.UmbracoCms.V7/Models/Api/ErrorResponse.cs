using System.Collections.Generic;
using Enterspeed.Source.Sdk.Domain.Connection;

namespace Enterspeed.Source.UmbracoCms.V7.Models.Api
{
    public class ErrorResponse
    {
        public int StatusCode { get; }
        public string Message { get; }
        public Dictionary<string, string> Exception { get; }
        public Dictionary<string, string> InnerException { get; }
        public string StackTrace { get; }
        public Dictionary<string, string> Errors { get; }
        public string ErrorCode { get; }
        public string Content { get; }

        public ErrorResponse(Response response)
        {
            StatusCode = response.StatusCode;
            Message = response.Message;
            StackTrace = response.Exception?.StackTrace;
            Errors = response.Errors;
            ErrorCode = response.ErrorCode;
            Content = response.Content;
            Exception = new Dictionary<string, string>
            {
                { "Type", response.Exception?.GetType().ToString() },
                { "Message", response.Exception?.Message },
                { "StackTrace", response.Exception?.StackTrace }
            };
            InnerException = new Dictionary<string, string>
            {
                { "Type", response.Exception?.InnerException?.GetType().ToString() },
                { "Message", response.Exception?.InnerException?.Message },
                { "StackTrace", response.Exception?.InnerException?.StackTrace }
            };
        }
    }
}