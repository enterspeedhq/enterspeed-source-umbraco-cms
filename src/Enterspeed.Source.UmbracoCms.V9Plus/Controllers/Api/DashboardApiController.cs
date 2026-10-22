using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;

namespace Enterspeed.Source.UmbracoCms.V9Plus.Controllers.Api
{
    [JsonCamelCaseFormatter]
    public class DashboardApiController : UmbracoAuthorizedApiController
    {
        internal const string PublishApiKeyError = "publishApiKey";
        internal const string PreviewApiKeyError = "previewApiKey";

        private const string PublishKeyName = "Publish";
        private const string PreviewKeyName = "Preview";

        private readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly IEnterspeedJobsHandlingService _enterspeedJobsHandlingService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobService _enterspeedJobService;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly IEnterspeedConnection _enterspeedConnection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardApiController(
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

        [HttpGet]
        public ApiResponse<List<EnterspeedJob>> GetFailedJobs()
        {
            var result = _enterspeedJobRepository.GetFailedJobs().ToList();
            return new ApiResponse<List<EnterspeedJob>>
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
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

        [HttpPost]
        public IActionResult CustomSeed(CustomSeed customSeed)
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

            var response = _enterspeedJobService.CustomSeed(publishConfigured, previewConfigured, customSeed);
            return Ok(
                new ApiResponse<SeedResponse>
                {
                    Data = response,
                    IsSuccess = true
                });
        }

        [HttpGet]
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

            var validation = ValidateConfiguration(configuration);

            // The publish API key is required, so nothing is saved when it is invalid. An invalid preview API key only
            // disables preview, so the configuration is still saved and the failure is reported as a warning.
            if (validation.Errors != null && validation.Errors.ContainsKey(PublishApiKeyError))
            {
                return Ok(validation);
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
                    Success = true,
                    Message = validation.Message,
                    Errors = validation.Errors
                });
        }

        [HttpGet]
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

        [HttpPost]
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
            return Ok(ValidateConfiguration(configuration));
        }

        // Both keys are always checked, so a single request reports the state of both, per key in Response.Errors.
        private Response ValidateConfiguration(EnterspeedUmbracoConfiguration configuration)
        {
            var errors = new Dictionary<string, string>();
            Response failedResponse = null;
            var publishRejected = false;
            var previewRejected = false;

            var publishKeyMissing = DescribeMissingKey(configuration.ApiKey, PublishKeyName);
            if (publishKeyMissing != null)
            {
                errors.Add(PublishApiKeyError, publishKeyMissing);
            }
            else
            {
                var publishResponse = TestConnection(configuration.GetPublishConfiguration());
                if (!publishResponse.Success)
                {
                    errors.Add(PublishApiKeyError, DescribeFailure(publishResponse, PublishKeyName));
                    publishRejected = publishResponse.StatusCode == 401;
                    failedResponse = publishResponse;
                }
            }

            // The preview API key is optional, so it is only verified when one has been entered.
            var previewConfigured = !string.IsNullOrWhiteSpace(configuration.PreviewApiKey);
            if (previewConfigured)
            {
                var previewResponse = TestConnection(configuration.GetPreviewConfiguration());
                if (!previewResponse.Success)
                {
                    errors.Add(PreviewApiKeyError, DescribeFailure(previewResponse, PreviewKeyName));
                    previewRejected = previewResponse.StatusCode == 401;
                    failedResponse = failedResponse ?? previewResponse;
                }
            }

            if (errors.Count == 0)
            {
                return new Response
                {
                    Status = HttpStatusCode.OK,
                    Success = true,
                    Message = previewConfigured
                        ? "Publish and preview API keys are valid"
                        : "Publish API key is valid"
                };
            }

            // Both keys rejected for the same reason is said once, rather than repeating the same sentence per key.
            var bothRejected = publishRejected && previewRejected;
            var statements = bothRejected
                ? "Publish and preview API keys were rejected by Enterspeed"
                : string.Join(". ", errors.Values);

            return new Response
            {
                Status = failedResponse?.Status ?? HttpStatusCode.BadRequest,
                Success = false,
                Message = $"{statements}. {DescribeAdvice(errors.Count)}",
                Errors = errors,
                Exception = failedResponse?.Exception
            };
        }

        private static string DescribeAdvice(int failureCount) => failureCount > 1
            ? "The keys are either incorrect or not valid API keys - please verify them in the Enterspeed app."
            : "The key is either incorrect or not a valid API key - please verify it in the Enterspeed app.";

        // Only checks presence. Validating the key format is left to Enterspeed, so a format change there does not
        // need a release of this package.
        private static string DescribeMissingKey(string apiKey, string keyName) =>
            string.IsNullOrWhiteSpace(apiKey) ? $"{keyName} API key is required" : null;

        // Prefers the message Enterspeed returned. It sends no body for an unauthorized request today, hence the
        // fallbacks, which cannot tell an unrecognised key from a malformed one.
        private static string DescribeFailure(Response response, string keyName)
        {
            if (!string.IsNullOrWhiteSpace(response.Message))
            {
                return $"{keyName} API key: {response.Message}";
            }

            return response.StatusCode == 401
                ? $"{keyName} API key was rejected by Enterspeed"
                : $"{keyName} API key could not be verified (HTTP {response.StatusCode})";
        }

        private Response TestConnection(EnterspeedConfiguration configuration)
        {
            var testConfigurationService = new InMemoryEnterspeedUmbracoConfigurationProvider(configuration);
            var testConnection = new EnterspeedConnection(testConfigurationService);
            var enterspeedIngestService = new EnterspeedIngestService(
                testConnection, new SystemTextJsonSerializer());

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