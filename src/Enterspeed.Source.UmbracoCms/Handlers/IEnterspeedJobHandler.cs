using Enterspeed.Source.UmbracoCms.Data.Models;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public interface IEnterspeedJobHandler
    {
        bool CanHandle(EnterspeedJob job);
        void Handle(EnterspeedJob job);
    }
}