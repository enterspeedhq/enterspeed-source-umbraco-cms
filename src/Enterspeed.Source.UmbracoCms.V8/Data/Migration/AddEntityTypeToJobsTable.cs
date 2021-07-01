using Enterspeed.Source.UmbracoCms.V8.Data.Schemas;
using Umbraco.Core.Migrations;

namespace Enterspeed.Source.UmbracoCms.V8.Data.Migration
{
    public class AddEntityTypeToJobsTable : MigrationBase
    {
        public AddEntityTypeToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

        public override void Migrate()
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
