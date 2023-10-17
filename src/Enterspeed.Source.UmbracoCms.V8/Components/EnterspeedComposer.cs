using System.Configuration;
using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.Sdk.Domain.SystemTextJson;
using Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter;
using Enterspeed.Source.UmbracoCms.V8.Data.MappingDefinitions;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.EventHandlers;
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Handlers;
using Enterspeed.Source.UmbracoCms.V8.Handlers.Media;
using Enterspeed.Source.UmbracoCms.V8.Handlers.PreviewContent;
using Enterspeed.Source.UmbracoCms.V8.Handlers.PreviewDictionaries;
using Enterspeed.Source.UmbracoCms.V8.Handlers.PreviewMedia;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultConverters;
using Enterspeed.Source.UmbracoCms.V8.Services.DataProperties.DefaultGridConverters;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Mapping;

namespace Enterspeed.Source.UmbracoCms.V8.Components
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class EnterspeedComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var webConfigMediaDomain = ConfigurationManager.AppSettings["Enterspeed.MediaDomain"];
            if (!string.IsNullOrWhiteSpace(webConfigMediaDomain) && !webConfigMediaDomain.IsAbsoluteUrl())
            {
                throw new ConfigurationErrorsException(
                    "Configuration value for Enterspeed.MediaDomain must be absolute url");
            }

            composition.Register<IEnterspeedPropertyService, EnterspeedPropertyService>(Lifetime.Transient);
            composition.Register<IEnterspeedGridEditorService, EnterspeedGridEditorService>(Lifetime.Transient);

            composition.Register<IEnterspeedJobRepository, EnterspeedJobRespository>(Lifetime.Request);
            composition.Register<IUmbracoUrlService, UmbracoUrlService>(Lifetime.Request);
            composition.Register<IUmbracoContextProvider, UmbracoContextProvider>(Lifetime.Request);
            composition.Register<IEnterspeedIngestService, EnterspeedIngestService>(Lifetime.Singleton);
            composition.Register<IEntityIdentityService, UmbracoEntityIdentityService>(Lifetime.Request);
            composition.Register<IEnterspeedJobService, EnterspeedJobService>(Lifetime.Request);
            composition.Register<IEnterspeedConfigurationService, EnterspeedConfigurationService>(Lifetime.Singleton);
            composition.Register<IEnterspeedConfigurationProvider, EnterspeedUmbracoConfigurationProvider>(
                Lifetime.Singleton);
            composition.Register<IJsonSerializer, SystemTextJsonSerializer>(Lifetime.Singleton);
            composition.Register<IUmbracoMediaUrlProvider, UmbracoMediaUrlProvider>(Lifetime.Request);
            composition.Register<IUmbracoRedirectsService, UmbracoRedirectsService>(Lifetime.Request);
            composition.Register<IUmbracoRedirectsService, UmbracoRedirectsService>(Lifetime.Request);
            composition.Register<IEnterspeedGuardService, EnterspeedGuardService>(Lifetime.Request);
            composition.Register<IUrlFactory, UrlFactory>(Lifetime.Request);
            composition.Register<IEnterspeedJobFactory, EnterspeedJobFactory>(Lifetime.Request);
            composition.Register<IEnterspeedJobsHandler, EnterspeedJobsHandler>(Lifetime.Transient);
            composition.Register<IEnterspeedJobsHandlingService, EnterspeedJobsHandlingService>(Lifetime.Transient);
            composition.Register<IUmbracoRichTextParser, UmbracoRichTextParser>(Lifetime.Singleton);
            composition.Register<IEnterspeedValidationService, EnterspeedValidationService>(Lifetime.Singleton);
            composition.Register<IUmbracoCultureProvider, UmbracoCultureProvider>(Lifetime.Transient);

            composition.Register<IEnterspeedConnection>(
                c =>
                {
                    var configurationProvider = c.GetInstance<IEnterspeedConfigurationProvider>();
                    return new EnterspeedConnection(configurationProvider);
                }, Lifetime.Singleton);

            composition.Register<IEnterspeedConnectionProvider>(
                c =>
                {
                    var configurationProvider = c.GetInstance<IEnterspeedConfigurationProvider>();
                    var configurationService = c.GetInstance<IEnterspeedConfigurationService>();

                    var connectionProvider = new EnterspeedConnectionProvider(configurationService, configurationProvider);

                    return connectionProvider;
                }, Lifetime.Singleton);

            composition.EnterspeedPropertyValueConverters()
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

            // Property data mappers
            composition.EnterspeedPropertyDataMappers();

            // Property metadata mappers
            composition.EnterspeedPropertyMetaDataMappers();

            // Default grid editor value converters
            composition.EnterspeedGridEditorValueConverters()
                .Append<DefaultRichTextEditorGridEditorValueConverter>()
                .Append<DefaultQuoteGridEditorValueConverter>()
                .Append<DefaultImageGridEditorValueConverter>()
                .Append<DefaultHeadlineGridEditorValueConverter>()
                .Append<DefaultEmbedGridEditorValueConverter>();

            // Content handling guards
            composition.EnterspeedContentHandlingGuards();

            // Dictionary items handling guards
            composition.EnterspeedDictionaryItemHandlingGuards();

            // Media handling guards
            composition.EnterspeedMediaHandlingGuards();

            // Job handlers
            composition.EnterspeedJobHandlers()
                // Content
                .Append<EnterspeedContentPublishJobHandler>()
                .Append<EnterspeedContentDeleteJobHandler>()

                // Dictionaries
                .Append<EnterspeedDictionaryItemPublishJobHandler>()
                .Append<EnterspeedDictionaryItemDeleteJobHandler>()

                // Media
                .Append<EnterspeedMediaPublishJobHandler>()
                .Append<EnterspeedMediaTrashedJobHandler>()

                // Preview media
                .Append<EnterspeedPreviewMediaPublishJobHandler>()
                .Append<EnterspeedPreviewMediaTrashedJobHandler>()

                // Preview content
                .Append<EnterspeedPreviewContentPublishJobHandler>()
                .Append<EnterspeedPreviewContentDeleteJobHandler>()

                // Preview dictionaries
                .Append<EnterspeedPreviewDictionaryItemPublishJobHandler>()
                .Append<EnterspeedPreviewDictionaryItemDeleteJobHandler>();

            // Mapping definitions
            composition.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<EnterspeedJobMappingDefinition>();

            // Register event components
            composition.Components().Append<EnterspeedContentPublishingEventHandler>();
            composition.Components().Append<EnterspeedContentSavingEventHandler>();
            composition.Components().Append<EnterspeedContentCacheRefresherEventHandler>();
            composition.Components().Append<EnterspeedContentTrashingEventHandler>();
            composition.Components().Append<EnterspeedContentUnpublishingEventHandler>();

            composition.Components().Append<EnterspeedDictionaryItemSavedEventHandler>();
            composition.Components().Append<EnterspeedDictionaryItemDeletingEventHandler>();

            composition.Components().Append<EnterspeedMediaItemSavedEventHandler>();
            composition.Components().Append<EnterspeedMediaTrashedEventHandler>();
            composition.Components().Append<EnterspeedMediaMovedEventHandler>();

            composition.Components().Append<EnterspeedJobsComponent>();
            composition.Components().Append<EnterspeedBackgroundTasksComponent>();
        }
    }
}