using System.Linq;
using Enterspeed.Source.UmbracoCms.V10.Guards;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V10.Services
{
    public class EnterspeedGuardService : IEnterspeedGuardService
    {
        private readonly ILogger<EnterspeedGuardService> _logger;
        private readonly EnterspeedContentHandlingGuardCollection _contentGuards;
        private readonly EnterspeedDictionaryItemHandlingGuardCollection _dictionaryItemGuards;

        public EnterspeedGuardService(
            EnterspeedContentHandlingGuardCollection contentGuards,
            EnterspeedDictionaryItemHandlingGuardCollection dictionaryItemGuards, 
            ILogger<EnterspeedGuardService> logger)
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
    }
}