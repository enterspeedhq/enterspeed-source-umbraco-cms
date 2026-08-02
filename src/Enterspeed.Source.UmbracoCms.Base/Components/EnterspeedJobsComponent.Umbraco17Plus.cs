#if NET10_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Enterspeed.Source.UmbracoCms.Base.Components
{
    /// <summary>
    /// Umbraco 17+ flavour: IComponent and the sync Upgrader.Execute were removed, so the
    /// component is an IAsyncComponent and the migration plan runs via ExecuteAsync.
    /// </summary>
    public partial class EnterspeedJobsComponent : IAsyncComponent
    {
        public async Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var upgrader = new Upgrader(BuildMigrationPlan());
            await upgrader.ExecuteAsync(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
#endif
