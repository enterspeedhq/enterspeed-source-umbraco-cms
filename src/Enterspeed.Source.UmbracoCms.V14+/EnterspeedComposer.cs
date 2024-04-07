using Enterspeed.Source.UmbracoCms.Models;
using Enterspeed.Source.UmbracoCms.V14.Configuration;
using Enterspeed.Source.UmbracoCmsV14.Models;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V14
{
    public class EnterspeedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IEnterspeedDictionaryTranslation, EnterspeedDictionaryTranslation>();
            builder.Services.AddSingleton<ISchemaIdSelector, EnterspeedSchemaIdSelector>();
            builder.Services.ConfigureOptions<ConfigureEnterspeedApiSwaggerGenOptions>();
        }
    }
}
