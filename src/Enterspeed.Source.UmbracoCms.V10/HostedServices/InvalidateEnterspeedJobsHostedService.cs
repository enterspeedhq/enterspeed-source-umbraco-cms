using System;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.V10.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Enterspeed.Source.UmbracoCms.V10.HostedServices
{
    public class InvalidateEnterspeedJobsHostedService : RecurringHostedServiceBase
    {
        private readonly IServiceProvider _serviceProvider;

        public InvalidateEnterspeedJobsHostedService(IServiceProvider serviceProvider, ILogger<InvalidateEnterspeedJobsHostedService> logger)
            : base(logger, TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1))
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
                var logger = serviceProvider.GetRequiredService<ILogger<HandleEnterspeedJobsHostedService>>();
                var serverRoleAccessor = serviceProvider.GetRequiredService<IServerRoleAccessor>();
                var configurationService = serviceProvider.GetRequiredService<IEnterspeedConfigurationService>();
                var scopeProvider = serviceProvider.GetRequiredService<IScopeProvider>();

                // Don't do anything if the site is not running.
                if (runtimeState.Level != RuntimeLevel.Run)
                {
                    return Task.CompletedTask;
                }

                if (!configurationService.GetConfiguration().IsConfigured)
                {
                    return Task.CompletedTask;
                }

                if (serverRoleAccessor.CurrentServerRole == ServerRole.SchedulingPublisher || serverRoleAccessor.CurrentServerRole == ServerRole.Single)
                {
                    using (var scope = scopeProvider.CreateScope(autoComplete: true))
                    {
                        enterspeedJobsHandlingService.InvalidateOldProcessingJobs();
                    }
                }
                else
                {
                    logger.LogDebug("Does not run on servers with {role} role.", serverRoleAccessor.CurrentServerRole.ToString());
                }

                return Task.CompletedTask;
            }
        }
    }
}