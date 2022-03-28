using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.V9.Data.Migration
{
    public class AddContentStateToJobsTable : MigrationBase
    {
        public AddContentStateToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
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
