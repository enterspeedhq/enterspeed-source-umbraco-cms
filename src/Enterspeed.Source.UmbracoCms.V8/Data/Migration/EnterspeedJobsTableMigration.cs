using Enterspeed.Source.UmbracoCms.V8.Data.Schemas;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;

namespace Enterspeed.Source.UmbracoCms.V8.Data.Migration
{
    public class EnterspeedJobsTableMigration : MigrationBase
    {
        public EnterspeedJobsTableMigration(IMigrationContext context)
            : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<EnterspeedJobsTableMigration>("Running migration {MigrationStep}", "JobsTableMigration");

            if (!TableExists("enterspeedJobs"))
            {
                Create.Table<EnterspeedJobSchema>().Do();
            }
            else
            {
                Logger.Debug<EnterspeedJobsTableMigration>("The database table {DbTable} already exists, skipping", "JobsTableMigration");
            }
        }
    }
}
