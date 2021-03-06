﻿using System.Net;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Data.Schemas;
using Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters;
using Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultGridConverters;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class ConfigurationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ConfigureDatabase(applicationContext);
            ConfigurePropertyValueConverters();
            ConfigureGridEditorValueConverters();
            ConfigureScheduledTasks();

            // TOOD: Maybe handle this better?
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        private void ConfigureDatabase(ApplicationContext applicationContext)
        {
            var databaseContext = applicationContext.DatabaseContext;
            var database = new DatabaseSchemaHelper(
                databaseContext.Database,
                applicationContext.ProfilingLogger.Logger,
                databaseContext.SqlSyntax);

            if (!database.TableExist("EnterspeedConfiguration"))
            {
                database.CreateTable<EnterspeedConfigurationSchema>(false);
            }

            if (!database.TableExist("EnterspeedJobs"))
            {
                database.CreateTable<EnterspeedJobSchema>(false);
            }
        }

        private void ConfigurePropertyValueConverters()
        {
            EnterspeedContext.Current.EnterspeedPropertyValueConverters
                .Append<DefaultRichTextEditorPropertyValueConverter>()
                .Append<DefaultGridLayoutPropertyValueConverter>()
                .Append<DefaultCheckboxListPropertyValueConverter>()
                .Append<DefaultColorPickerPropertyValueConverter>()
                .Append<DefaultContentPickerPropertyValueConverter>()
                .Append<DefaultDatePickerPropertyValueConverter>()
                .Append<DefaultDecimalPropertyValueConverter>()
                .Append<DefaultDropdownListPropertyValueConverter>()
                .Append<DefaultDropdownListMultiplePropertyValueConverter>()
                .Append<DefaultDropdownListMultiplePublishingKeysPropertyValueConverter>()
                .Append<DefaultDropdownListPublishingKeysPropertyValueConverter>()
                .Append<DefaultEmailAddressPropertyValueConverter>()
                .Append<DefaultFileUploadPropertyValueConverter>()
                .Append<DefaultImageCropperPropertyValueConverter>()
                .Append<DefaultLegacyMediaPickerPropertyValueConverter>()
                .Append<DefaultMarkdownEditorPropertyValueConverter>()
                .Append<DefaultMediaPickerPropertyValueConverter>()
                .Append<DefaultMemberGroupPickerPropertyValueConverter>()
                .Append<DefaultMemberPickerPropertyValueConverter>()
                .Append<DefaultMultiNodeTreePickerPropertyValueConverter>()
                .Append<DefaultNumericPropertyValueConverter>()
                .Append<DefaultRadioButtonListPropertyValueConverter>()
                .Append<DefaultRelatedLinksPropertyValueConverter>()
                .Append<DefaultRepeatableTextStringPropertyValueConverter>()
                .Append<DefaultSliderPropertyValueConverter>()
                .Append<DefaultTagsPropertyValueConverter>()
                .Append<DefaultTextAreaPropertyValueConverter>()
                .Append<DefaultTextboxPropertyValueConverter>()
                .Append<DefaultTrueFalsePropertyValueConverter>()
                .Append<DefaultUserPickerPropertyValueConverter>();
        }

        private void ConfigureGridEditorValueConverters()
        {
            EnterspeedContext.Current.EnterspeedGridEditorValueConverters
                .Append<DefaultRichTextEditorGridEditorValueConverter>()
                .Append<DefaultEmbedGridEditorValueConverter>()
                .Append<DefaultHeadlineGridEditorValueConverter>()
                .Append<DefaultImageGridEditorValueConverter>()
                .Append<DefaultQuoteGridEditorValueConverter>();
        }

        private void ConfigureScheduledTasks()
        {
            // Handle pending jobs every minute
            EnterspeedContext.Current.Services.SchedulerService.ScheduleTask(60000, (sender, args) =>
            {
                EnterspeedContext.Current.Handlers.JobHandler.HandlePendingJobs(50);
            });

            // Invalidate old processing jobs every 10 minutes
            EnterspeedContext.Current.Services.SchedulerService.ScheduleTask(600000, (sender, args) =>
            {
                EnterspeedContext.Current.Handlers.JobHandler.InvalidateOldProcessingJobs();
            });
        }
    }
}
