using Enterspeed.Source.Sdk.Api.Connection;
using Enterspeed.Source.UmbracoCms.Base.Models;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public interface IEnterspeedConnectionProvider
    {
        IEnterspeedConnection GetConnection(ConnectionType type);
        void Initialize();
    }
}
