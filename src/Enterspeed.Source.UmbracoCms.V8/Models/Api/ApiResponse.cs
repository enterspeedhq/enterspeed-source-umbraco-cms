namespace Enterspeed.Source.UmbracoCms.V8.Models.Api
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }
    }
}
