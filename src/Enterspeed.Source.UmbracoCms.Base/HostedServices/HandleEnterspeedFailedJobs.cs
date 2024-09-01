using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace Enterspeed.Source.UmbracoCms.Base.HostedServices
{
    public class HandleEnterspeedFailedJobs : RecurringHostedServiceBase
    {
        private readonly IServiceProvider _serviceProvider;

        public HandleEnterspeedFailedJobs(ILogger<HandleEnterspeedJobsHostedService> logger, IServiceProvider serviceProvider)
            : base(logger, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10))
        {
            _serviceProvider = serviceProvider;
        }

        public override Task PerformExecuteAsync(object state)
        {
            throw new NotImplementedException();
        }
    }
}
