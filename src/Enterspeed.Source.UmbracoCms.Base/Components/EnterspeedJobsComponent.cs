#if NET10_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using Enterspeed.Source.UmbracoCms.Base.Data.Migration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.Base.Components
{
#if NET10_0_OR_GREATER
    public class EnterspeedJobsComponent : IAsyncComponent
#else
    public class EnterspeedJobsComponent : IComponent
#endif
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IRuntimeState _runtimeState;

        public EnterspeedJobsComponent(
            IScopeProvider scopeProvider,
            IKeyValueService keyValueService,
            IMigrationPlanExecutor migrationPlanExecutor,
            IRuntimeState runtimeState)
        {
            _scopeProvider = scopeProvider;
            _keyValueService = keyValueService;
            _migrationPlanExecutor = migrationPlanExecutor;
            _runtimeState = runtimeState;
        }

#if NET10_0_OR_GREATER
        public async Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var migrationPlan = new MigrationPlan("EnterspeedJobs");
            migrationPlan.From(string.Empty)
                .To<EnterspeedJobsTableMigration>("enterspeedjobs-db")
                .To<AddEntityTypeToJobsTable>("enterspeedjobs-db-v2")
                .To<AddContentStateToJobsTable>("enterspeedjobs-db-v3")
                .To<AddFailedCountToJobsTable>("enterspeedjobs-db-v4");

            var upgrader = new Upgrader(migrationPlan);
            await upgrader.ExecuteAsync(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
#else
        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var migrationPlan = new MigrationPlan("EnterspeedJobs");
            migrationPlan.From(string.Empty)
                .To<EnterspeedJobsTableMigration>("enterspeedjobs-db")
                .To<AddEntityTypeToJobsTable>("enterspeedjobs-db-v2")
                .To<AddContentStateToJobsTable>("enterspeedjobs-db-v3")
                .To<AddFailedCountToJobsTable>("enterspeedjobs-db-v4");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public void Terminate()
        {
        }
#endif
    }
}
