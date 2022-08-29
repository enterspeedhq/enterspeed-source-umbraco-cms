using Enterspeed.Source.UmbracoCms.V10.Guards;
using Enterspeed.Source.UmbracoCms.V10.Handlers;
using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V10.DataPropertyValueConverters
{
    public static class CompositionExtensions
    {
        public static EnterspeedPropertyValueConverterCollectionBuilder EnterspeedPropertyValueConverters(this IUmbracoBuilder  composition)
            => composition.WithCollectionBuilder<EnterspeedPropertyValueConverterCollectionBuilder>();

        public static EnterspeedGridEditorValueConverterCollectionBuilder EnterspeedGridEditorValueConverters(this IUmbracoBuilder  composition)
            => composition.WithCollectionBuilder<EnterspeedGridEditorValueConverterCollectionBuilder>();
        
        public static EnterspeedContentHandlingGuardCollectionBuilder EnterspeedContentHandlingGuards(this IUmbracoBuilder  composition)
            => composition.WithCollectionBuilder<EnterspeedContentHandlingGuardCollectionBuilder>();
        
        public static EnterspeedDictionaryItemHandlingGuardCollectionBuilder EnterspeedDictionaryItemHandlingGuards(this IUmbracoBuilder  composition)
            => composition.WithCollectionBuilder<EnterspeedDictionaryItemHandlingGuardCollectionBuilder>();

        public static EnterspeedJobHandlerCollectionBuilder EnterspeedJobHandlers(this IUmbracoBuilder composition)
            => composition.WithCollectionBuilder<EnterspeedJobHandlerCollectionBuilder>();
    }
}
