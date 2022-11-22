using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Guards;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Services
{
    public class EnterspeedGuardService : IEnterspeedGuardService
    {
        private readonly ILogger _logger;
        private readonly EnterspeedContentHandlingGuardCollection _contentGuards;
        private readonly EnterspeedDictionaryItemHandlingGuardCollection _dictionaryItemGuards;
        private readonly EnterspeedMediaHandlingGuardCollection _mediaHandlingGuards;

        public EnterspeedGuardService(
            EnterspeedContentHandlingGuardCollection contentGuards,
            EnterspeedDictionaryItemHandlingGuardCollection dictionaryItemGuards,
            ILogger logger,
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

            _logger.Debug<EnterspeedGuardService>(
                "Content {contentId} with {culture} culture, ingest avoided by '{guard}'.",
                content.Id,
                culture,
                blockingGuard.GetType().Name);
            return false;
        }

        public bool CanIngest(IDictionaryItem dictionaryItem, string culture)
        {
            var blockingGuard =
                _dictionaryItemGuards.FirstOrDefault(guard => !guard.CanIngest(dictionaryItem, culture));
            if (blockingGuard == null)
            {
                return true;
            }

            _logger.Debug<EnterspeedGuardService>(
                "Dictionary item {dictionaryItemId} with {culture} culture, ingest avoided by '{guard}'.",
                dictionaryItem.Id,
                culture,
                blockingGuard.GetType().Name);
            return false;
        }

        public bool CanIngest(IMedia media, string culture)
        {
            var blockingGuard = _mediaHandlingGuards.FirstOrDefault(guard => !guard.CanIngest(media, culture));
            if (blockingGuard == null)
            {
                return true;
            }

            _logger.Debug<EnterspeedGuardService>(
                "Media {mediaId} with {culture} culture, ingest avoided by '{guard}'.",
                media.Id,
                culture,
                blockingGuard.GetType().Name);
            return false;
        }
    }
}