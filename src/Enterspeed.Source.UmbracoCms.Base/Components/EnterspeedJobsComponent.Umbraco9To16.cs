#if !NET10_0_OR_GREATER
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Enterspeed.Source.UmbracoCms.Base.Components
{
    /// <summary>
    /// Umbraco 9-16 flavour: the classic sync IComponent contract.
    /// </summary>
    public partial class EnterspeedJobsComponent : IComponent
    {
        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var upgrader = new Upgrader(BuildMigrationPlan());
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public void Terminate()
        {
        }
    }
}
#endif
