using Enterspeed.Source.UmbracoCms.Base.Composers;
using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.Base.NotificationHandlers;
using Enterspeed.Source.UmbracoCms.V9Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Enterspeed.Source.UmbracoCms.V9Plus
{
    public class EnterspeedComposer : EnterspeedBaseComposer
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
            builder.AddNotificationHandler<ContentMovedToRecycleBinNotification, EnterspeedContentUnpublishingNotificationHandler>();
            builder.AddNotificationHandler<ContentUnpublishingNotification, EnterspeedContentUnpublishingNotificationHandler>();
            base.Compose(builder);
        }
    }
}