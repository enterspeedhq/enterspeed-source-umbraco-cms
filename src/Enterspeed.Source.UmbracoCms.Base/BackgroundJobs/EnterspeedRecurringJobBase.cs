#if NET8_0_OR_GREATER
using System;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.BackgroundJobs;

namespace Enterspeed.Source.UmbracoCms.Base.BackgroundJobs
{
    /// <summary>
    /// Base class for the Enterspeed recurring background jobs (Umbraco 13+, where the
    /// IRecurringBackgroundJob system replaces our RecurringHostedServiceBase usage).
    /// The job runner gates on runtime level and MainDom before running a job, and runs
    /// each job on an isolated execution context, so jobs need neither the runtime-state
    /// check nor the ambient-scope workaround the old hosted services carried.
    /// </summary>
    public abstract class EnterspeedRecurringJobBase : IRecurringBackgroundJob
    {
        protected EnterspeedRecurringJobBase(TimeSpan period, TimeSpan delay)
        {
            Period = period;
            Delay = delay;
        }

        public TimeSpan Period { get; }

        public TimeSpan Delay { get; }

        // Declare every role so the runner never blocks on role: role handling stays in
        // the job bodies via IEnterspeedJobsHandlingService.IsJobsProcessingEnabled(),
        // which also honours the RunJobsOnAllServerRoles configuration setting
        public ServerRole[] ServerRoles => new[]
        {
            ServerRole.Unknown,
            ServerRole.Single,
            ServerRole.Subscriber,
            ServerRole.SchedulingPublisher,
        };

        // Our periods never change at runtime
        public event EventHandler PeriodChanged
        {
            add { }
            remove { }
        }

        public Task RunJobAsync()
        {
            return RunJobAsync(CancellationToken.None);
        }

        public abstract Task RunJobAsync(CancellationToken cancellationToken);
    }
}
#endif
