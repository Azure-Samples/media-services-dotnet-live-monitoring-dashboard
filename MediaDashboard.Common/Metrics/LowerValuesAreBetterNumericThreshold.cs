using MediaDashboard.Common.Data;
using System;
using System.Globalization;

namespace MediaDashboard.Common.Metrics
{

    public class LowerValuesAreBetterNumericThreshold<T> : NumericThreshold<T> where T : IComparable
    {
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "<[..., {0}] => {2}; ({0}, {1}] => {3}; ({1}, ...] => {4}>",
                this.ExcellentLowerBound,
                this.CriticalLowerBound,
                HealthStatus.Healthy,
                HealthStatus.Warning,
                HealthStatus.Critical);
        }

        protected override bool IsValueWithinThreshold(T value, T threshold)
        {
            return value.CompareTo(threshold) <= 0;
        }
    }
}