using Enterspeed.Source.UmbracoCms.V10.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V10.Handlers
{
    public interface IEnterspeedJobHandler
    {
        bool CanHandle(EnterspeedJob job);
        void Handle(EnterspeedJob job);
    }
}