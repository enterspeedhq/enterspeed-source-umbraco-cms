using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Core.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Core.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyDataMapperCollection : BuilderCollectionBase<IEnterspeedPropertyDataMapper>
    {
        public EnterspeedPropertyDataMapperCollection(Func<IEnumerable<IEnterspeedPropertyDataMapper>> items)
            : base(items)
        {
        }
    }
}