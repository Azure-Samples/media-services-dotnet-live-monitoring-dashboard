using MediaDashboard.Common.Data;
using System;

namespace MediaDashboard.Common.Metrics
{

    public abstract class NumericThreshold<T> where T : IComparable
    {
        public T ExcellentLowerBound { get; set; }

        public T CriticalLowerBound { get; set; }

        public static NumericThreshold<T> Create(T excellent, T critical)
        {
            if (excellent.CompareTo(critical) > 0)
            {
                return new HigherValuesAreBetterNumericThreshold<T>()
                    {
                        ExcellentLowerBound = excellent,
                        CriticalLowerBound = critical
                    };
            }

            if (excellent.CompareTo(critical) < 0)
            {
                return new LowerValuesAreBetterNumericThreshold<T>()
                    {
                        ExcellentLowerBound = excellent,
                        CriticalLowerBound = critical
                    };
            }

            throw new ArgumentException("The bounds for the different states should be either in ascending or descending order");
        }

        public T[] Values()
        {
            return new T[] { this.ExcellentLowerBound, this.CriticalLowerBound };
        }

        public HealthStatus StateFor(T value)
        {
            var valueThresholds = this.Values();

            return this.IsValueWithinThreshold(value, valueThresholds[0])
                       ? HealthStatus.Healthy
                       : (this.IsValueWithinThreshold(value, valueThresholds[1])
                              ? HealthStatus.Warning
                              : HealthStatus.Critical);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is NumericThreshold<T>))
            {
                return false;
            }

            return this.Equals((NumericThreshold<T>)obj);
        }

        public bool Equals(NumericThreshold<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return NumericThreshold<T>.Equals(other.ExcellentLowerBound, this.ExcellentLowerBound) && NumericThreshold<T>.Equals(other.CriticalLowerBound, this.CriticalLowerBound);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.ExcellentLowerBound.GetHashCode();
                result = (result * 397) ^ this.CriticalLowerBound.GetHashCode();
                return result;
            }
        }

        protected abstract bool IsValueWithinThreshold(T value, T threshold);
    }
}