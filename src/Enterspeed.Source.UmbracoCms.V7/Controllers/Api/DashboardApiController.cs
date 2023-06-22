using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Models;
using Enterspeed.Source.UmbracoCms.V7.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V7.Extensions;
using Enterspeed.Source.UmbracoCms.V7.Models.Api;
using Enterspeed.Source.UmbracoCms.V7.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V7.Providers;
using Enterspeed.Source.UmbracoCms.V7.Services.Serializers;
using Umbraco.Web.WebApi;

namespace Enterspeed.Source.UmbracoCms.V7.Controllers.Api
{
    [JsonCamelCaseFormatter]
    public class DashboardApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public ApiResponse<List<EnterspeedJob>> GetFailedJobs()
        {
            var result = EnterspeedContext.Current.Repositories.JobRepository.GetFailedJobs().ToList();

            return new ApiResponse<List<EnterspeedJob>>
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        public ApiResponse<SeedResponse> Seed()
        {
            var publishConfigured = EnterspeedContext.Current?.Configuration != null && EnterspeedContext.Current.Configuration.IsConfigured;
            var previewConfigured = EnterspeedContext.Current?.Configuration != null
                                    && EnterspeedContext.Current.Configuration.IsConfigured
                                    && !string.IsNullOrWhiteSpace(EnterspeedContext.Current.Configuration.PreviewApiKey);

            if (!publishConfigured && !previewConfigured)
            {
                return new ApiResponse<SeedResponse>
                {
                    ErrorCode = "400",
                    IsSuccess = false
                };
            }

            var response = EnterspeedContext.Current.Services.JobService.Seed(publishConfigured, previewConfigured);

            return new ApiResponse<SeedResponse>
            {
                Data = response,
                IsSuccess = true
            };
        }

        [HttpGet]
        public ApiResponse<EnterspeedConfiguration> GetEnterspeedConfiguration()
        {
            var config = EnterspeedContext.Current.Services.ConfigurationService.GetConfiguration();
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

            var response = TestConnections(configuration);

            if (!response.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            try
            {
                EnterspeedContext.Current.Services.ConfigurationService.Save(configuration);
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

            EnterspeedContext.Current.PublishConnection.Flush();
            EnterspeedContext.Current.PreviewConnection?.Flush();

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
            var enterspeedIngestService = new EnterspeedIngestService(testConnection, new NewtonsoftJsonSerializer(), testConfigurationService);

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
            var jobRepository = EnterspeedContext.Current.Repositories.JobRepository;
            var failedJobs = jobRepository.GetFailedJobs().ToList();

            if (failedJobs.Any())
            {
                jobRepository.Delete(failedJobs.Select(fj => fj.Id).ToList());
            }

            return new ApiResponse()
            {
                IsSuccess = true
            };
        }

        [HttpPost]
        public ApiResponse DeleteJobs(JobIdsToDelete jobIdsToDelete)
        {
            if (jobIdsToDelete != null && jobIdsToDelete.Ids.Any())
            {
                EnterspeedContext.Current.Repositories.JobRepository.Delete(jobIdsToDelete.Ids);
            }

            return new ApiResponse()
            {
                IsSuccess = true
            };
        }
    }
}