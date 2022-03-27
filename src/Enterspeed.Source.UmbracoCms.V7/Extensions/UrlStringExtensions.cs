namespace Enterspeed.Source.UmbracoCms.V7.Extensions
{
    public static class UrlStringExtensions
    {
        public static string EnsureTrailingSlash(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            return url;
        }
    }
}
