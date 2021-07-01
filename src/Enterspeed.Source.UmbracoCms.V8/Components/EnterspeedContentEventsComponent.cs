using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Handlers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Changes;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Enterspeed.Source.UmbracoCms.V8.Components
{
    public class EnterspeedContentEventsComponent : IComponent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;
        private readonly IEnterspeedConfigurationService _configurationService;
        private readonly IScopeProvider _scopeProvider;

        public EnterspeedContentEventsComponent(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobHandler enterspeedJobHandler,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _enterspeedJobRepository = enterspeedJobRepository;
            _enterspeedJobHandler = enterspeedJobHandler;
            _configurationService = configurationService;
            _scopeProvider = scopeProvider;
        }

        public void Initialize()
        {
            ContentCacheRefresher.CacheUpdated += ContentCacheUpdated;
            ContentService.Publishing += ContentServicePublishing;
            ContentService.Unpublishing += ContentServiceUnpublishing;
            ContentService.Trashing += ContentServiceTrashing;
        }

        private void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var entities = e.MoveInfoCollection.Select(x => x.Entity).ToList();
            HandleUnpublishing(entities);
        }

        private void ContentServiceUnpublishing(IContentService sender, PublishEventArgs<IContent> e)
        {
            var entities = e.PublishedEntities.ToList();
            HandleUnpublishing(entities);
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
                        jobs.Add(new EnterspeedJob
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
                            descendants = Current.Services.ContentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                        }

                        foreach (var descendant in descendants)
                        {
                            var descendantCultures = descendant.ContentType.VariesByCulture()
                                ? descendant.AvailableCultures
                                : new List<string> { GetDefaultCulture(context) };
                            foreach (var descendantCulture in descendantCultures)
                            {
                                jobs.Add(new EnterspeedJob
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

        private void ContentServicePublishing(IContentService sender, ContentPublishingEventArgs e)
        {
            if (!IsConfigured())
            {
                return;
            }

            // This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method
            var entities = e.PublishedEntities.ToList();
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
                        var isCultureUnpublished = e.IsUnpublishingCulture(content, culture);

                        if (isCultureUnpublished)
                        {
                            var now = DateTime.UtcNow;
                            jobs.Add(new EnterspeedJob
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
                                descendants = Current.Services.ContentService
                                    .GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                            }

                            foreach (var descendant in descendants)
                            {
                                if (descendant.ContentType.VariesByCulture())
                                {
                                    var descendantCultures = descendant.AvailableCultures;
                                    if (descendantCultures.Contains(culture))
                                    {
                                        jobs.Add(new EnterspeedJob
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

        private void ContentCacheUpdated(ContentCacheRefresher sender, CacheRefresherEventArgs e)
        {
            if (!IsConfigured())
            {
                return;
            }

            var jsonPayloads = e.MessageObject as ContentCacheRefresher.JsonPayload[];

            if (jsonPayloads == null || !jsonPayloads.Any())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var umb = context.UmbracoContext;
                foreach (var payload in jsonPayloads)
                {
                    var node = umb.Content.GetById(payload.Id);
                    var savedNode = umb.Content.GetById(true, payload.Id);
                    if (node == null || savedNode == null)
                    {
                        continue;
                    }

                    var cultures = node.ContentType.VariesByCulture()
                        ? node.Cultures.Keys
                        : new List<string> { GetDefaultCulture(context) };

                    List<IPublishedContent> descendants = null;

                    foreach (var culture in cultures)
                    {
                        var publishedUpdateDate = node.CultureDate(culture);
                        var savedUpdateDate = savedNode.CultureDate(culture);

                        if (savedUpdateDate > publishedUpdateDate)
                        {
                            // This means that the nodes was only saved, so we skip creating any jobs for this node and culture
                            continue;
                        }

                        var now = DateTime.UtcNow;
                        jobs.Add(new EnterspeedJob
                        {
                            EntityId = node.Id.ToString(),
                            EntityType = EnterspeedJobEntityType.Content,
                            Culture = culture,
                            JobType = EnterspeedJobType.Publish,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });

                        if (payload.ChangeTypes == TreeChangeTypes.RefreshBranch)
                        {
                            if (descendants == null)
                            {
                                descendants = node.Descendants("*").ToList();
                            }

                            foreach (var descendant in descendants)
                            {
                                var descendantCultures = descendant.ContentType.VariesByCulture()
                                    ? descendant.Cultures.Keys
                                    : new List<string> { GetDefaultCulture(context) };

                                foreach (var descendantCulture in descendantCultures)
                                {
                                    jobs.Add(new EnterspeedJob
                                    {
                                        EntityId = descendant.Id.ToString(),
                                        EntityType = EnterspeedJobEntityType.Content,
                                        Culture = descendantCulture,
                                        JobType = EnterspeedJobType.Publish,
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

            EnqueueJobs(jobs);
        }

        private string GetDefaultCulture(UmbracoContextReference context)
        {
            return context.UmbracoContext.Domains.DefaultCulture.ToLowerInvariant();
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

        private bool IsConfigured()
        {
            return _configurationService.GetConfiguration().IsConfigured;
        }

        public void Terminate()
        {
        }
    }
}
