using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public abstract class BaseEnterspeedNotificationHandler
    {
        internal readonly IEnterspeedConfigurationService _configurationService;
        internal readonly IEnterspeedJobRepository _enterspeedJobRepository;
        internal readonly IEnterspeedJobHandler _enterspeedJobHandler;
        internal readonly IUmbracoContextFactory _umbracoContextFactory;
        internal readonly IScopeProvider _scopeProvider;
        internal readonly IEnterspeedJobFactory _enterspeedJobFactory;
        internal readonly IAuditService _auditService;

        protected BaseEnterspeedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobHandler enterspeedJobHandler,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService)
        {
            _configurationService = configurationService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobHandler = enterspeedJobHandler;
            _umbracoContextFactory = umbracoContextFactory;
            _scopeProvider = scopeProvider;
            _enterspeedJobFactory = enterspeedJobFactory;
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

        internal string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }

        internal void EnqueueJobs(List<EnterspeedJob> jobs)
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
                using (var scope = _scopeProvider.CreateScope(autoComplete: true))
                {
                    _enterspeedJobHandler.HandleJobs(jobs);
                }
            }
        }
    }
}
