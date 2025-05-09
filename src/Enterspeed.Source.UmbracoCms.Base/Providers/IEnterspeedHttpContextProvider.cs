using System;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public interface IEnterspeedHttpContextProvider
    {
        IDisposable CreateFakeHttpContext();
    }
}