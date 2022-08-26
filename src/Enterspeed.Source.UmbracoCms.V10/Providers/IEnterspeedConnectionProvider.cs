using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.V10.Models;

namespace Enterspeed.Source.UmbracoCms.V10.Providers
{
    public interface IEnterspeedConnectionProvider
    {
        IEnterspeedConnection GetConnection(ConnectionType type);
        void Initialize();
    }
}
