using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.Services.DataProperties;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
#if NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Cms.Infrastructure.Scoping;
#endif

namespace Enterspeed.Source.UmbracoCms.NotificationHandlers
{
    public abstract class BaseEnterspeedNotificationHandler
    {
        protected readonly IEnterspeedConfigurationService _configurationService;
        protected readonly IEnterspeedJobRepository _enterspeedJobRepository;
        protected readonly IEnterspeedJobsHandlingService _enterspeedJobsHandlingService;
        protected readonly IUmbracoContextFactory _umbracoContextFactory;
        protected readonly IScopeProvider _scopeProvider;
        protected readonly IAuditService _auditService;
        protected readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly IEnterspeedMasterContentService _enterspeedMasterContentService;
        protected readonly ILogger _logger;

        protected BaseEnterspeedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IAuditService auditService,
            IServerRoleAccessor serverRoleAccessor,
            IEnterspeedMasterContentService enterspeedMasterContentService,
            ILogger logger)
        {
            _configurationService = configurationService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobsHandlingService = enterspeedJobsHandlingService;
            _umbracoContextFactory = umbracoContextFactory;
            _scopeProvider = scopeProvider;
            _auditService = auditService;
            _serverRoleAccessor = serverRoleAccessor;
            _enterspeedMasterContentService = enterspeedMasterContentService;
            _logger = logger;
        }

        protected bool IsPublishConfigured()
        {
            return _configurationService.IsPublishConfigured();
        }

        protected bool IsPreviewConfigured()
        {
            return _configurationService.IsPreviewConfigured();
        }

        protected void EnqueueJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            if (_enterspeedMasterContentService.IsMasterContentEnabled())
            {
                jobs.AddRange(_enterspeedMasterContentService.CreateMasterContentJobs(jobs));
            }

            _enterspeedJobRepository.Save(jobs);

            if (_enterspeedJobsHandlingService.IsJobsProcessingEnabled())
            {
                using (_umbracoContextFactory.EnsureUmbracoContext())
                {
                    if (_scopeProvider.Context != null)
                    {
                        var key = $"UpdateEnterspeed_{DateTime.Now.Ticks}";
                        // Add a callback to the current Scope which will execute when it's completed
                        _scopeProvider.Context.Enlist(key, scopeCompleted => HandleJobs(scopeCompleted, jobs));
                    }
                }
            }
            else
            {
                _logger.LogInformation("Enterspeed jobs does not run on servers with {role} role.", _serverRoleAccessor.CurrentServerRole.ToString());
            }
        }

        private void HandleJobs(bool scopeCompleted, List<EnterspeedJob> jobs)
        {
            // Do not continue if the scope did not complete - the transaction may have been canceled and rolled back
            if (scopeCompleted)
            {
                _enterspeedJobsHandlingService.HandleJobs(jobs);
            }
        }
    }
}