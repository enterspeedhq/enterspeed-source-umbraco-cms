using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Services.DataProperties;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public class EnterspeedGridEditorValueConverterCollection : BuilderCollectionBase<IEnterspeedGridEditorValueConverter>
    {
        public EnterspeedGridEditorValueConverterCollection(IEnumerable<IEnterspeedGridEditorValueConverter> items)
            : base(items)
        {
        }
    }
}
