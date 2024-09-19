namespace Enterspeed.Source.UmbracoCms.Base.Models.Api
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
}
