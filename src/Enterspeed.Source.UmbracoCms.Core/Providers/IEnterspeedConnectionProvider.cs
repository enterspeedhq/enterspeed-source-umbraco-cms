using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Core.Providers
{
    public interface IEnterspeedConnectionProvider
    {
        IEnterspeedConnection GetConnection(ConnectionType type);
        void Initialize();
    }
}
