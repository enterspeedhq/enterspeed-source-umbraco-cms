using Enterspeed.Source.UmbracoCms.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.Composers
{
    public abstract class EnterspeedBaseComposer : IComposer
    {
        public virtual void Compose(IUmbracoBuilder builder)
        {
            builder.AddEnterspeed();
        }
    }
}