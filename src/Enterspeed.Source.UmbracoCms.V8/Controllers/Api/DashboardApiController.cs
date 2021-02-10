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
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Models.Api;
using Enterspeed.Source.UmbracoCms.V8.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
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

        public DashboardApiController(
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobService enterspeedJobService,
            IEnterspeedConfigurationService enterspeedConfigurationService,
            IEnterspeedConnection enterspeedConnection)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobService = enterspeedJobService;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _enterspeedConnection = enterspeedConnection;
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
        public ApiResponse<SeedResponse> Seed()
        {
            var response = _enterspeedJobService.Seed();

            return new ApiResponse<SeedResponse>
            {
                Data = response,
                IsSuccess = true
            };
        }

        [HttpGet]
        public ApiResponse<EnterspeedConfiguration> GetEnterspeedConfiguration()
        {
            var config = _enterspeedConfigurationService.GetConfiguration();
            return new ApiResponse<EnterspeedConfiguration>
            {
                Data = config,
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

            var response = TestConnection(configuration);

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
        public HttpResponseMessage TestConfigurationConnection(EnterspeedConfiguration configuration)
        {
            return Request.CreateResponse(HttpStatusCode.OK, TestConnection(configuration));
        }

        private Response TestConnection(EnterspeedConfiguration configuration)
        {
            var testConfigurationService = new InMemoryEnterspeedUmbracoConfigurationProvider(configuration);
            var testConnection = new EnterspeedConnection(testConfigurationService);
            var enterspeedIngestService = new EnterspeedIngestService(testConnection);

            return enterspeedIngestService.Test();
        }
    }
}
