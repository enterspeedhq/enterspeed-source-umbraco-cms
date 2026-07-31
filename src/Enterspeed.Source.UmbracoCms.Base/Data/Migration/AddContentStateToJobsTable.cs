#if NET10_0_OR_GREATER
using System.Threading.Tasks;
#endif
using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
#if NET10_0_OR_GREATER
    public class AddContentStateToJobsTable : AsyncMigrationBase
#else
    public class AddContentStateToJobsTable : MigrationBase
#endif
    {
        public AddContentStateToJobsTable(IMigrationContext context)
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
#if NET10_0_OR_GREATER

            return Task.CompletedTask;
#endif
        }
    }
}
