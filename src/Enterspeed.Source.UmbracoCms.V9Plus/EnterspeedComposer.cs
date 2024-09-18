using Enterspeed.Source.UmbracoCms.Base.Models;
using Enterspeed.Source.UmbracoCms.V9Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V9Plus
{
    public class EnterspeedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
        }
    }
}
    