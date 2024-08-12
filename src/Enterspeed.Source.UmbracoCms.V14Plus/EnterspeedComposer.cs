using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.V14Plus.Configuration;
using Enterspeed.Source.UmbracoCms.V14Plus.Models;
using Enterspeed.Source.UmbracoCms.V14Plus.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V14Plus
{
    public class EnterspeedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
            builder.Services.AddSingleton<ISchemaIdSelector, EnterspeedSchemaIdSelector>();
            builder.Services.AddSingleton<IOperationIdSelector, EnterspeedOperationIdSelector>();
            builder.Services.Replace(ServiceDescriptor.Singleton<IEnterspeedConfigurationEditorProvider, EnterspeedConfigurationEditorProvider>());

            builder.Services.AddTransient<IEnterspeedJobService, EnterspeedJobService>();
            builder.Services.ConfigureOptions<ConfigureEnterspeedApiSwaggerGenOptions>();
        }
    }
}