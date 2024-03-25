using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCmsV14.Models;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V14
{
    public class EnterspeedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
        }
    }
}
