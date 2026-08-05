#if NET8_0_OR_GREATER
using System;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Sync;

namespace Enterspeed.Source.UmbracoCms.Base.BackgroundJobs
{
    /// <summary>
    /// Processes the pending Enterspeed job queue.
    /// </summary>
    public class HandleEnterspeedJobsJob : EnterspeedRecurringJobBase
    {
        private readonly IServiceProvider _serviceProvider;

        public HandleEnterspeedJobsJob(IServiceProvider serviceProvider)
            : base(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10))
        {
            _serviceProvider = serviceProvider;
        }

        public override Task RunJobAsync(CancellationToken cancellationToken)
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var enterspeedJobsHandlingService = serviceProvider.GetRequiredService<IEnterspeedJobsHandlingService>();
                var logger = serviceProvider.GetRequiredService<ILogger<HandleEnterspeedJobsJob>>();
                var serverRoleAccessor = serviceProvider.GetRequiredService<IServerRoleAccessor>();
                var configurationService = serviceProvider.GetRequiredService<IEnterspeedConfigurationService>();

                if (!configurationService.GetConfiguration().IsConfigured)
                {
                    return Task.CompletedTask;
                }

                if (enterspeedJobsHandlingService.IsJobsProcessingEnabled())
                {
                    enterspeedJobsHandlingService.HandlePendingJobs(50);
                }
                else
                {
                    logger.LogInformation("Enterspeed jobs does not run on servers with {role} role.", serverRoleAccessor.CurrentServerRole.ToString());
                }
            }

            return Task.CompletedTask;
        }
    }
}
#endif
