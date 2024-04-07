using System;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Enterspeed.Source.UmbracoCms.V14.Configuration
{
    public class EnterspeedSchemaIdSelector : SchemaIdSelector
    {
        public override string SchemaId(Type type)
        {
            if (type.Namespace?.Contains("Enterspeed") is false)
            {
                return base.SchemaId(type);
            }

            if (!type.IsGenericType)
            {
                return base.SchemaId(type);
            }
            
            return type.ToString().Replace("Enterspeed.Source.UmbracoCms.V14.Models.", "");
        }
    }
}
