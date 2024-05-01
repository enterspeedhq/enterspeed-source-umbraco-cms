using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Enterspeed.Source.UmbracoCms.V14.Configuration;

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
                return $"{apiDescription.ActionDescriptor.RouteValues["action"]}";
            }
        }

        return base.OperationId(apiDescription);
    }
}