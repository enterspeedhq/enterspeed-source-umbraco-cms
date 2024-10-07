using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Asp.Versioning;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Models.Api;
using Enterspeed.Source.UmbracoCms.Base.Models.Configuration;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Enterspeed.Source.UmbracoCms.V14Plus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Sync;
using IEnterspeedJobService = Enterspeed.Source.UmbracoCms.V14Plus.Services.IEnterspeedJobService;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Dashboard")]
    public class DashboardController : EnterspeedBaseController
    {
        private readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly IEnterspeedJobsHandlingService _enterspeedJobsHandlingService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobService _enterspeedJobService;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly IEnterspeedConnection _enterspeedConnection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobService enterspeedJobService,
            IEnterspeedConfigurationService enterspeedConfigurationService,
            IEnterspeedConnection enterspeedConnection,
            IHttpContextAccessor httpContextAccessor,
            IServerRoleAccessor serverRoleAccessor,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobService = enterspeedJobService;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _enterspeedConnection = enterspeedConnection;
            _httpContextAccessor = httpContextAccessor;
            _serverRoleAccessor = serverRoleAccessor;
            _enterspeedJobsHandlingService = enterspeedJobsHandlingService;
        }

        [HttpGet("GetFailedJobs")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse<List<EnterspeedJob>>), 200)]
        public ApiResponse<List<EnterspeedJob>> GetFailedJobs()
        {
            var result = _enterspeedJobRepository.GetFailedJobs().ToList();
            return new ApiResponse<List<EnterspeedJob>>
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet("Seed")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse<SeedResponse>), 200)]
        public IActionResult Seed()
        {
            var publishConfigured = _enterspeedConfigurationService.IsPublishConfigured();
            var previewConfigured = _enterspeedConfigurationService.IsPreviewConfigured();

            if (!publishConfigured && !previewConfigured)
            {
                return BadRequest(
                    new Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Enterspeed has not yet been configured"
                    });
            }

            var response = _enterspeedJobService.Seed(publishConfigured, previewConfigured);
            return Ok(
                new ApiResponse<SeedResponse>
                {
                    Data = response,
                    IsSuccess = true
                });
        }

        [HttpPost("CustomSeed")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse<SeedResponse>), 200)]
        public async Task<IActionResult> CustomSeed([FromBody] CustomSeedModel customSeed)
        {
            var publishConfigured = _enterspeedConfigurationService.IsPublishConfigured();
            var previewConfigured = _enterspeedConfigurationService.IsPreviewConfigured();

            if (!publishConfigured && !previewConfigured)
            {
                return BadRequest(
                    new Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Enterspeed has not yet been configured"
                    });
            }

            var response = await _enterspeedJobService.CustomSeed(customSeed, publishConfigured, previewConfigured);

            return Ok(
                new ApiResponse<SeedResponse>
                {
                    Data = response,
                    IsSuccess = true
                });
        }

        [HttpGet("GetEnterspeedConfiguration")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse<EnterspeedUmbracoConfigurationResponse>), 200)]
        public ApiResponse<EnterspeedUmbracoConfigurationResponse> GetEnterspeedConfiguration()
        {
            var config = _enterspeedConfigurationService.GetConfiguration();
            var runJobsOnServer = _enterspeedJobsHandlingService.IsJobsProcessingEnabled();
            return new ApiResponse<EnterspeedUmbracoConfigurationResponse>
            {
                Data = new EnterspeedUmbracoConfigurationResponse(config, _serverRoleAccessor.CurrentServerRole, runJobsOnServer),
                IsSuccess = true
            };
        }

        [HttpPost("SaveConfiguration")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(Response), 200)]
        public IActionResult SaveConfiguration([FromBody] EnterspeedUmbracoConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration?.ApiKey) || string.IsNullOrEmpty(configuration?.BaseUrl))
            {
                return BadRequest(
                    new Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Apikey or url is empty"
                    });
            }

            if (string.IsNullOrWhiteSpace(configuration.MediaDomain))
            {
                configuration.MediaDomain =
                    new Uri(_httpContextAccessor.HttpContext!.Request.GetEncodedUrl())
                        .GetLeftPart(UriPartial.Authority);
            }

            var response = TestConnection(configuration);
            if (!response.Success)
            {
                return Ok(response);
            }

            try
            {
                _enterspeedConfigurationService.Save(configuration);
            }
            catch (ConfigurationException exception)
            {
                return BadRequest(
                    new Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Success = false,
                        Message = exception.Message,
                        Exception = exception
                    });
            }

            _enterspeedConnection.Flush();

            return Ok(
                new Response
                {
                    Status = HttpStatusCode.OK,
                    Success = true
                });
        }

        [HttpGet("GetNumberOfPendingJobs")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse<GetNumberOfPendingJobsResponse>), 200)]
        public IActionResult GetNumberOfPendingJobs()
        {
            int numberOfPendingJobs;
            try
            {
                numberOfPendingJobs = _enterspeedJobRepository.GetNumberOfPendingJobs();
            }
            catch (Exception exception)
            {
                return BadRequest(
                    new Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Success = false,
                        Message = exception.Message,
                        Exception = exception
                    });
            }

            return Ok(
                new ApiResponse<GetNumberOfPendingJobsResponse>
                {
                    Data = new GetNumberOfPendingJobsResponse { NumberOfPendingJobs = numberOfPendingJobs },
                    IsSuccess = true
                });
        }

        [HttpPost("ClearPendingJobs")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public IActionResult ClearPendingJobs()
        {
            _enterspeedJobRepository.ClearPendingJobs();

            return Ok(
                new ApiResponse
                {
                    IsSuccess = true
                });
        }

        [HttpPost("DeleteFailedJobs")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public ActionResult DeleteFailedJobs()
        {
            var failedJobs = _enterspeedJobRepository.GetFailedJobs();
            if (failedJobs != null && failedJobs.Any())
            {
                _enterspeedJobRepository.Delete(failedJobs.Select(fj => fj.Id).ToList());
            }

            return Ok(new ApiResponse()
            {
                IsSuccess = true
            });
        }

        [HttpPost("DeleteJobs")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public ActionResult DeleteJobs([FromBody] JobIdsToDelete jobIdsToDelete)
        {
            if (jobIdsToDelete != null && jobIdsToDelete.Ids.Any())
            {
                _enterspeedJobRepository.Delete(jobIdsToDelete.Ids);
            }

            return Ok(new ApiResponse()
            {
                IsSuccess = true
            });
        }

        [HttpPost("TestConfigurationConnection")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(Response), 200)]
        public IActionResult TestConfigurationConnection(EnterspeedUmbracoConfiguration configuration)
        {
            return Ok(TestConnections(configuration));
        }

        private Response TestConnections(EnterspeedUmbracoConfiguration configuration)
        {
            var publishConfiguration = configuration.GetPublishConfiguration();
            var previewConfiguration = configuration.GetPreviewConfiguration();

            var publishResponse = TestConnection(publishConfiguration);
            if (!publishResponse.Success || previewConfiguration == null)
            {
                if (!publishResponse.Success && publishResponse.StatusCode == 401)
                {
                    publishResponse.Message = "Publish API key is invalid";
                }

                return publishResponse;
            }

            var previewResponse = TestConnection(previewConfiguration);
            if (!previewResponse.Success && previewResponse.StatusCode == 401)
            {
                previewResponse.Message = "Preview API key is invalid";
            }

            return previewResponse;
        }

        private Response TestConnection(EnterspeedConfiguration configuration)
        {
            var testConfigurationService = new InMemoryEnterspeedUmbracoConfigurationProvider(configuration);
            var testConnection = new EnterspeedConnection(testConfigurationService);
            var enterspeedIngestService = new EnterspeedIngestService(
                testConnection, new SystemTextJsonSerializer(), testConfigurationService);

            var response = enterspeedIngestService.Test();

            if (response.StatusCode == 422)
            {
                return new Response
                {
                    Exception = response.Exception,
                    Message = response.Message,
                    Status = response.Status,
                    Success = true
                };
            }

            return response;
        }
    }
}