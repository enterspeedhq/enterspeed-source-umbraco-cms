using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Configuration
{
    public class ConfigureEnterspeedApiSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("enterspeed", new OpenApiInfo
            {
                Version = "Latest",
                Title = "Enterspeed API",
                Description = "Enterspeed API",
            });
        }
    }
}
