using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Core.Scoping;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public abstract class BaseEnterspeedEventHandler
    {
        internal readonly IUmbracoContextFactory _umbracoContextFactory;
        internal readonly IEnterspeedJobRepository _enterspeedJobRepository;
        internal readonly IEnterspeedJobsHandlingService _jobsHandlingService;
        internal readonly IEnterspeedConfigurationService _configurationService;
        internal readonly IScopeProvider _scopeProvider;

        protected BaseEnterspeedEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedJobRepository = enterspeedJobRepository;
            _jobsHandlingService = jobsHandlingService;
            _configurationService = configurationService;
            _scopeProvider = scopeProvider;
        }

        protected string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }

        protected void EnqueueJobs(IList<EnterspeedJob> jobs)
        {
            if (!jobs.Any())
            {
                return;
            }

            _enterspeedJobRepository.Save(jobs);

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

        protected void HandleJobs(bool scopeCompleted, IList<EnterspeedJob> jobs)
        {
            // Do not continue if the scope did not complete - the transaction may have been canceled and rolled back
            if (scopeCompleted)
            {
                _jobsHandlingService.HandleJobs(jobs);
            }
        }

        protected bool IsConfigured()
        {
            return _configurationService.GetConfiguration().IsConfigured;
        }
    }
}