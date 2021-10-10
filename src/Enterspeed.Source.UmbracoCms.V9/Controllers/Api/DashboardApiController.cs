using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Models.Api;
using Enterspeed.Source.UmbracoCms.V9.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V9.Providers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;

namespace Enterspeed.Source.UmbracoCms.V9.Controllers.Api
{
    [JsonCamelCaseFormatter]
    public class DashboardApiController : UmbracoAuthorizedApiController
    {
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
            IHttpContextAccessor httpContextAccessor)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobService = enterspeedJobService;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _enterspeedConnection = enterspeedConnection;
            _httpContextAccessor = httpContextAccessor;
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
            if (!_enterspeedConfigurationService.GetConfiguration().IsConfigured)
            {
                return BadRequest(
                    new Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Enterspeed has not yet been configured"
                    });
            }

            var response = _enterspeedJobService.Seed();
            return Ok(
                new ApiResponse<SeedResponse>
                {
                    Data = response,
                    IsSuccess = true
                });
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
                    new Uri(_httpContextAccessor.HttpContext!.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
            }

            var response = TestConnection(configuration);
            if (!response.Success)
            {
                // TODO: why is this OK?
                return Ok(response);
            }

            try
            {
                _enterspeedConfigurationService.Save(configuration);
            }
            catch (ConfigurationErrorsException exception)
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

        [HttpPost]
        public IActionResult TestConfigurationConnection(EnterspeedConfiguration configuration)
        {
            return Ok(TestConnection(configuration));
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