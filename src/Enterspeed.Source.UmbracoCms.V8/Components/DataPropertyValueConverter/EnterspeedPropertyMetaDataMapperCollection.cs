using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public sealed class EnterspeedPropertyMetaDataMapperCollection : BuilderCollectionBase<IEnterspeedPropertyMetaDataMapper>
    {
        public EnterspeedPropertyMetaDataMapperCollection(IEnumerable<IEnterspeedPropertyMetaDataMapper> items)
            : base(items)
        {
        }
    }
}