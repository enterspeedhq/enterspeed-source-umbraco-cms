﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Asp.Versioning;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Models.Api;
using Enterspeed.Source.UmbracoCms.Models.Configuration;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.V14.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;


namespace Enterspeed.Source.UmbracoCms14.Controllers.Api
{
    [ApiController]
    [MapToApi("Enterspeed")]
    [BackOfficeRoute("enterspeed/api/dashboard/")]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    public class DashboardController : ControllerBase
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

        [HttpGet()]
        public ApiResponse<List<EnterspeedJob>> GetFailedJobs()
        {
            var result = _enterspeedJobRepository.GetFailedJobs().ToList();
            return new ApiResponse<List<EnterspeedJob>>
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet("seed")]
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
        public IActionResult CustomSeed([FromBody] CustomSeedModel customSeed)
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

            //var response = _enterspeedJobService.CustomSeed(publishConfigured, previewConfigured, customSeed);
            return Ok(
                new ApiResponse<SeedResponse>
                {
                    //Data = response,
                    IsSuccess = true
                });
        }

        [HttpGet("GetEnterspeedConfiguration")]
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

        [HttpPost]
        public IActionResult SaveConfiguration(EnterspeedUmbracoConfiguration configuration)
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
        public IActionResult ClearPendingJobs()
        {
            _enterspeedJobRepository.ClearPendingJobs();

            return Ok(
                new ApiResponse
                {
                    IsSuccess = true
                });
        }

        [HttpPost]
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

        [HttpPost]
        public ActionResult DeleteJobs(JobIdsToDelete jobIdsToDelete)
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

        [HttpPost]
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