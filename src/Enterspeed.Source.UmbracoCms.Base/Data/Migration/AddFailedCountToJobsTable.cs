#if NET10_0_OR_GREATER
using System.Threading.Tasks;
#endif
using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
#if NET10_0_OR_GREATER
    public class AddFailedCountToJobsTable : AsyncMigrationBase
#else
    public class AddFailedCountToJobsTable : MigrationBase
#endif
    {
        public AddFailedCountToJobsTable(IMigrationContext context)
            : base(context)
        {
        }

#if NET10_0_OR_GREATER
        protected override Task MigrateAsync()
#else
        protected override void Migrate()
#endif
        {
            var jobsTable = Database.PocoDataFactory.ForType(typeof(EnterspeedJobSchema));
            var jobsTableName = jobsTable.TableInfo.TableName;

            const string entityTypeColumnName = "FailedCount";

            // If column doesn't exist, create it
            if (!ColumnExists(jobsTableName, entityTypeColumnName))
            {
                Create
                    .Column(entityTypeColumnName)
                    .OnTable(jobsTableName)
                    .AsInt32().NotNullable().WithDefaultValue(0)
                    .Do();
            }
#if NET10_0_OR_GREATER

            return Task.CompletedTask;
#endif
        }
    }
}
