using System;

namespace Vts.Common
{
    public class DoubleRange : Range<double>
    {
        public DoubleRange(double start, double stop, int number)
            : base(start, stop, number)
        {
        }

        public DoubleRange(double start, double stop)
            : this(start, stop, 2)
        {
        }

        public DoubleRange()
            : this(0D, 1D, 2)
        {
        }

        protected override double GetDelta()
        {
            if (Count == 1)
            {
                return 0D;
            }

            return (Stop - Start) / (Count - 1);
        }

        protected override int GetNewCount()
        {
            if (Delta == 0d)
            {
                return 1;
            }

            return (int)((Stop - Start) / Delta + 1);
        }

        protected override Func<double, double> GetIncrement()
        {
            return d => d + Delta;
        }
    }
}
