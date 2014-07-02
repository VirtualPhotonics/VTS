using System;

namespace Vts.Common
{
    /// <summary>
    /// Class that specifies a range of integers
    /// </summary>
    public class IntRange : Range<int>
    {
        /// <summary>
        /// Defines the range of integers
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The The number of values in the range, inclusive of the endpoints</param>
        public IntRange(int start, int stop, int number)
            : base(start, stop, number)
        {
        }

        /// <summary>
        /// Defines a range of integers with a count of (stop - start) + 1
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        public IntRange(int start, int stop)
            : base(start, stop, (stop - start) + 1)
        {
        }

        /// <summary>
        /// Defines a range of integers from 0 to 1 with a count of 2
        /// </summary>
        public IntRange()
            : base(0, 1, 2)
        {
        }

        /// <summary>
        /// Returns the value for delta
        /// </summary>
        /// <returns>An integer representing delta</returns>
        protected override int GetDelta()
        {
            if (Count == 1)
            {
                if (Start != Stop)
                {
                    return Stop - Start;
                }

                return 0;
            }

            return (Stop - Start) / (Count - 1);
        }

        /// <summary>
        /// Returns the increment
        /// </summary>
        /// <returns></returns>
        protected override Func<int, int> GetIncrement()
        {
            return d => d + Delta;
        }
        
        /// <summary>
        /// Clones the range of integers
        /// </summary>
        /// <returns>A new IntRange</returns>
        public IntRange Clone()
        {
            return new IntRange(Start, Stop, Count);
        }
    }
}
