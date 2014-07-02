using System;

namespace Vts.Common
{
    /// <summary>
    /// Class that specifies a range of long variables
    /// </summary>
    public class LongRange : Range<long>
    {
        /// <summary>
        /// Defines the range of long variables
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The The number of values in the range, inclusive of the endpoints</param>
        public LongRange(long start, long stop, int number)
            : base(start, stop, number)
        {
        }

        /// <summary>
        /// Defines a range of long variables with a count of (stop - start) + 1
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        public LongRange(int start, int stop)
            : this(start, stop, (stop - start) + 1)
        {
        }

        /// <summary>
        /// Defines a range of long variables from 0 to 1 with a count of 2
        /// </summary>
        public LongRange()
            : this(0L, 1L, 2)
        {
        }

        /// <summary>
        /// Returns the value for delta
        /// </summary>
        /// <returns>A long representing delta</returns>
        protected override long GetDelta()
        {
            if (Count == 1)
            {
                if (Start != Stop)
                {
                    return Stop - Start;
                }

                return 0L;
            }

            return (Stop - Start) / (Count - 1);
        }

        /// <summary>
        /// Returns the increment
        /// </summary>
        /// <returns></returns>
        protected override Func<long, long> GetIncrement()
        {
            return d => d + Delta;
        }

        /// <summary>
        /// Clones the range of long variables
        /// </summary>
        /// <returns>A new LongRange</returns>
        public LongRange Clone()
        {
            return new LongRange(Start, Stop, Count);
        }
    }
}
