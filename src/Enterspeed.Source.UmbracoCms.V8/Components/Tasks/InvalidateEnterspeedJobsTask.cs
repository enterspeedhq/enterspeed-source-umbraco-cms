using Enterspeed.Source.UmbracoCms.V8.Handlers;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Sync;
using Umbraco.Web.Scheduling;

namespace Enterspeed.Source.UmbracoCms.V8.Components.Tasks
{
    public class InvalidateEnterspeedJobsTask : RecurringTaskBase
    {
        private readonly IRuntimeState _runtime;
        private readonly IProfilingLogger _logger;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;

        public InvalidateEnterspeedJobsTask(
            IBackgroundTaskRunner<RecurringTaskBase> runner,
            int delayMilliseconds,
            int periodMilliseconds,
            IRuntimeState runtime,
            IProfilingLogger logger,
            IEnterspeedJobHandler enterspeedJobHandler)
            : base(runner, delayMilliseconds, periodMilliseconds)
        {
            _runtime = runtime;
            _logger = logger;
            _enterspeedJobHandler = enterspeedJobHandler;
        }

        public override bool IsAsync => false;

        public override bool PerformRun()
        {
            if (_runtime.ServerRole == ServerRole.Master || _runtime.ServerRole == ServerRole.Single)
            {
                _enterspeedJobHandler.InvalidateOldProcessingJobs();
            }
            else
            {
                _logger.Debug<InvalidateEnterspeedJobsTask>("Does not run on servers with {role} role.", _runtime.ToString());
            }

            return true;
        }
    }
}
