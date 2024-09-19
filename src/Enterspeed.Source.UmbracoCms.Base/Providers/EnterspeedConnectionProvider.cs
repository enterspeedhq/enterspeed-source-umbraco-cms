using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.UmbracoCms.Base.Connections;
using Enterspeed.Source.UmbracoCms.Base.Extensions;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.Services;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public class EnterspeedConnectionProvider : IEnterspeedConnectionProvider
    {
        private Dictionary<ConnectionType, IEnterspeedConnection> _connections;
        private readonly IEnterspeedConfigurationService _configurationService;
        private readonly IEnterspeedConfigurationProvider _configurationProvider;

        public EnterspeedConnectionProvider(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedConfigurationProvider configurationProvider)
        {
            _configurationService = configurationService;
            _configurationProvider = configurationProvider;
            Initialize();
        }

        public IEnterspeedConnection GetConnection(ConnectionType type)
        {
            if (!_connections.TryGetValue(type, out var connection))
            {
                return null;
            }

            return connection;
        }

        public void Initialize()
        {
            _connections = new Dictionary<ConnectionType, IEnterspeedConnection>();

            var configuration = _configurationService.GetConfiguration();
            var publishConfiguration = configuration?.GetPublishConfiguration();
            var previewConfiguration = configuration?.GetPreviewConfiguration();

            if (publishConfiguration != null)
            {
                _connections.Add(ConnectionType.Publish, new EnterspeedConnection(_configurationProvider));
            }

            if (previewConfiguration != null)
            {
                _connections.Add(ConnectionType.Preview, new PreviewEnterspeedConnection(_configurationService));
            }
        }
    }
}