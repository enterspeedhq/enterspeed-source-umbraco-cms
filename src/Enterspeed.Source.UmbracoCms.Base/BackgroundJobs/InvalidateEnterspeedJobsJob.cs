#if NET8_0_OR_GREATER
using System;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Enterspeed.Source.UmbracoCms.Base.BackgroundJobs
{
    /// <summary>
    /// Invalidates Enterspeed jobs that have been stuck in the processing state for too long.
    /// </summary>
    public class InvalidateEnterspeedJobsJob : EnterspeedRecurringJobBase
    {
        private readonly IServiceProvider _serviceProvider;

        public InvalidateEnterspeedJobsJob(IServiceProvider serviceProvider)
            : base(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1))
        {
            _serviceProvider = serviceProvider;
        }

        public override Task RunJobAsync(CancellationToken cancellationToken)
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var enterspeedJobsHandlingService = serviceProvider.GetRequiredService<IEnterspeedJobsHandlingService>();
                var logger = serviceProvider.GetRequiredService<ILogger<InvalidateEnterspeedJobsJob>>();
                var serverRoleAccessor = serviceProvider.GetRequiredService<IServerRoleAccessor>();
                var configurationService = serviceProvider.GetRequiredService<IEnterspeedConfigurationService>();
                var scopeProvider = serviceProvider.GetRequiredService<IScopeProvider>();

                if (!configurationService.GetConfiguration().IsConfigured)
                {
                    return Task.CompletedTask;
                }

                if (enterspeedJobsHandlingService.IsJobsProcessingEnabled())
                {
                    using (var scope = scopeProvider.CreateScope(autoComplete: true))
                    {
                        enterspeedJobsHandlingService.InvalidateOldProcessingJobs();
                    }
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
