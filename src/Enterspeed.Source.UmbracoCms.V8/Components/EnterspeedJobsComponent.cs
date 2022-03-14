using Enterspeed.Source.UmbracoCms.V8.Data.Migration;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Components
{
    public class EnterspeedJobsComponent : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger _logger;

        public EnterspeedJobsComponent(
            IScopeProvider scopeProvider,
            IMigrationBuilder migrationBuilder,
            IKeyValueService keyValueService,
            ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }

        public void Initialize()
        {
            var migrationPlan = new MigrationPlan("EnterspeedJobs");
            migrationPlan.From(string.Empty)
                .To<EnterspeedJobsTableMigration>("enterspeedjobs-db")
                .To<AddEntityTypeToJobsTable>("enterspeedjobs-db-v2")
                .To<AddContentStateToJobsTable>("enterspeedjobs-db-v3");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
}
