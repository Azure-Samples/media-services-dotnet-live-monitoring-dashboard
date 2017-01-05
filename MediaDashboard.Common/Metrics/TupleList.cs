using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Metrics
{
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public TupleList()
        {
        }

        public TupleList(IEnumerable<Tuple<T1, T2>> source) : base(source)
        {
        }
    }
}
