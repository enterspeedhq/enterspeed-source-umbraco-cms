using Enterspeed.Source.UmbracoCms.V8.Guards;
using Enterspeed.Source.UmbracoCms.V8.Handlers;
using Umbraco.Core.Composing;

namespace Enterspeed.Source.UmbracoCms.V8.Components.DataPropertyValueConverter
{
    public static class CompositionExtensions
    {
        public static EnterspeedPropertyValueConverterCollectionBuilder EnterspeedPropertyValueConverters(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedPropertyValueConverterCollectionBuilder>();

        public static EnterspeedPropertyMetaDataCollectionBuilder EnterspeedPropertyMetaDataServices(this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedPropertyMetaDataCollectionBuilder>();

        public static EnterspeedGridEditorValueConverterCollectionBuilder EnterspeedGridEditorValueConverters(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedGridEditorValueConverterCollectionBuilder>();

        public static EnterspeedContentHandlingGuardCollectionBuilder EnterspeedContentHandlingGuards(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedContentHandlingGuardCollectionBuilder>();

        public static EnterspeedDictionaryItemHandlingGuardCollectionBuilder EnterspeedDictionaryItemHandlingGuards(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedDictionaryItemHandlingGuardCollectionBuilder>();

        public static EnterspeedMediaHandlingGuardCollectionBuilder EnterspeedMediaHandlingGuards(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedMediaHandlingGuardCollectionBuilder>();

        public static EnterspeedJobHandlerCollectionBuilder EnterspeedJobHandlers(this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedJobHandlerCollectionBuilder>();
    }
}