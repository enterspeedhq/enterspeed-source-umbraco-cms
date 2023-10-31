﻿using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Sync;
using Umbraco.Web.Scheduling;

namespace Enterspeed.Source.UmbracoCms.V8.Components.Tasks
{
    public class HandleEnterspeedJobsTask : RecurringTaskBase
    {
        private readonly IRuntimeState _runtime;
        private readonly IProfilingLogger _logger;
        private readonly IEnterspeedJobsHandlingService _enterspeedJobsHandlingService;
        private readonly IEnterspeedConfigurationService _configurationService;

        public HandleEnterspeedJobsTask(
            IBackgroundTaskRunner<RecurringTaskBase> runner,
            int delayMilliseconds,
            int periodMilliseconds,
            IRuntimeState runtime,
            IProfilingLogger logger,
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService)
            : base(runner, delayMilliseconds, periodMilliseconds)
        {
            _runtime = runtime;
            _logger = logger;
            _configurationService = configurationService;
            _enterspeedJobsHandlingService = enterspeedJobsHandlingService;
        }

        public override bool IsAsync => false;

        public override bool PerformRun()
        {
            if (!_configurationService.GetConfiguration().IsConfigured)
            {
                // Enterspeed is not yet configured, but we still want the background task to run.
                return true;
            }

            if (_configurationService.RunJobsOnServer(_runtime.ServerRole))
            {
                // Handle jobs in batches of 50
                _enterspeedJobsHandlingService.HandlePendingJobs(50);
            }
            else
            {
                _logger.Info<HandleEnterspeedJobsTask>("Enterspeed jobs does not run on servers with {role} role.", _runtime.ToString());
            }

            return true;
        }
    }
}
