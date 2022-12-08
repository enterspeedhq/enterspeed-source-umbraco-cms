using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.DataPropertyValueConverters
{
    public class EnterspeedPropertyValueConverterCollection : BuilderCollectionBase<IEnterspeedPropertyValueConverter>
    {
        public EnterspeedPropertyValueConverterCollection(Func<IEnumerable<IEnterspeedPropertyValueConverter>> items)
            : base(items)
        {

        }
    }
}
