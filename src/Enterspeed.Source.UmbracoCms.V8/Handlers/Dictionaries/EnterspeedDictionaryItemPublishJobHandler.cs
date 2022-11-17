﻿using System;
using System.Web.Helpers;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Exceptions;
using Enterspeed.Source.UmbracoCms.V8.Models;
using Enterspeed.Source.UmbracoCms.V8.Models.Api;
using Enterspeed.Source.UmbracoCms.V8.Providers;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public class EnterspeedDictionaryItemPublishJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly IEnterspeedGuardService _enterspeedGuardService;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public EnterspeedDictionaryItemPublishJobHandler(
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            IEnterspeedGuardService enterspeedGuardService,
            ILocalizationService localizationService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider)
        {
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _enterspeedGuardService = enterspeedGuardService;
            _localizationService = localizationService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return
                _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish) != null
                && job.EntityType == EnterspeedJobEntityType.Dictionary
                && job.ContentState == EnterspeedContentState.Publish
                && job.JobType == EnterspeedJobType.Publish;
        }

        public void Handle(EnterspeedJob job)
        {
            var dictionaryItem = GetDictionaryItem(job);
            if (!CanIngest(dictionaryItem, job))
            {
                return;
            }

            var umbracoData = CreateUmbracoDictionaryEntity(dictionaryItem, job);
            Ingest(umbracoData, job);
        }

        protected virtual IDictionaryItem GetDictionaryItem(EnterspeedJob job)
        {
            var isDictionaryId = Guid.TryParse(job.EntityId, out var dictionaryId);
            var dictionaryItem = isDictionaryId
                ? _localizationService.GetDictionaryItemById(dictionaryId)
                : null;
            if (dictionaryItem == null)
            {
                throw new JobHandlingException($"Dictionary with id {job.EntityId} not in database");
            }
            return dictionaryItem;
        }

        protected virtual bool CanIngest(IDictionaryItem dictionaryItem, EnterspeedJob job)
        {
            // Check if any of guards are against it
            return _enterspeedGuardService.CanIngest(dictionaryItem, job.Culture);
        }

        protected virtual UmbracoDictionaryEntity CreateUmbracoDictionaryEntity(IDictionaryItem dictionaryItem, EnterspeedJob job)
        {
            try
            {
                return new UmbracoDictionaryEntity(
                    dictionaryItem, _enterspeedPropertyService, _entityIdentityService, job.Culture);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}/{job.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        protected virtual void Ingest(IEnterspeedEntity umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _enterspeedConnectionProvider.GetConnection(ConnectionType.Publish));
            if (!ingestResponse.Success)
            {
                var message = Json.Encode(new ErrorResponse(ingestResponse));
                throw new JobHandlingException($"Failed ingesting entity ({job.EntityId}/{job.Culture}). Message: {message}");
            }
        }
    }
}
