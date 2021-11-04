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
using Enterspeed.Source.UmbracoCms.V8.Extensions;
using Enterspeed.Source.UmbracoCms.V8.Guards;
using Enterspeed.Source.UmbracoCms.V8.Handlers;
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
            composition.Register<IEnterspeedJobHandler, EnterspeedJobHandler>(Lifetime.Request);
            composition.Register<IUmbracoUrlService, UmbracoUrlService>(Lifetime.Request);
            composition.Register<IUmbracoContextProvider, UmbracoContextProvider>(Lifetime.Request);
            composition.Register<IEnterspeedIngestService, EnterspeedIngestService>(Lifetime.Singleton);
            composition.Register<IEntityIdentityService, UmbracoEntityIdentityService>(Lifetime.Request);
            composition.Register<IEnterspeedJobService, EnterspeedJobService>(Lifetime.Request);
            composition.Register<IEnterspeedConfigurationService, EnterspeedConfigurationService>(Lifetime.Singleton);
            composition.Register<IEnterspeedConfigurationProvider, EnterspeedUmbracoConfigurationProvider>(Lifetime.Singleton);
            composition.Register<IJsonSerializer, SystemTextJsonSerializer>(Lifetime.Singleton);
            composition.Register<IUmbracoMediaUrlProvider, UmbracoMediaUrlProvider>(Lifetime.Request);
            composition.Register<IUmbracoRedirectsService, UmbracoRedirectsService>(Lifetime.Request);
            composition.Register<IUmbracoRedirectsService, UmbracoRedirectsService>(Lifetime.Request);
            composition.Register<IEnterspeedGuardService, EnterspeedGuardService>(Lifetime.Request);

            composition.Register<IEnterspeedConnection>(
                c =>
                {
                    var configurationProvider = c.GetInstance<IEnterspeedConfigurationProvider>();
                    return new EnterspeedConnection(configurationProvider);
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

            // Default grid editor value converters
            composition.EnterspeedGridEditorValueConverters()
                .Append<DefaultRichTextEditorGridEditorValueConverter>();

            // Content handling guards
            composition.EnterspeedContentHandlingGuards()
                .Append<ContentCultureUrlRequiredGuard>();

            // Dictionary items handling guards
            composition.EnterspeedDictionaryItemHandlingGuards();

            // Mapping definitions
            composition.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<EnterspeedJobMappingDefinition>();

            // Register event components
            composition.Components().Append<EnterspeedContentEventsComponent>();
            composition.Components().Append<EnterspeedJobsComponent>();
            composition.Components().Append<EnterspeedBackgroundTasksComponent>();
            composition.Components().Append<EnterspeedDictionaryEventsComponent>();
        }
    }
}
