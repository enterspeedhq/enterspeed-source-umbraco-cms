using System;
using System.Linq;
using System.Net;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultConverters;
using Enterspeed.Source.UmbracoCms.V7.Services.DataProperties.DefaultGridConverters;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;

namespace Enterspeed.Source.UmbracoCms.V7.EventHandlers
{
    public class ConfigurationEventHandler : ApplicationEventHandler
    {
        private readonly SemVersion _jobsTableVersion = new SemVersion(3, 0, 0);
        private readonly SemVersion _configurationTableVersion = new SemVersion(2, 0, 0);

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
            SetupTable("EnterspeedJobs", _jobsTableVersion, applicationContext);
            SetupTable("EnterspeedConfiguration", _configurationTableVersion, applicationContext);
        }

        private void SetupTable(string tableName, SemVersion version, ApplicationContext applicationContext)
        {
            var databaseContext = applicationContext.DatabaseContext;
            var database = new DatabaseSchemaHelper(
                databaseContext.Database,
                applicationContext.ProfilingLogger.Logger,
                databaseContext.SqlSyntax);

            var currentVersion = new SemVersion(0, 0, 0);
            var migrations = ApplicationContext.Current.Services.MigrationEntryService.GetAll(tableName);
            var latestMigration = migrations.OrderByDescending(x => x.Version).FirstOrDefault();

            if (latestMigration != null)
            {
                currentVersion = latestMigration.Version;
            }
            else
            {
                // If there is no existing migrations, and the table is created,
                // this means that it was created before using migrations, so we need to delete it.
                if (database.TableExist(tableName))
                {
                    database.DropTable(tableName);
                }
            }

            if (version != currentVersion)
            {
                var migrationsRunner = new MigrationRunner(
                    ApplicationContext.Current.Services.MigrationEntryService,
                    ApplicationContext.Current.ProfilingLogger.Logger,
                    currentVersion,
                    version,
                    tableName);

                try
                {
                    migrationsRunner.Execute(applicationContext.DatabaseContext.Database);
                }
                catch (Exception e)
                {
                    LogHelper.Error<ConfigurationEventHandler>($"Error running {tableName} table migration", e);
                }
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
                .Append<DefaultMediaPicker2PropertyValueConverter>()
                .Append<DefaultMemberGroupPickerPropertyValueConverter>()
                .Append<DefaultMemberPickerPropertyValueConverter>()
                .Append<DefaultMultiNodeTreePickerPropertyValueConverter>()
                .Append<DefaultMultiNodeTreePicker2PropertyValueConverter>()
                .Append<DefaultNestedContentPropertyValueConverter>()
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
