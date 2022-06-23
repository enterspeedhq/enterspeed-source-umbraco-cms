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
    public class EnterspeedMediaItemDeletingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedMediaItemDeletingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory)
            : base(
                umbracoContextFactory,
                enterspeedJobRepository,
                jobsHandlingService,
                configurationService,
                scopeProvider)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Initialize()
        {
            MediaService.Trashed += MediaService_Trashed;
        }

        private void MediaService_Trashed(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();

            if (!isPublishConfigured)
            {
                return;
            }

            var entities = e.MoveInfoCollection.ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var mediaItem in entities.Select(ei => ei.Entity))
                {
                    jobs.Add(_enterspeedJobFactory.GetDeleteJob(mediaItem, string.Empty, EnterspeedContentState.Publish));
                }
            }

            EnqueueJobs(jobs);
        }

        public void Terminate()
        {
        }
    }
}