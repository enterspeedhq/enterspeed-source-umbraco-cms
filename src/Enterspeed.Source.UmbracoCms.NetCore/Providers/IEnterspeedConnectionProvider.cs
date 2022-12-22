using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.Models;

namespace Enterspeed.Source.UmbracoCms.Providers
{
    public interface IEnterspeedConnectionProvider
    {
        IEnterspeedConnection GetConnection(ConnectionType type);
        void Initialize();
    }
}
