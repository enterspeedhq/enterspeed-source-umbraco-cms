using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Components.NotificationHandlers
{
    public class EnterspeedContentPublishingNotificationHandler : INotificationHandler<ContentPublishingNotification>
    {
        private readonly IEnterspeedConfigurationService _configurationService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IContentService _contentService;
        private readonly IScopeProvider _scopeProvider;

        public EnterspeedContentPublishingNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobHandler enterspeedJobHandler,
            IUmbracoContextFactory umbracoContextFactory,
            IContentService contentService,
            IScopeProvider scopeProvider)
        {
            _configurationService = configurationService;
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobHandler = enterspeedJobHandler;
            _umbracoContextFactory = umbracoContextFactory;
            _contentService = contentService;
            _scopeProvider = scopeProvider;
        }

        public void Handle(ContentPublishingNotification notification)
        {
            ContentServicePublishing(notification.PublishedEntities.ToList());
        }

        private bool IsConfigured()
        {
            return _configurationService.GetConfiguration().IsConfigured;
        }

        private void ContentServicePublishing(List<IContent> entities)
        {
            if (!IsConfigured())
            {
                return;
            }

            // This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    if (!content.ContentType.VariesByCulture())
                    {
                        continue;
                    }

                    List<IContent> descendants = null;

                    foreach (var culture in content.AvailableCultures)
                    {
                        var isCultureUnpublished = content.IsPropertyDirty(ContentBase.ChangeTrackingPrefix.UnpublishedCulture + culture);
                        if (isCultureUnpublished)
                        {
                            var now = DateTime.UtcNow;
                            jobs.Add(
                                new EnterspeedJob
                                {
                                    EntityId = content.Id.ToString(),
                                    EntityType = EnterspeedJobEntityType.Content,
                                    Culture = culture,
                                    JobType = EnterspeedJobType.Delete,
                                    State = EnterspeedJobState.Pending,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                });

                            if (descendants == null)
                            {
                                descendants = _contentService
                                    .GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                            }

                            foreach (var descendant in descendants)
                            {
                                if (descendant.ContentType.VariesByCulture())
                                {
                                    var descendantCultures = descendant.AvailableCultures;
                                    if (descendantCultures.Contains(culture))
                                    {
                                        jobs.Add(
                                            new EnterspeedJob
                                            {
                                                EntityId = descendant.Id.ToString(),
                                                EntityType = EnterspeedJobEntityType.Content,
                                                Culture = culture,
                                                JobType = EnterspeedJobType.Delete,
                                                State = EnterspeedJobState.Pending,
                                                CreatedAt = now,
                                                UpdatedAt = now,
                                            });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        private void EnqueueJobs(List<EnterspeedJob> jobs)
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

        private void HandleJobs(bool scopeCompleted, List<EnterspeedJob> jobs)
        {
            // Do not continue if the scope did not complete - the transaction may have been canceled and rolled back
            if (scopeCompleted)
            {
                _enterspeedJobHandler.HandleJobs(jobs);
            }
        }
    }
}