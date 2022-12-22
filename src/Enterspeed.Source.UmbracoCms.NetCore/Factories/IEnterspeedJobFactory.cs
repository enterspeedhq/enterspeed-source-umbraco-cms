using Enterspeed.Source.UmbracoCms.Data.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Factories
{
    public interface IEnterspeedJobFactory
    {
        EnterspeedJob GetPublishJob(IPublishedContent content, string culture, EnterspeedContentState state);
        EnterspeedJob GetPublishJob(IContent content, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteJob(IContent content, string culture, EnterspeedContentState state);
        EnterspeedJob GetPublishJob(IDictionaryItem dictionaryItem, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteJob(IDictionaryItem dictionaryItem, string culture, EnterspeedContentState state);
        EnterspeedJob GetFailedJob(EnterspeedJob job, string exception);
        EnterspeedJob GetPublishJob(IMedia media, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteJob(IMedia media, string culture, EnterspeedContentState state);
    }
}
