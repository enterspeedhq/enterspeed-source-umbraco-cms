using Enterspeed.Source.UmbracoCms.Composers;
using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.V9Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V9Plus
{
    public class EnterspeedComposer : EnterspeedBaseComposer
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
            base.Compose(builder);
        }
    }
}
