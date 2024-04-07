using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Authorization;

namespace Enterspeed.Source.UmbracoCms.V14.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi("enterspeed")]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    public class EnterspeedBaseController : ControllerBase
    {
    }
}
