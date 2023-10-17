using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public sealed class EnterspeedPropertyDataMapperCollection : BuilderCollectionBase<IEnterspeedPropertyDataMapper>
    {
        public EnterspeedPropertyDataMapperCollection(IEnumerable<IEnterspeedPropertyDataMapper> items)
            : base(items)
        {
        }
    }
}