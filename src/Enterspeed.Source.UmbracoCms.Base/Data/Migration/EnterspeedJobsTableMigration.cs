#if NET10_0_OR_GREATER
using System.Threading.Tasks;
#endif
using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
#if NET10_0_OR_GREATER
    public class EnterspeedJobsTableMigration : AsyncMigrationBase
#else
    public class EnterspeedJobsTableMigration : MigrationBase
#endif
    {
        public EnterspeedJobsTableMigration(IMigrationContext context)
            : base(context)
        {
        }

#if NET10_0_OR_GREATER
        protected override Task MigrateAsync()
#else
        protected override void Migrate()
#endif
        {
            Logger.LogDebug("Running migration {MigrationStep}", "JobsTableMigration");

            if (!TableExists("enterspeedJobs"))
            {
                Create.Table<EnterspeedJobSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "JobsTableMigration");
            }
#if NET10_0_OR_GREATER

            return Task.CompletedTask;
#endif
        }
    }
}
