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

            var schemaId = $"{type.Name[..^2]}<{string.Join(",", type.GenericTypeArguments.Select(x => x.Name))}>";
            return schemaId;
        }
    }
}
