using Enterspeed.Source.UmbracoCms.Base.Composers;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.Providers;
using Enterspeed.Source.UmbracoCms.V14Plus.Configuration;
using Enterspeed.Source.UmbracoCms.V14Plus.Models;
using Enterspeed.Source.UmbracoCms.V14Plus.NotificationHandlers;
using Enterspeed.Source.UmbracoCms.V14Plus.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#if UMBRACO_18_OR_GREATER
using Enterspeed.Source.UmbracoCms.V14Plus.Configuration.Umbraco18;
#else
using Umbraco.Cms.Api.Common.OpenApi;
#endif
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Enterspeed.Source.UmbracoCms.V14Plus
{
    public class EnterspeedComposer : EnterspeedBaseComposer
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
#if UMBRACO_18_OR_GREATER
            builder.AddEnterspeedOpenApiDocument();
#else
            builder.Services.AddSingleton<EnterspeedSchemaIdSelector>();
            builder.Services.AddSingleton<EnterspeedOperationIdSelector>();
            builder.Services.AddSingleton<ISchemaIdSelector>(provider => provider.GetRequiredService<EnterspeedSchemaIdSelector>());
            builder.Services.AddSingleton<IOperationIdSelector>(provider => provider.GetRequiredService<EnterspeedOperationIdSelector>());
#endif
            builder.Services.Replace(ServiceDescriptor.Singleton<IEnterspeedConfigurationEditorProvider, EnterspeedConfigurationEditorProvider>());
            builder.Services.AddTransient<IEnterspeedJobService, EnterspeedJobService>();
#if !UMBRACO_18_OR_GREATER
            builder.Services.ConfigureOptions<ConfigureEnterspeedApiSwaggerGenOptions>();
#endif
            builder.AddNotificationHandler<ContentMovedToRecycleBinNotification, EnterspeedContentUnpublishingNotificationHandlerV14>();
            builder.AddNotificationHandler<ContentUnpublishingNotification, EnterspeedContentUnpublishingNotificationHandlerV14>();

            base.Compose(builder);
        }
    }
}