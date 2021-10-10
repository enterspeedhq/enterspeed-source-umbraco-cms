// using Enterspeed.Source.UmbracoCms.V9.Handlers;
// using Enterspeed.Source.UmbracoCms.V9.Services;
// using Umbraco.Cms.Core.Logging;
// using Umbraco.Cms.Core.Services;
// using Umbraco.Cms.Core.Sync;
//
// namespace Enterspeed.Source.UmbracoCms.V9.Components.Tasks
// {
//     public class HandleEnterspeedJobsTask : RecurringTaskBase
//     {
//         private readonly IRuntimeState _runtime;
//         private readonly IProfilingLogger _logger;
//         private readonly IEnterspeedJobHandler _enterspeedJobHandler;
//         private readonly IEnterspeedConfigurationService _configurationService;
//
//         public HandleEnterspeedJobsTask(
//             IBackgroundTaskRunner<RecurringTaskBase> runner,
//             int delayMilliseconds,
//             int periodMilliseconds,
//             IRuntimeState runtime,
//             IProfilingLogger logger,
//             IEnterspeedJobHandler enterspeedJobHandler,
//             IEnterspeedConfigurationService configurationService)
//             : base(runner, delayMilliseconds, periodMilliseconds)
//         {
//             _runtime = runtime;
//             _logger = logger;
//             _enterspeedJobHandler = enterspeedJobHandler;
//             _configurationService = configurationService;
//         }
//
//         public override bool IsAsync => false;
//
//         public override bool PerformRun()
//         {
//             if (!_configurationService.GetConfiguration().IsConfigured)
//             {
//                 // Enterspeed is not yet configured, but we still want the background task to run.
//                 return true;
//             }
//
//             if (_runtime.ServerRole == ServerRole.Master || _runtime.ServerRole == ServerRole.Single)
//             {
//                 // Handle jobs in batches of 50
//                 _enterspeedJobHandler.HandlePendingJobs(50);
//             }
//             else
//             {
//                 _logger.Debug<HandleEnterspeedJobsTask>("Does not run on servers with {role} role.", _runtime.ToString());
//             }
//
//             return true;
//         }
//     }
// }
