﻿using System;

namespace Enterspeed.Source.UmbracoCms.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsAbsoluteUrl(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
