using Enterspeed.Source.UmbracoCms.Base.Data.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.Base.Factories
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
        EnterspeedJob GetDictionaryItemRootJob(string culture, EnterspeedContentState contentState);
        EnterspeedJob GetPublishMasterContentJob(string nodeId, string culture, EnterspeedContentState state);
        EnterspeedJob GetDeleteMasterContentJob(string nodeId, string culture, EnterspeedContentState state);
    }
}