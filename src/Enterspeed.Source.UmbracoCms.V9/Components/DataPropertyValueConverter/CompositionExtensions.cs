using Umbraco.Cms.Core.DependencyInjection;

namespace Enterspeed.Source.UmbracoCms.V9.Components.DataPropertyValueConverter
{
    public static class CompositionExtensions
    {
        public static EnterspeedPropertyValueConverterCollectionBuilder EnterspeedPropertyValueConverters(this IUmbracoBuilder  composition)
            => composition.WithCollectionBuilder<EnterspeedPropertyValueConverterCollectionBuilder>();

        public static EnterspeedGridEditorValueConverterCollectionBuilder EnterspeedGridEditorValueConverters(this IUmbracoBuilder  composition)
            => composition.WithCollectionBuilder<EnterspeedGridEditorValueConverterCollectionBuilder>();
    }
}
