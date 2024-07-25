﻿using Enterspeed.Source.UmbracoCms.V14.Configuration;
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
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [MapToApi("enterspeed")]
    [EnterspeedVersionedRoute("")]
    public class EnterspeedBaseController : ControllerBase
    {
    }
}