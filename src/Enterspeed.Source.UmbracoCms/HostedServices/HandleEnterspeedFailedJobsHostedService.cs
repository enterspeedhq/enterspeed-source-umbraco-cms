using System;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;
#if NET5_0
using Umbraco.Cms.Core.Scoping;

#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.HostedServices
{
    public class HandleEnterspeedFailedJobsHostedService : RecurringHostedServiceBase
    {
        private readonly IServiceProvider _serviceProvider;

        public HandleEnterspeedFailedJobsHostedService(
            ILogger<HandleEnterspeedFailedJobsHostedService> logger,
            IServiceProvider serviceProvider)
                : base(logger, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1))
        {
            _serviceProvider = serviceProvider;
        }

        public override Task PerformExecuteAsync(object state)
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var runtimeState = serviceProvider.GetRequiredService<IRuntimeState>();
                var enterspeedJobsHandlingService = serviceProvider.GetRequiredService<IEnterspeedJobsHandlingService>();
                var logger = serviceProvider.GetRequiredService<ILogger<HandleEnterspeedFailedJobsHostedService>>();
                var serverRoleAccessor = serviceProvider.GetRequiredService<IServerRoleAccessor>();
                var configurationService = serviceProvider.GetRequiredService<IEnterspeedConfigurationService>();

                var configuration = configurationService.GetConfiguration();

                // Don't do anything if the site is not running.
                if (runtimeState.Level != RuntimeLevel.Run)
                {
                    return Task.CompletedTask;
                }

                if (!configurationService.GetConfiguration().IsConfigured)
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
