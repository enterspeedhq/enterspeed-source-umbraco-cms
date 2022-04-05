using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public interface IEnterspeedJobHandler
    {
        bool CanHandle(EnterspeedJob job);
        void Handle(EnterspeedJob job);
    }
}