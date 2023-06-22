using System.Diagnostics.CodeAnalysis;

namespace Enterspeed.Source.UmbracoCms.V7.Models.Api
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
}
