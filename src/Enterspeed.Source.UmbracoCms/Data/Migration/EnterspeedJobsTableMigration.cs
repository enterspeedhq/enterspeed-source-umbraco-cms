using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
    public class EnterspeedJobsTableMigration : MigrationBase
    {
        public EnterspeedJobsTableMigration(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
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
        }
    }
}
