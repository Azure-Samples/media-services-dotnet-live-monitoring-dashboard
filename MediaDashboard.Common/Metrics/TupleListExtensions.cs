using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Metrics
{
    public static class TupleListExtensions
    {
        public static TupleList<T1, T2> AsTupleList<T1, T2>(this IEnumerable<Tuple<T1, T2>> source)
        {
            return new TupleList<T1, T2>(source);
        }
    }
}
