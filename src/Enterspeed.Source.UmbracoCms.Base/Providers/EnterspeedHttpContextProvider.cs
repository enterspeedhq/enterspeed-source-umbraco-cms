using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Enterspeed.Source.UmbracoCms.Base.Providers
{
    public class EnterspeedHttpContextProvider : IEnterspeedHttpContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public EnterspeedHttpContextProvider(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public IDisposable CreateFakeHttpContext()
        {
            var httpContext = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("enterspeed.com")
                }
            };

            var scope = _serviceProvider.CreateScope();
            httpContext.RequestServices = scope.ServiceProvider;

            var originalContext = _httpContextAccessor.HttpContext;
            _httpContextAccessor.HttpContext = httpContext;

            return new HttpContextScope(_httpContextAccessor, originalContext, scope);
        }

        private class HttpContextScope : IDisposable
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly HttpContext _originalContext;
            private readonly IServiceScope _scope;

            public HttpContextScope(
                IHttpContextAccessor httpContextAccessor,
                HttpContext originalContext,
                IServiceScope scope)
            {
                _httpContextAccessor = httpContextAccessor;
                _originalContext = originalContext;
                _scope = scope;
            }

            public void Dispose()
            {
                _httpContextAccessor.HttpContext = _originalContext;
                _scope.Dispose();
            }
        }
    }
}
