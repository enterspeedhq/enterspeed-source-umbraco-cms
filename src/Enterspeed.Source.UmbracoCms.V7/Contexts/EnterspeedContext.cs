using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.UmbracoCms.V7.Models;
using Enterspeed.Source.UmbracoCms.V7.Models.Configuration;
using Enterspeed.Source.UmbracoCms.V7.Services.DataProperties;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedContext
    {
        private static EnterspeedContext _current;
        private EnterspeedServiceContext _serviceContext;
        private EnterspeedProviderContext _providerContext;
        private EnterspeedRepositoryContext _repositoryContext;
        private EnterspeedHandlerContext _handlerContext;
        private IEnterspeedConnection _publishConnection;
        private IEnterspeedConnection _previewConnection;
        private OrderedCollection<IEnterspeedPropertyValueConverter> _propertyValueConverters;
        private OrderedCollection<IEnterspeedGridEditorValueConverter> _gridEditorValueConverters;

        public static EnterspeedContext Current => _current ?? (_current = new EnterspeedContext());

        public EnterspeedServiceContext Services => _serviceContext ?? (_serviceContext = new EnterspeedServiceContext());
        public EnterspeedProviderContext Providers => _providerContext ?? (_providerContext = new EnterspeedProviderContext());
        public EnterspeedRepositoryContext Repositories => _repositoryContext ?? (_repositoryContext = new EnterspeedRepositoryContext());

        public EnterspeedHandlerContext Handlers => _handlerContext ?? (_handlerContext = new EnterspeedHandlerContext());

        public IEnterspeedConnection PublishConnection => _publishConnection ?? (_publishConnection = new EnterspeedConnection(Providers.PublishConfigurationProvider));

        public IEnterspeedConnection PreviewConnection => _previewConnection ?? (_previewConnection =
            Providers.PreviewConfigurationProvider != null
                ? new EnterspeedConnection(Providers.PreviewConfigurationProvider)
                : null);
        public UmbracoGlobalSettings UmbracoGlobalSettings => new UmbracoGlobalSettings();
        public EnterspeedUmbracoConfiguration Configuration => Services.ConfigurationService.GetConfiguration();

        public OrderedCollection<IEnterspeedPropertyValueConverter> EnterspeedPropertyValueConverters =>
            _propertyValueConverters
            ?? (_propertyValueConverters = new OrderedCollection<IEnterspeedPropertyValueConverter>());

        public OrderedCollection<IEnterspeedGridEditorValueConverter> EnterspeedGridEditorValueConverters =>
            _gridEditorValueConverters
            ?? (_gridEditorValueConverters = new OrderedCollection<IEnterspeedGridEditorValueConverter>());
    }
}
