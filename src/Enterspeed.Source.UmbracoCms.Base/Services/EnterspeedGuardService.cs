using System.Linq;
using Enterspeed.Source.UmbracoCms.Base.Guards;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Services
{
    public class EnterspeedGuardService : IEnterspeedGuardService
    {
        private readonly ILogger<EnterspeedGuardService> _logger;
        private readonly EnterspeedContentHandlingGuardCollection _contentGuards;
        private readonly EnterspeedDictionaryItemHandlingGuardCollection _dictionaryItemGuards;
        private readonly EnterspeedMediaHandlingGuardCollection _mediaHandlingGuards;

        public EnterspeedGuardService(
            EnterspeedContentHandlingGuardCollection contentGuards,
            EnterspeedDictionaryItemHandlingGuardCollection dictionaryItemGuards,
            ILogger<EnterspeedGuardService> logger,
            EnterspeedMediaHandlingGuardCollection mediaHandlingGuards)
        {
            _contentGuards = contentGuards;
            _dictionaryItemGuards = dictionaryItemGuards;
            _logger = logger;
            _mediaHandlingGuards = mediaHandlingGuards;
        }

        public bool CanIngest(IPublishedContent content, string culture)
        {
            var blockingGuard = _contentGuards.FirstOrDefault(guard => !guard.CanIngest(content, culture));
            if (blockingGuard == null)
            {
                return true;
            }

            _logger.LogInformation("Content {contentId} with {culture} culture, ingest avoided by '{guard}'.", content.Id, culture, blockingGuard.GetType().Name);
            return false;
        }

        public bool CanIngest(IDictionaryItem dictionaryItem, string culture)
        {
            var blockingGuard = _dictionaryItemGuards.FirstOrDefault(guard => !guard.CanIngest(dictionaryItem, culture));
            if (blockingGuard == null)
            {
                return true;
            }

            _logger.LogInformation("Dictionary item {dictionaryItemId} with {culture} culture, ingest avoided by '{guard}'.", dictionaryItem.Id, culture, blockingGuard.GetType().Name);
            return false;
        }

        public bool CanIngest(IMedia media, string culture)
        {
            var blockingGuard = _mediaHandlingGuards.FirstOrDefault(guard => !guard.CanIngest(media, culture));
            if (blockingGuard == null)
            {
                return true;
            }

            _logger.LogDebug(
                "Media {mediaId} with {culture} culture, ingest avoided by '{guard}'.",
                media.Id,
                culture,
                blockingGuard.GetType().Name);
            return false;
        }
    }
}