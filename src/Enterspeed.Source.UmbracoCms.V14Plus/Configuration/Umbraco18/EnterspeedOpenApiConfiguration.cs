#if UMBRACO_18_OR_GREATER
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Configuration.Umbraco18
{
    /// <summary>
    /// Umbraco 18 replaced Swashbuckle with Microsoft.AspNetCore.OpenApi. This registers the
    /// "enterspeed" OpenAPI document (served at /umbraco/openapi/enterspeed.json) that the
    /// Swashbuckle-based configuration used to provide on Umbraco 14-17. Schema and operation
    /// ids use the Umbraco defaults; the generated TypeScript client is regenerated from this
    /// document, so the previous custom id selectors are not carried over.
    /// </summary>
    public static class EnterspeedOpenApiConfiguration
    {
        public static IUmbracoBuilder AddEnterspeedOpenApiDocument(this IUmbracoBuilder builder)
        {
            // The document name matches [MapToApi("enterspeed")] on the API controllers
            return builder.AddBackOfficeOpenApiDocument(
                "enterspeed",
                documentBuilder => documentBuilder
                    .WithTitle("Enterspeed API")
                    .WithJsonOptions(Constants.JsonOptionsNames.BackOffice)
                    .ConfigureOpenApiOptions(options => options.AddBackofficeSecurityRequirements()));
        }
    }
}
#endif
