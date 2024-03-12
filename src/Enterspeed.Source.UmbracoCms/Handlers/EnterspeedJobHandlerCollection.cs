﻿using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.Handlers
{
    public class EnterspeedJobHandlerCollection : BuilderCollectionBase<IEnterspeedJobHandler>
    {
        public EnterspeedJobHandlerCollection(Func<IEnumerable<IEnterspeedJobHandler>> items) : base(items)
        {
        }
    }
}
