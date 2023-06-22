using System.Diagnostics.CodeAnalysis;

namespace Enterspeed.Source.UmbracoCms.V8.Models.Api
{
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
    }
}