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

        public EnterspeedGuardService(
            EnterspeedContentHandlingGuardCollection contentGuards,
            EnterspeedDictionaryItemHandlingGuardCollection dictionaryItemGuards,
            ILogger logger)
        {
            _contentGuards = contentGuards;
            _dictionaryItemGuards = dictionaryItemGuards;
            _logger = logger;
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
    }
}