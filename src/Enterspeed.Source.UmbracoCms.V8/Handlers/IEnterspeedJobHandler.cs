using Enterspeed.Source.UmbracoCms.V8.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Handlers
{
    public interface IEnterspeedJobHandler
    {
        bool CanHandle(EnterspeedJob job);
        void Handle(EnterspeedJob job);
    }
}