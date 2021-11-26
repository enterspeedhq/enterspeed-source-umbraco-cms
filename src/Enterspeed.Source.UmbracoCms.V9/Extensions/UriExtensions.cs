using System;

namespace Enterspeed.Source.UmbracoCms.V9.Extensions
{
    public static class UriExtensions
    {
        public static Uri AppendPath(this Uri uri, string path)
        {
            return new Uri(uri, uri.AbsolutePath.TrimEnd('/') + '/' + path.TrimStart('/'));	
        }
    }
}