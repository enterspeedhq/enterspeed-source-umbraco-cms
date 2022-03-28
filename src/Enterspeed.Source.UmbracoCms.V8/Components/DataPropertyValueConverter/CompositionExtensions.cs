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

        public static EnterspeedGridEditorValueConverterCollectionBuilder EnterspeedGridEditorValueConverters(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedGridEditorValueConverterCollectionBuilder>();

        public static EnterspeedContentHandlingGuardCollectionBuilder EnterspeedContentHandlingGuards(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedContentHandlingGuardCollectionBuilder>();

        public static EnterspeedDictionaryItemHandlingGuardCollectionBuilder EnterspeedDictionaryItemHandlingGuards(
            this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedDictionaryItemHandlingGuardCollectionBuilder>();

        public static EnterspeedJobHandlerCollectionBuilder EnterspeedJobHandlers(this Composition composition)
            => composition.WithCollectionBuilder<EnterspeedJobHandlerCollectionBuilder>();
    }
}