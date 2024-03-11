using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Core.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Core.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyMetaDataMapperCollection : BuilderCollectionBase<IEnterspeedPropertyMetaDataMapper>
    {
        public EnterspeedPropertyMetaDataMapperCollection(Func<IEnumerable<IEnterspeedPropertyMetaDataMapper>> items)
            : base(items)
        {
        }
    }
}