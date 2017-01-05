using System.Collections.Generic;

namespace MediaDashboard.Common.Helpers
{
    public static class LinqHelpers
    {
        public static IEnumerable<TSource> Denull<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? new List<TSource>();
        }
    }
}
