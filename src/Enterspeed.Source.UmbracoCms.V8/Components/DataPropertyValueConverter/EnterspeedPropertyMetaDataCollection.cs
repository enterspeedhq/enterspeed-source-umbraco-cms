using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public sealed class EnterspeedPropertyMetaDataCollection : BuilderCollectionBase<IEnterspeedPropertyMetaDataService>
    {
        public EnterspeedPropertyMetaDataCollection(IEnumerable<IEnterspeedPropertyMetaDataService> items)
            : base(items)
        {
        }
    }
}