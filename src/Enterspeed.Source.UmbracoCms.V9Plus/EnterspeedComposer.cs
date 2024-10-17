using Enterspeed.Source.UmbracoCms.Base.Composers;
using Enterspeed.Source.UmbracoCms.Base.Models;
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
