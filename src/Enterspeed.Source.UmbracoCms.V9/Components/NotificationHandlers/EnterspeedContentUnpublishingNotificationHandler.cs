using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Enterspeed.Source.UmbracoCms.V9.Components.NotificationHandlers
{
    public class EnterspeedContentUnpublishingNotificationHandler
        : INotificationHandler<ContentUnpublishingNotification>,
            INotificationHandler<ContentMovedToRecycleBinNotification>
    {
        private readonly IEnterspeedConfigurationService _configurationService;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IContentService _contentService;
        private readonly IScopeProvider _scopeProvider;

        public void Handle(ContentUnpublishingNotification notification)
        {
            var entities = notification.UnpublishedEntities.ToList();
            HandleUnpublishing(entities);
        }

        public void Handle(ContentMovedToRecycleBinNotification notification)
        {
            var entities = notification.MoveInfoCollection.Select(x => x.Entity).ToList();
            HandleUnpublishing(entities);
        }

        private bool IsConfigured()
        {
            return _configurationService.GetConfiguration().IsConfigured;
        }

        private void HandleJobs(bool scopeCompleted, List<EnterspeedJob> jobs)
        {
            // Do not continue if the scope did not complete - the transaction may have been canceled and rolled back
            if (scopeCompleted)
            {
                _enterspeedJobHandler.HandleJobs(jobs);
            }
        }

        private string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
        }

        private void HandleUnpublishing(List<IContent> entities)
        {
            if (!IsConfigured())
            {
                return;
            }

            if (!entities.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    var cultures = content.ContentType.VariesByCulture()
                        ? content.AvailableCultures
                        : new List<string> { GetDefaultCulture(context) };

                    List<IContent> descendants = null;
                    foreach (var culture in cultures)
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
                            descendants = _contentService.GetPagedDescendants(
                                content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            var descendantCultures = descendant.ContentType.VariesByCulture()
                                ? descendant.AvailableCultures
                                : new List<string> { GetDefaultCulture(context) };
                            foreach (var descendantCulture in descendantCultures)
                            {
                                jobs.Add(
                                    new EnterspeedJob
                                    {
                                        EntityId = descendant.Id.ToString(),
                                        EntityType = EnterspeedJobEntityType.Content,
                                        Culture = descendantCulture,
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
    }
}