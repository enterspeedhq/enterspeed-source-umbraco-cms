﻿using Enterspeed.Source.UmbracoCms.Guards;
using Enterspeed.Source.UmbracoCms.Handlers;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.DataPropertyValueConverters
{
    public static class CompositionExtensions
    {
        public static EnterspeedPropertyValueConverterCollectionBuilder EnterspeedPropertyValueConverters(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedPropertyValueConverterCollectionBuilder>();

        public static EnterspeedPropertyMetaDataMapperCollectionBuilder EnterspeedPropertyMetaDataMappers(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedPropertyMetaDataMapperCollectionBuilder>();

        public static EnterspeedGridEditorValueConverterCollectionBuilder EnterspeedGridEditorValueConverters(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedGridEditorValueConverterCollectionBuilder>();

        public static EnterspeedContentHandlingGuardCollectionBuilder EnterspeedContentHandlingGuards(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedContentHandlingGuardCollectionBuilder>();

        public static EnterspeedDictionaryItemHandlingGuardCollectionBuilder EnterspeedDictionaryItemHandlingGuards(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedDictionaryItemHandlingGuardCollectionBuilder>();

        public static EnterspeedJobHandlerCollectionBuilder EnterspeedJobHandlers(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedJobHandlerCollectionBuilder>();

        public static EnterspeedMediaHandlingGuardCollectionBuilder EnterspeedMediaHandlingGuards(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedMediaHandlingGuardCollectionBuilder>();
    }
}