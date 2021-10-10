using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V9.Components
{
    public class EnterspeedBackgroundTasksComponent : IComponent
    {
        // TODO: https://our.umbraco.com/documentation/reference/Scheduling/
        // private readonly BackgroundTaskRunner<IBackgroundTask> _handleJobsRunner;
        // private readonly BackgroundTaskRunner<IBackgroundTask> _invalidateJobsRunner;
        private readonly IProfilingLogger _logger;
        private readonly IRuntimeState _runtimeState;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;
        private readonly IEnterspeedConfigurationService _configurationService;

        public EnterspeedBackgroundTasksComponent(
            IProfilingLogger logger,
            IRuntimeState runtimeState,
            IEnterspeedJobHandler enterspeedJobHandler,
            IEnterspeedConfigurationService configurationService)
        {
            _logger = logger;
            _runtimeState = runtimeState;
            _enterspeedJobHandler = enterspeedJobHandler;
            _configurationService = configurationService;
            // _handleJobsRunner = new BackgroundTaskRunner<IBackgroundTask>("HandleEnterspeedJobs", _logger);
            // _invalidateJobsRunner = new BackgroundTaskRunner<IBackgroundTask>("InvalidateEnterspeedJobs", _logger);
        }

        public void Initialize()
        {
            SetupJobhandlerTask();
            SetupJobInvalidaterTask();
        }

        private void SetupJobhandlerTask()
        {
            var delay = 10000; // 10000ms = 10seconds
            var repeatAfter = 60000; //60000ms = 1minute

            // var task = new HandleEnterspeedJobsTask(
            //     _handleJobsRunner,
            //     delay,
            //     repeatAfter,
            //     _runtimeState,
            //     _logger,
            //     _enterspeedJobHandler,
            //     _configurationService);
            //
            // _handleJobsRunner.TryAdd(task);
        }

        private void SetupJobInvalidaterTask()
        {
            var delay = 60000; //60000ms = 1minute
            var repeatAfter = 600000; //600000ms = 10minutes

            // var task = new InvalidateEnterspeedJobsTask(
            //     _invalidateJobsRunner,
            //     delay,
            //     repeatAfter,
            //     _runtimeState,
            //     _logger,
            //     _enterspeedJobHandler,
            //     _configurationService);
            //
            // _invalidateJobsRunner.TryAdd(task);
        }

        public void Terminate()
        {
        }
    }
}
