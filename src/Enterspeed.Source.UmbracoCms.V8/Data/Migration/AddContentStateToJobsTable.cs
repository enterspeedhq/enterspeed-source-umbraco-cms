using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Schemas;
using Umbraco.Core.Migrations;

namespace Enterspeed.Source.UmbracoCms.V8.Data.Migration
{
    public class AddContentStateToJobsTable : MigrationBase
    {
        public AddContentStateToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

        public override void Migrate()
        {
            var jobsTable = Database.PocoDataFactory.ForType(typeof(EnterspeedJobSchema));
            var jobsTableName = jobsTable.TableInfo.TableName;

            var entityTypeColumnName = "ContentState";

            // If column doesnt exist, create it
            if (!ColumnExists(jobsTableName, entityTypeColumnName))
            {
                Create
                    .Column("ContentState")
                    .OnTable(jobsTableName)
                    .AsInt32().NotNullable().WithDefaultValue(EnterspeedContentState.Publish.GetHashCode())
                    .Do();
            }
        }
    }
}
