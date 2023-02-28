﻿using System;
using System.Net.Http;
using System.Reflection;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Models.Configuration;
using Enterspeed.Source.UmbracoCms.Services;

namespace Enterspeed.Source.UmbracoCms.Connections
{
    public class PreviewEnterspeedConnection : IEnterspeedConnection
    {
        private readonly EnterspeedUmbracoConfiguration _configuration;
        private HttpClient _httpClientConnection;
        private DateTime? _connectionEstablishedDate;

        public PreviewEnterspeedConnection(IEnterspeedConfigurationService configurationService)
        {
            _configuration = configurationService.GetConfiguration().GetPreviewConfiguration();
        }

        private string ApiKey => _configuration.ApiKey;
        private int ConnectionTimeout => _configuration.ConnectionTimeout;
        private string BaseUrl => _configuration.BaseUrl;

        public void Flush()
        {
            _httpClientConnection = null;
            _connectionEstablishedDate = null;
        }

        public HttpClient HttpClientConnection
        {
            get
            {
                if (_httpClientConnection == null
                    || !_connectionEstablishedDate.HasValue
                    || (DateTime.Now - _connectionEstablishedDate.Value).TotalSeconds > ConnectionTimeout)
                {
                    Connect();
                }

                return _httpClientConnection;
            }
        }

        private void Connect()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new Exception("ApiKey is missing in the connection.");
            }

            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new Exception("BaseUrl is missing in the connection.");
            }

            _httpClientConnection = new HttpClient();
            _httpClientConnection.BaseAddress = new Uri(BaseUrl);
            _httpClientConnection.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
            _httpClientConnection.DefaultRequestHeaders.Add("Accept", "application/json");

#if NETSTANDARD2_0_OR_GREATER || NET || NETCOREAPP2_0_OR_GREATER
            _httpClientConnection.DefaultRequestHeaders.Add("X-Enterspeed-System", $"sdk-dotnet/{Assembly.GetExecutingAssembly().GetName().Version}");
#endif

            _connectionEstablishedDate = DateTime.Now;
        }

        public void Dispose()
        {
            _httpClientConnection.Dispose();
        }
    }
}