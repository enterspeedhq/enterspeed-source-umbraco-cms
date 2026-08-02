using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
    public class AddEntityTypeToJobsTable : EnterspeedMigrationBase
    {
        public AddEntityTypeToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void ExecuteMigration()
        {
            var jobsTable = Database.PocoDataFactory.ForType(typeof(EnterspeedJobSchema));
            var jobsTableName = jobsTable.TableInfo.TableName;

            var entityTypeColumnName = "EntityType";

            // If column doesnt exist, drop table and recreate it
            if (!ColumnExists(jobsTableName, entityTypeColumnName))
            {
                Delete.Table(jobsTableName).Do();
                Create.Table<EnterspeedJobSchema>().Do();
            }
        }
    }
}
