using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V8.Factories
{
    public interface IEnterspeedJobFactory
    {
        EnterspeedJob GetPublishJob(IPublishedContent content, string culture, EnterspeedContentState state);
        EnterspeedJob GetPublishJob(IContent content, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteJob(IContent content, string culture, EnterspeedContentState state);
        EnterspeedJob GetPublishJob(IDictionaryItem dictionaryItem, string culture, EnterspeedContentState state);
        EnterspeedJob GetPublishJob(IMedia media, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteJob(IDictionaryItem dictionaryItem, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteJob(IMedia media, string culture, EnterspeedContentState state);

        EnterspeedJob GetFailedJob(EnterspeedJob job, string exception);
    }
}
