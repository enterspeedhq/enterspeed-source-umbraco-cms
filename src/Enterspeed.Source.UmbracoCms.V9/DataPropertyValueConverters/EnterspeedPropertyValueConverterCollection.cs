using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V9.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.DataPropertyValueConverters
{
    public class EnterspeedPropertyValueConverterCollection : BuilderCollectionBase<IEnterspeedPropertyValueConverter>
    {
        public EnterspeedPropertyValueConverterCollection(Func<IEnumerable<IEnterspeedPropertyValueConverter>> items) 
            : base(items)
        {

        }
    }
}
