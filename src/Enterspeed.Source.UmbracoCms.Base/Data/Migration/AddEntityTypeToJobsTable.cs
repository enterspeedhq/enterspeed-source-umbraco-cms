#if NET10_0_OR_GREATER
using System.Threading.Tasks;
#endif
using Enterspeed.Source.UmbracoCms.Base.Data.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
#if NET10_0_OR_GREATER
    public class AddEntityTypeToJobsTable : AsyncMigrationBase
#else
    public class AddEntityTypeToJobsTable : MigrationBase
#endif
    {
        public AddEntityTypeToJobsTable(IMigrationContext context)
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

            var entityTypeColumnName = "EntityType";

            // If column doesnt exist, drop table and recreate it
            if (!ColumnExists(jobsTableName, entityTypeColumnName))
            {
                Delete.Table(jobsTableName).Do();
                Create.Table<EnterspeedJobSchema>().Do();
            }
#if NET10_0_OR_GREATER

            return Task.CompletedTask;
#endif
        }
    }
}
