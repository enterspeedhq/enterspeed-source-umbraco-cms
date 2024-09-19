using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyMetaDataMapperCollection : BuilderCollectionBase<IEnterspeedPropertyMetaDataMapper>
    {
        public EnterspeedPropertyMetaDataMapperCollection(Func<IEnumerable<IEnterspeedPropertyMetaDataMapper>> items)
            : base(items)
        {
        }
    }
}