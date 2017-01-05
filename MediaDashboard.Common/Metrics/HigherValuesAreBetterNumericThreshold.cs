using MediaDashboard.Common.Data;
using System;
using System.Globalization;

namespace MediaDashboard.Common.Metrics
{
    public class HigherValuesAreBetterNumericThreshold<T> : NumericThreshold<T> where T : IComparable
    {
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "<[..., {0}] => {2}; ({0}, {1}] => {3}; ({1}, ...] => {4}>",
                this.CriticalLowerBound,
                this.ExcellentLowerBound,
                HealthStatus.Critical,
                HealthStatus.Warning,
                HealthStatus.Healthy);
        }

        protected override bool IsValueWithinThreshold(T value, T threshold)
        {
            return value.CompareTo(threshold) >= 0;
        }
    }
}