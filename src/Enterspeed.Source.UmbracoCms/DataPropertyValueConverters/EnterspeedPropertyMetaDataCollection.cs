using System;
using System.Collections.Generic;
using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.DataPropertyValueConverters
{
    public sealed class EnterspeedPropertyMetaDataCollection : BuilderCollectionBase<IEnterspeedPropertyMetaDataService>
    {
        public EnterspeedPropertyMetaDataCollection(Func<IEnumerable<IEnterspeedPropertyMetaDataService>> items)
            : base(items)
        {
        }
    }
}