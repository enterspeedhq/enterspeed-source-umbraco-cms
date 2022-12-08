using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.NetCore.Models;

namespace Enterspeed.Source.UmbracoCms.NetCore.Providers
{
    public interface IEnterspeedConnectionProvider
    {
        IEnterspeedConnection GetConnection(ConnectionType type);
        void Initialize();
    }
}
