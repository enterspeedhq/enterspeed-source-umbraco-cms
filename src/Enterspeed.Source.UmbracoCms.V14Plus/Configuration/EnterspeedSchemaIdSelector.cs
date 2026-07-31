using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Configuration
{
    public class EnterspeedSchemaIdSelector : SchemaIdSelector
    {
        public EnterspeedSchemaIdSelector(IEnumerable<ISchemaIdHandler> schemaIdHandlers) : base(schemaIdHandlers)
        {
        }

        public override string SchemaId(Type type)
        {
            if (type.Namespace?.Contains("Enterspeed") is false)
            {
                return base.SchemaId(type);
            }

            if (!type.IsGenericType || (type.IsGenericType && !type.GenericTypeArguments.Any()))
            {
                return base.SchemaId(type);
            }

            // Match the schema id format Umbraco 18's UmbracoSchemaIdGenerator produces
            // (e.g. ApiResponseOfListOfEnterspeedJob), so the 17 and 18 documents
            // generate identical clients
            return FriendlyName(type);
        }

        private static string FriendlyName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var name = type.Name[..type.Name.IndexOf('`')];
            return $"{name}Of{string.Join(string.Empty, type.GenericTypeArguments.Select(FriendlyName))}";
        }
    }
}
