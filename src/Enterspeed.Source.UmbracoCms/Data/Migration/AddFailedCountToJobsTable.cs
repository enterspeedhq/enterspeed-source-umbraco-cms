using Enterspeed.Source.UmbracoCms.Data.Schemas;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Data.Migration
{
    public class AddFailedCountToJobsTable : MigrationBase
    {
        public AddFailedCountToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            var jobsTable = Database.PocoDataFactory.ForType(typeof(EnterspeedJobSchema));
            var jobsTableName = jobsTable.TableInfo.TableName;

            const string entityTypeColumnName = "FailedCount";

            // If column doesnt exist, create it
            if (!ColumnExists(jobsTableName, entityTypeColumnName))
            {
                Create
                    .Column(entityTypeColumnName)
                    .OnTable(jobsTableName)
                    .AsInt32().WithDefaultValue(0)
                    .Do();
            }
        }
    }
}
