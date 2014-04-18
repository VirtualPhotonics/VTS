using System;

namespace Vts.Common
{
    /// <summary>
    /// Class that specifies a range of doubles
    /// </summary>
    public class DoubleRange : Range<double>
    {
        /// <summary>
        /// Defines the double range
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The The number of values in the range, inclusive of the endpoints</param>
        public DoubleRange(double start, double stop, int number)
            : base(start, stop, number)
        {
        }

        /// <summary>
        /// Defines a double range with a count of 2
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        public DoubleRange(double start, double stop)
            : this(start, stop, 2)
        {
        }

        /// <summary>
        /// Defines a double range starting from 0 to 1 with a count of 2
        /// </summary>
        public DoubleRange()
            : this(0D, 1D, 2)
        {
        }

        /// <summary>
        /// Returns the value for delta
        /// </summary>
        /// <returns>A double representing delta</returns>
        protected override double GetDelta()
        {
            if (Count == 1)
            {
                if (Start != Stop)
                {
                    return Stop - Start;
                }

                return 0D;
            }

            return (Stop - Start) / (Count - 1);
        }

        /// <summary>
        /// Returns the increment
        /// </summary>
        /// <returns></returns>
        protected override Func<double, double> GetIncrement()
        {
            return d => d + Delta;
        }

        /// <summary>
        /// Clones the double range
        /// </summary>
        /// <returns>A new DoubleRange</returns>
        public DoubleRange Clone()
        {
            return new DoubleRange(Start, Stop, Count);
        }
    }
}
