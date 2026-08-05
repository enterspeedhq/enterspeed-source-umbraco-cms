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
    public class ConfigureEnterspeedApiSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>, IPostConfigureOptions<SwaggerGenOptions>
    {
        private readonly EnterspeedSchemaIdSelector _schemaIdSelector;
        private readonly EnterspeedOperationIdSelector _operationIdSelector;

        public ConfigureEnterspeedApiSwaggerGenOptions(
            EnterspeedSchemaIdSelector schemaIdSelector,
            EnterspeedOperationIdSelector operationIdSelector)
        {
            _schemaIdSelector = schemaIdSelector;
            _operationIdSelector = operationIdSelector;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("enterspeed", new OpenApiInfo
            {
                Version = "Latest",
                Title = "Enterspeed API",
                Description = "Enterspeed API",
            });
        }

        // Umbraco resolves its own ISchemaIdSelector/IOperationIdSelector when it configures
        // SwaggerGenOptions, and depending on composer order the Enterspeed selectors can lose
        // that registration race (observed on Umbraco 17.5.x: generic ApiResponse<T> types then
        // collide on schema id and the enterspeed swagger document 500s). PostConfigure always
        // runs after every Configure, so this applies the Enterspeed selectors deterministically.
        // Non-Enterspeed types/actions still delegate to the Umbraco base selectors.
        public void PostConfigure(string name, SwaggerGenOptions options)
        {
            options.CustomSchemaIds(_schemaIdSelector.SchemaId);
            options.CustomOperationIds(_operationIdSelector.OperationId);
        }
    }
}
