using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Services.DataProperties;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public class EnterspeedPropertyValueConverterCollection : BuilderCollectionBase<IEnterspeedPropertyValueConverter>
    {
        public EnterspeedPropertyValueConverterCollection(IEnumerable<IEnterspeedPropertyValueConverter> items)
            : base(items)
        {
        }
    }
}
