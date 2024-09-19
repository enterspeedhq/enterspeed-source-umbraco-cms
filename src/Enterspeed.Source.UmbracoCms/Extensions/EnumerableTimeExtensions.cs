using System;
using System.Collections.Generic;
using System.Linq;

namespace Enterspeed.Source.UmbracoCms.Base.Extensions
{
    public static class EnumerableTimeExtensions
    {
        /// <summary>
        /// Our own version of DistinctBy in order to support .net5
        /// </summary>
        public static IEnumerable<T> DistinctByProperty<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
    }
}