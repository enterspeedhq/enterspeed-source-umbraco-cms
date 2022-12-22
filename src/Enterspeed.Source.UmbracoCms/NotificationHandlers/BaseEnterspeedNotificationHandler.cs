using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.Data.Models;
using Enterspeed.Source.UmbracoCms.Data.Repositories;
using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
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
        internal readonly IEnterspeedConfigurationService _configurationService;
        internal readonly IEnterspeedJobRepository _enterspeedJobRepository;
        internal readonly IEnterspeedJobsHandlingService _enterspeedJobsHandlingService;
        internal readonly IUmbracoContextFactory _umbracoContextFactory;
        internal readonly IScopeProvider _scopeProvider;
        internal readonly IAuditService _auditService;

        protected BaseEnterspeedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IAuditService auditService)
        {
            _configurationService = configurationService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobsHandlingService = enterspeedJobsHandlingService;
            _umbracoContextFactory = umbracoContextFactory;
            _scopeProvider = scopeProvider;
            _auditService = auditService;
        }

        internal bool IsPublishConfigured()
        {
            return _configurationService.IsPublishConfigured();
        }

        internal bool IsPreviewConfigured()
        {
            return _configurationService.IsPreviewConfigured();
        }

        protected string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }

        protected void EnqueueJobs(List<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                _enterspeedJobRepository.Save(jobs);
            }

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

        private void HandleJobs(bool scopeCompleted, List<EnterspeedJob> jobs)
        {
            // Do not continue if the scope did not complete - the transaction may have been canceled and rolled back
            if (scopeCompleted)
            {
                using (_scopeProvider.CreateScope(autoComplete: true))
                {
                    _enterspeedJobsHandlingService.HandleJobs(jobs);
                }
            }
        }
    }
}
