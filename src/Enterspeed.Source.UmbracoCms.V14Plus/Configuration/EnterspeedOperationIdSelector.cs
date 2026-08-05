using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Configuration;

public class EnterspeedOperationIdSelector : OperationIdSelector
{
    public EnterspeedOperationIdSelector(IEnumerable<IOperationIdHandler> operationIdHandlers) : base(operationIdHandlers)
    {
    }

    public override string OperationId(ApiDescription apiDescription)
    {
        if (apiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
        {
            if (actionDescriptor.ControllerTypeInfo.Namespace != null && actionDescriptor.ControllerTypeInfo.Namespace.ToLowerInvariant().Contains("enterspeed"))
            {
                // Match the operation id format Umbraco 18's UmbracoOperationIdTransformer
                // produces (verb prefix + action name, e.g. PostClearPendingJobs), so the
                // 17 and 18 documents generate identical clients
                var httpMethod = apiDescription.HttpMethod ?? "GET";
                var verb = char.ToUpperInvariant(httpMethod[0]) + httpMethod[1..].ToLowerInvariant();
                return $"{verb}{apiDescription.ActionDescriptor.RouteValues["action"]}";
            }
        }

        return base.OperationId(apiDescription);
    }
}
