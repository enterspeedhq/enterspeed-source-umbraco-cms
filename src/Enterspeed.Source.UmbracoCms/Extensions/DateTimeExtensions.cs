using System;

namespace Enterspeed.Source.UmbracoCms.Base.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> object to a string with the following format: yyyy-MM-ddTHH:mm:ss.
        /// </summary>
        public static string ToEnterspeedFormatString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}