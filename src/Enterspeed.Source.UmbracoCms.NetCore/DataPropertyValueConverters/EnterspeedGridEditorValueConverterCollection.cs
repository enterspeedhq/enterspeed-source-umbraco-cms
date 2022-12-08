using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.NetCore.Services.DataProperties;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.DataPropertyValueConverters
{
    public class
        EnterspeedGridEditorValueConverterCollection : BuilderCollectionBase<IEnterspeedGridEditorValueConverter>
    {
        public EnterspeedGridEditorValueConverterCollection(Func<IEnumerable<IEnterspeedGridEditorValueConverter>> items)
            : base(items)
        {
        }
    }
}