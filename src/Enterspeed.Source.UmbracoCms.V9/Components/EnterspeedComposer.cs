using System.Configuration;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.V9.Components.DataPropertyValueConverter;
using Enterspeed.Source.UmbracoCms.V9.Components.NotificationHandlers;
using Enterspeed.Source.UmbracoCms.V9.Data.MappingDefinitions;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Providers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Enterspeed.Source.UmbracoCms.V9.Services.DataProperties;
using Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultConverters;
using Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultGridConverters;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V9.Components
{
    public class EnterspeedComposer : IComposer
    {
        //private readonly IRuntimeState _runtimeState;

        //public EnterspeedComposer(IRuntimeState runtimeState)
        //{
        //    _runtimeState = runtimeState;
        //}

        public void Compose(IUmbracoBuilder builder)
        {
            //if (_runtimeState.Level != RuntimeLevel.Run)
            //{
            //    return;
            //}
            var webConfigMediaDomain = builder.Config["Enterspeed:MediaDomain"];
            if (!string.IsNullOrWhiteSpace(webConfigMediaDomain) && !webConfigMediaDomain.IsAbsoluteUrl())
            {
                throw new ConfigurationErrorsException(
                    "Configuration value for Enterspeed.MediaDomain must be absolute url");
            }

            builder.Services.AddTransient<IEnterspeedPropertyService, EnterspeedPropertyService>();
            builder.Services.AddTransient<IEnterspeedGridEditorService, EnterspeedGridEditorService>();
            builder.Services.AddTransient<IEntityIdentityService, UmbracoEntityIdentityService>();
            builder.Services.AddTransient<IUmbracoContextProvider, UmbracoContextProvider>();
            builder.Services.AddTransient<IUmbracoMediaUrlProvider, UmbracoMediaUrlProvider>();
            builder.Services.AddTransient<IEnterspeedJobRepository, EnterspeedJobRespository>();
            builder.Services.AddTransient<IEnterspeedJobHandler, EnterspeedJobHandler>();
            builder.Services.AddTransient<IUmbracoUrlService, UmbracoUrlService>();
            builder.Services.AddTransient<IEnterspeedJobService, EnterspeedJobService>();
            builder.Services.AddTransient<IUmbracoRedirectsService, UmbracoRedirectsService>();

            builder.Services.AddSingleton<IEnterspeedIngestService, EnterspeedIngestService>();
            builder.Services.AddSingleton<IEnterspeedConfigurationService, EnterspeedConfigurationService>();
            builder.Services.AddSingleton<IEnterspeedConfigurationProvider, EnterspeedUmbracoConfigurationProvider>();
            builder.Services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();

            //var enterspeedConfigurationProvider = builder.Services.BuildServiceProvider().GetService<IEnterspeedConfigurationProvider>();
            builder.Services.AddSingleton<IEnterspeedConnection, EnterspeedConnection>();

            builder.EnterspeedPropertyValueConverters()
                .Append<DefaultBlockListPropertyValueConverter>()
                .Append<DefaultCheckboxPropertyValueConverter>()
                .Append<DefaultCheckboxListPropertyValueConverter>()
                .Append<DefaultColorPickerPropertyValueConverter>()
                .Append<DefaultContentPickerPropertyValueConverter>()
                .Append<DefaultDateTimePropertyValueConverter>()
                .Append<DefaultDecimalPropertyValueConverter>()
                .Append<DefaultDropdownPropertyValueConverter>()
                .Append<DefaultEmailAddressPropertyValueConverter>()
                .Append<DefaultFileUploadPropertyValueConverter>()
                .Append<DefaultGridLayoutPropertyValueConverter>()
                .Append<DefaultImageCropperPropertyValueConverter>()
                .Append<DefaultMarkdownEditorPropertyValueConverter>()
                .Append<DefaultMediaPickerPropertyValueConverter>()
                .Append<DefaultMemberGroupPickerPropertyValueConverter>()
                .Append<DefaultMemberPickerPropertyValueConverter>()
                .Append<DefaultMultiUrlPickerPropertyValueConverter>()
                .Append<DefaultMultiNodeTreePickerPropertyValueConverter>()
                .Append<DefaultNestedContentPropertyValueConverter>()
                .Append<DefaultNumericPropertyValueConverter>()
                .Append<DefaultRadioButtonListPropertyValueConverter>()
                .Append<DefaultRepeatableTextStringPropertyValueConverter>()
                .Append<DefaultRichTextEditorPropertyValueConverter>()
                .Append<DefaultSliderPropertyValueConverter>()
                .Append<DefaultTagsPropertyValueConverter>()
                .Append<DefaultTextAreaPropertyValueConverter>()
                .Append<DefaultTextboxPropertyValueConverter>()
                .Append<DefaultUserPickerPropertyValueConverter>();

            // Default grid editor value convertres
            builder.EnterspeedGridEditorValueConverters()
                .Append<DefaultRichTextEditorGridEditorValueConverter>();

            // Mapping definitions
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<EnterspeedJobMappingDefinition>();

            // Register event componenents
            builder
                .AddNotificationHandler<ContentPublishingNotification,
                    EnterspeedContentPublishingNotificationHandler>();
            builder
                .AddNotificationHandler<ContentCacheRefresherNotification,
                    EnterspeedContentCacheRefresherNotificationHandler>();
            builder
                .AddNotificationHandler<ContentMovedToRecycleBinNotification,
                    EnterspeedContentUnpublishingNotificationHandler>();
            builder
                .AddNotificationHandler<ContentUnpublishingNotification,
                    EnterspeedContentUnpublishingNotificationHandler>();

            // builder.Components().Append<EnterspeedContentEventsComponent>();
            builder.Components().Append<EnterspeedJobsComponent>();
            //builder.Components().Append<EnterspeedBackgroundTasksComponent>();
            // builder.Components().Append<EnterspeedDictionaryEventsComponent>();
        }
    }
}
