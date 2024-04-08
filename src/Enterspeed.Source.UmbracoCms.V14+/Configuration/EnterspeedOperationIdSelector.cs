using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Enterspeed.Source.UmbracoCms.V14.Configuration;

public class EnterspeedOperationIdSelector : OperationIdSelector
{
    public override string OperationId(ApiDescription apiDescription, ApiVersioningOptions apiVersioningOptions)
    {
        if (apiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
        {
            if (actionDescriptor.ControllerTypeInfo.Namespace != null && actionDescriptor.ControllerTypeInfo.Namespace.ToLowerInvariant().Contains("enterspeed"))
            {
                return $"{apiDescription.ActionDescriptor.RouteValues["action"]}";
            }
        }

        return base.OperationId(apiDescription, apiVersioningOptions);
    }
}