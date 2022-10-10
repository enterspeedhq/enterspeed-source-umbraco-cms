using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedMediaMovedEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private const int IndexPageSize = 9999;
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;
        private readonly IMediaService _mediaService;

        public EnterspeedMediaMovedEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IMediaService mediaService)
            : base(
                umbracoContextFactory,
                enterspeedJobRepository,
                jobsHandlingService,
                configurationService,
                scopeProvider)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
            _mediaService = mediaService;
        }

        public void Initialize()
        {
            MediaService.Moved += MediaService_Moved;
        }

        private void MediaService_Moved(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var entities = e.MoveInfoCollection.Select(ei => ei.Entity).ToList();
            var jobs = new List<EnterspeedJob>();

            foreach (var mediaItem in entities)
            {
                if (mediaItem.ContentType.Alias.Equals("Folder"))
                {
                    var mediaItems = _mediaService.GetPagedDescendants(mediaItem.Id, 0, IndexPageSize, out var totalRecords).ToList();
                    if (totalRecords > 0)
                    {
                        foreach (var item in mediaItems)
                        {
                            jobs.Add(_enterspeedJobFactory.GetPublishJob(item, string.Empty, EnterspeedContentState.Publish));
                        }
                    }
                }

                jobs.Add(_enterspeedJobFactory.GetPublishJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
            }

            EnqueueJobs(jobs);
        }

        public void Terminate()
        {
        }
    }
}