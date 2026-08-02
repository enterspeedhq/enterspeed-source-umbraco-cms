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
    /// Retries failed Enterspeed jobs when failed-jobs processing is enabled.
    /// </summary>
    public class HandleEnterspeedFailedJobsJob : EnterspeedRecurringJobBase
    {
        private readonly IServiceProvider _serviceProvider;

        public HandleEnterspeedFailedJobsJob(IServiceProvider serviceProvider)
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
                var logger = serviceProvider.GetRequiredService<ILogger<HandleEnterspeedFailedJobsJob>>();
                var serverRoleAccessor = serviceProvider.GetRequiredService<IServerRoleAccessor>();
                var configurationService = serviceProvider.GetRequiredService<IEnterspeedConfigurationService>();

                var configuration = configurationService.GetConfiguration();

                if (!configuration.IsConfigured)
                {
                    return Task.CompletedTask;
                }

                if (enterspeedJobsHandlingService.IsJobsProcessingEnabled() && configuration.EnabledFailedJobsProcessing)
                {
                    enterspeedJobsHandlingService.HandleFailedJobs(50, 5);
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
