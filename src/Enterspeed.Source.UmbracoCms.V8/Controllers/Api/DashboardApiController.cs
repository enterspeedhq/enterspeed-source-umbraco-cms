using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Enterspeed.Source.UmbracoCms.V8.Models.Api;
using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Scoping;
using Umbraco.Web.WebApi;

namespace Enterspeed.Source.UmbracoCms.V8.Controllers.Api
{
    [JsonCamelCaseFormatter]
    public class DashboardApiController : UmbracoAuthorizedApiController
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobService _enterspeedJobService;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly IEnterspeedConnection _enterspeedConnection;
        private readonly IRuntimeState _runtimeState;
        private readonly IScopeProvider _scopeProvider;
        private readonly IEnterspeedJobsHandlingService _enterspeedJobsHandlingService;

        public DashboardApiController(
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobService enterspeedJobService,
            IEnterspeedConfigurationService enterspeedConfigurationService,
            IEnterspeedConnection enterspeedConnection,
            IRuntimeState runtimeState,
            IScopeProvider scopeProvider,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobService = enterspeedJobService;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _enterspeedConnection = enterspeedConnection;
            _runtimeState = runtimeState;
            _scopeProvider = scopeProvider;
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
        public HttpResponseMessage Seed()
        {
            var publishConfigured = _enterspeedConfigurationService.IsPublishConfigured();
            var previewConfigured = _enterspeedConfigurationService.IsPreviewConfigured();
            if (!publishConfigured && !previewConfigured)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Response
                {
                    Status = HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Enterspeed has not yet been configured"
                });
            }

            var response = _enterspeedJobService.Seed(publishConfigured, previewConfigured);

            return Request.CreateResponse(HttpStatusCode.OK, new ApiResponse<SeedResponse>
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
                Data = new EnterspeedUmbracoConfigurationResponse(config, _runtimeState.ServerRole, runJobsOnServer),
                IsSuccess = true
            };
        }

        [HttpPost]
        public HttpResponseMessage SaveConfiguration(EnterspeedUmbracoConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration?.ApiKey) || string.IsNullOrEmpty(configuration?.BaseUrl))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Response
                {
                    Status = HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Apikey or url is empty"
                });
            }

            if (string.IsNullOrWhiteSpace(configuration.MediaDomain))
            {
                configuration.MediaDomain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            }

            var response = TestConnections(configuration);

            if (!response.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            try
            {
                _enterspeedConfigurationService.Save(configuration);
            }
            catch (ConfigurationErrorsException exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Response
                {
                    Status = HttpStatusCode.BadRequest,
                    Success = false,
                    Message = exception.Message,
                    Exception = exception
                });
            }

            _enterspeedConnection.Flush();

            return Request.CreateResponse(HttpStatusCode.OK, new Response
            {
                Status = HttpStatusCode.OK,
                Success = true
            });
        }

        [HttpPost]
        public HttpResponseMessage TestConfigurationConnection(EnterspeedUmbracoConfiguration configuration)
        {
            return Request.CreateResponse(HttpStatusCode.OK, TestConnections(configuration));
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
            var enterspeedIngestService = new EnterspeedIngestService(testConnection, new SystemTextJsonSerializer(), testConfigurationService);

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

        [HttpPost]
        public ApiResponse DeleteFailedJobs()
        {
            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                var failedJobs = _enterspeedJobRepository.GetFailedJobs();
                if (failedJobs != null && failedJobs.Any())
                {
                    _enterspeedJobRepository.Delete(failedJobs.Select(fj => fj.Id).ToList());
                }
            }

            return new ApiResponse()
            {
                IsSuccess = true
            };
        }

        [HttpPost]
        public ApiResponse DeleteJobs(JobIdsToDelete jobIdsToDelete)
        {
            using (_scopeProvider.CreateScope(autoComplete: true))
            {
                if (jobIdsToDelete != null && jobIdsToDelete.Ids.Any())
                {
                    _enterspeedJobRepository.Delete(jobIdsToDelete.Ids);
                }
            }

            return new ApiResponse()
            {
                IsSuccess = true
            };
        }
    }
}