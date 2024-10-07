using Enterspeed.Source.UmbracoCms.Base.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Handlers
{
    public interface IEnterspeedJobHandler
    {
        bool CanHandle(EnterspeedJob job);
        void Handle(EnterspeedJob job);
    }
}