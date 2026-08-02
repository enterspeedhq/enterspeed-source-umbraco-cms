#if NET10_0_OR_GREATER
using System.Threading.Tasks;
#endif
using Umbraco.Cms.Infrastructure.Migrations;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Migration
{
    /// <summary>
    /// Bridges the migration base class difference between Umbraco generations so the
    /// concrete migrations stay free of conditional compilation: Umbraco 17+ removed the
    /// sync MigrationBase, so the net10.0 build derives from AsyncMigrationBase instead.
    /// Concrete migrations implement ExecuteMigration(); the bodies are sync-safe on both
    /// bases. Named ExecuteMigration because the Umbraco bases already expose an Execute
    /// fluent-builder property that a member named Execute would shadow.
    /// </summary>
    public abstract class EnterspeedMigrationBase :
#if NET10_0_OR_GREATER
        AsyncMigrationBase
#else
        MigrationBase
#endif
    {
        protected EnterspeedMigrationBase(IMigrationContext context)
            : base(context)
        {
        }

        protected abstract void ExecuteMigration();

#if NET10_0_OR_GREATER
        protected override Task MigrateAsync()
        {
            ExecuteMigration();
            return Task.CompletedTask;
        }
#else
        protected override void Migrate()
        {
            ExecuteMigration();
        }
#endif
    }
}
