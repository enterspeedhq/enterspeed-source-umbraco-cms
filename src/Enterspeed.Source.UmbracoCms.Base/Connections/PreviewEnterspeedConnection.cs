using System;
using System.Net.Http;
using System.Reflection;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Models.Configuration;
using Enterspeed.Source.UmbracoCms.Base.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Connections
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
        private string SystemInformation => _configuration.SystemInformation;

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
            _httpClientConnection.DefaultRequestHeaders.Add("X-Enterspeed-System", $"sdk-dotnet/{Assembly.GetExecutingAssembly().GetName().Version}");

            if (!string.IsNullOrEmpty(SystemInformation))
            {
                _httpClientConnection.DefaultRequestHeaders.Add("X-Enterspeed-System-Information", SystemInformation);
            }

            _connectionEstablishedDate = DateTime.Now;
        }

        public void Dispose()
        {
            _httpClientConnection.Dispose();
        }
    }
}