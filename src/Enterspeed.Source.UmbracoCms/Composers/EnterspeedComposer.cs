using Enterspeed.Source.UmbracoCms.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.Composers
{
    public class EnterspeedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddEnterspeed();
        }
    }
}