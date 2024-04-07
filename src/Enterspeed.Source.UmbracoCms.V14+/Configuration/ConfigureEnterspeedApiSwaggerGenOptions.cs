using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace Enterspeed.Source.UmbracoCms.V14.Configuration
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
