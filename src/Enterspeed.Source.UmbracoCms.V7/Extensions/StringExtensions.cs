using System;

namespace Enterspeed.Source.UmbracoCms.V7.Extensions
{
    public static class StringExtensions
    {
        public static bool IsAbsoluteUrl(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        public static bool DetectIsJson(this string input)
        {
            if (input == null)
            {
                return false;
            }

            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}"))
                   || (input.StartsWith("[") && input.EndsWith("]"));
        }
    }
}
