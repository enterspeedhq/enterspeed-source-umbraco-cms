using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Configuration;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.Components;
using Enterspeed.Source.UmbracoCms.Data.MappingDefinitions;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.DataPropertyValueConverters;
using Enterspeed.Source.UmbracoCms.Extensions;
using Enterspeed.Source.UmbracoCms.Factories;
using Enterspeed.Source.UmbracoCms.Guards;
using Enterspeed.Source.UmbracoCms.Handlers;
using Enterspeed.Source.UmbracoCms.Handlers.Content;
using Enterspeed.Source.UmbracoCms.Handlers.Dictionaries;
using Enterspeed.Source.UmbracoCms.Handlers.Media;
using Enterspeed.Source.UmbracoCms.Handlers.PreviewContent;
using Enterspeed.Source.UmbracoCms.Handlers.PreviewDictionaries;
using Enterspeed.Source.UmbracoCms.Handlers.PreviewMedia;
using Enterspeed.Source.UmbracoCms.HostedServices;
using Enterspeed.Source.UmbracoCms.NotificationHandlers;
using Enterspeed.Source.UmbracoCms.Providers;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.Services.DataProperties.DefaultConverters;
using Enterspeed.Source.UmbracoCms.Services.DataProperties.DefaultGridConverters;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;

namespace Enterspeed.Source.UmbracoCms.Composers
{
    public class EnterspeedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var webConfigMediaDomain = builder.Config["Enterspeed:MediaDomain"];
            if (!string.IsNullOrWhiteSpace(webConfigMediaDomain) && !webConfigMediaDomain.IsAbsoluteUrl())
            {
                throw new ConfigurationException(
                    "Configuration value for Enterspeed.MediaDomain must be absolute url");
            }

            builder.Services.AddTransient<IEnterspeedPropertyService, EnterspeedPropertyService>();
            builder.Services.AddTransient<IEnterspeedGridEditorService, EnterspeedGridEditorService>();
            builder.Services.AddTransient<IEntityIdentityService, UmbracoEntityIdentityService>();
            builder.Services.AddTransient<IUmbracoContextProvider, UmbracoContextProvider>();
            builder.Services.AddTransient<IUmbracoMediaUrlProvider, UmbracoMediaUrlProvider>();
            builder.Services.AddTransient<IEnterspeedJobRepository, EnterspeedJobRespository>();
            builder.Services.AddTransient<IUmbracoUrlService, UmbracoUrlService>();
            builder.Services.AddTransient<IEnterspeedJobService, EnterspeedJobService>();
            builder.Services.AddTransient<IUmbracoRedirectsService, UmbracoRedirectsService>();
            builder.Services.AddTransient<IEnterspeedJobsHandler, EnterspeedJobsHandler>();
            builder.Services.AddTransient<IEnterspeedGuardService, EnterspeedGuardService>();
            builder.Services.AddTransient<IUrlFactory, UrlFactory>();
            builder.Services.AddTransient<IEnterspeedJobFactory, EnterspeedJobFactory>();
            builder.Services.AddTransient<IEnterspeedJobsHandlingService, EnterspeedJobsHandlingService>();
            builder.Services.AddSingleton<IEnterspeedValidationService, EnterspeedValidationService>();
            builder.Services.AddTransient<IUmbracoCultureProvider, UmbracoCultureProvider>();
                
            builder.Services.AddSingleton<IEnterspeedIngestService, EnterspeedIngestService>();
            builder.Services.AddSingleton<IEnterspeedConfigurationService, EnterspeedConfigurationService>();
            builder.Services.AddSingleton<IEnterspeedConfigurationProvider, EnterspeedUmbracoConfigurationProvider>();
            builder.Services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
            builder.Services.AddSingleton<IEnterspeedConnection, EnterspeedConnection>();
            builder.Services.AddSingleton<IUmbracoRichTextParser, UmbracoRichTextParser>();

            builder.Services.AddSingleton<IEnterspeedConnectionProvider>(
                c =>
                {
                    var configurationProvider = c.GetService<IEnterspeedConfigurationProvider>();
                    var configurationService = c.GetService<IEnterspeedConfigurationService>();

                    var connectionProvider =
                        new EnterspeedConnectionProvider(configurationService, configurationProvider);

                    return connectionProvider;
                });

            builder.EnterspeedPropertyValueConverters()
                .Append<DefaultBlockListPropertyValueConverter>()
#if NET6_0_OR_GREATER
                            .Append<DefaultBlockGridPropertyValueConverter>()
#endif
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
                .Append<DefaultLegacyMediaPickerPropertyValueConverter>()
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

            // Default grid editor value converters
            builder.EnterspeedGridEditorValueConverters()
                .Append<DefaultRichTextEditorGridEditorValueConverter>()
                .Append<DefaultEmbedGridEditorValueConverter>()
                .Append<DefaultHeadlineGridEditorValueConverter>()
                .Append<DefaultImageGridEditorValueConverter>()
                .Append<DefaultQuoteGridEditorValueConverter>();

            // Content handling guards
            builder.EnterspeedContentHandlingGuards()
                .Append<ContentCultureUrlRequiredGuard>();

            // Dictionary items handling guards
            builder.EnterspeedDictionaryItemHandlingGuards();

            // Media handling guards
            builder.EnterspeedMediaHandlingGuards();

            // Job handlers
            builder.EnterspeedJobHandlers()
                // Content
                .Append<EnterspeedContentPublishJobHandler>()
                .Append<EnterspeedContentDeleteJobHandler>()

                // Dictionaries
                .Append<EnterspeedDictionaryItemPublishJobHandler>()
                .Append<EnterspeedDictionaryItemDeleteJobHandler>()

                // Preview content
                .Append<EnterspeedPreviewContentPublishJobHandler>()
                .Append<EnterspeedPreviewContentDeleteJobHandler>()

                // Preview dictionaries
                .Append<EnterspeedPreviewDictionaryItemPublishJobHandler>()
                .Append<EnterspeedPreviewDictionaryItemDeleteJobHandler>()

                // Media 
                .Append<EnterspeedMediaPublishJobHandler>()
                .Append<EnterspeedMediaTrashedJobHandler>()

                // Preview media
                .Append<EnterspeedPreviewMediaPublishJobHandler>()
                .Append<EnterspeedPreviewMediaTrashedJobHandler>();


            // Mapping definitions
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<EnterspeedJobMappingDefinition>();

            // Notification handlers
            builder.AddNotificationHandler<ContentPublishingNotification, EnterspeedContentPublishingNotificationHandler>();
            builder.AddNotificationHandler<ContentCacheRefresherNotification, EnterspeedContentCacheRefresherNotificationHandler>();
#if NET5_0
            builder.AddNotificationHandler<ContentSortedNotification, EnterspeedContentSortedNotificationHandler>();
#endif
            builder.AddNotificationHandler<ContentMovedToRecycleBinNotification, EnterspeedContentUnpublishingNotificationHandler>();
            builder.AddNotificationHandler<ContentUnpublishingNotification, EnterspeedContentUnpublishingNotificationHandler>();
            builder.AddNotificationHandler<DictionaryItemSavedNotification, EnterspeedDictionaryItemSavedNotificationHandler>();
            builder.AddNotificationHandler<DictionaryItemDeletingNotification, EnterspeedDictionaryItemDeletingNotificationHandler>();
            builder.AddNotificationHandler<MediaSavedNotification, EnterspeedMediaItemSavedEventHandler>();
            builder.AddNotificationHandler<MediaMovedNotification, EnterspeedMediaMovedEventHandler>();
            builder.AddNotificationHandler<MediaMovedToRecycleBinNotification, EnterspeedMediaTrashedNotificationHandler>();

            // Components
            builder.Components().Append<EnterspeedJobsComponent>();

            // Hosted Services
            builder.Services.AddHostedService<HandleEnterspeedJobsHostedService>();
            builder.Services.AddHostedService<InvalidateEnterspeedJobsHostedService>();
        }
    }
}