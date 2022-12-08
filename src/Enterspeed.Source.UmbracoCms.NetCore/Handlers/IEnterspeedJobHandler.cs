using Enterspeed.Source.UmbracoCms.NetCore.Data.Models;

namespace Enterspeed.Source.UmbracoCms.NetCore.Handlers
{
    public interface IEnterspeedJobHandler
    {
        bool CanHandle(EnterspeedJob job);
        void Handle(EnterspeedJob job);
    }
}