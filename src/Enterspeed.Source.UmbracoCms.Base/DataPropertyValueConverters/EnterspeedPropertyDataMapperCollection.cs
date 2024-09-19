using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Base.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Base.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyDataMapperCollection : BuilderCollectionBase<IEnterspeedPropertyDataMapper>
    {
        public EnterspeedPropertyDataMapperCollection(Func<IEnumerable<IEnterspeedPropertyDataMapper>> items)
            : base(items)
        {
        }
    }
}