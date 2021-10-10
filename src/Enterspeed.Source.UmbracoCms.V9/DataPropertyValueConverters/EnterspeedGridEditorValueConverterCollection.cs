using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V9.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V9.DataPropertyValueConverters
{
    public class
        EnterspeedGridEditorValueConverterCollection : BuilderCollectionBase<IEnterspeedGridEditorValueConverter>
    {
        public EnterspeedGridEditorValueConverterCollection(IEnumerable<IEnterspeedGridEditorValueConverter> items)
            : base(() => items)
        {
        }
    }
}