﻿using Umbraco.Cms.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.NetCore.Handlers
{
    public class EnterspeedJobHandlerCollectionBuilder : OrderedCollectionBuilderBase<EnterspeedJobHandlerCollectionBuilder, EnterspeedJobHandlerCollection,
            IEnterspeedJobHandler>
    {
        public EnterspeedJobHandlerCollectionBuilder()
        {
        }

        protected override EnterspeedJobHandlerCollectionBuilder This => this;
    }
}
