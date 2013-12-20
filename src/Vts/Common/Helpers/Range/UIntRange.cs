using System;

namespace Vts.Common
{
    /// <summary>
    /// Class that specifies a range of unsigned integers
    /// </summary>
    public class UIntRange : Range<uint>
    {
        /// <summary>
        /// Defines the range of unsigned integers
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The The number of values in the range, inclusive of the endpoints</param>
        public UIntRange(uint start, uint stop, int number)
            : base(start, stop, number)
        {
        }

        /// <summary>
        /// Defines a range of unsigned integers with a count of (stop - start) + 1
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        public UIntRange(uint start, uint stop)
            : this(start, stop, (int)(stop - start) + 1)
        {
        }

        /// <summary>
        /// Defines a range of unsigned integers from 0 to 1 with a count of 2
        /// </summary>
        public UIntRange()
            : this(0U, 1U, 2)
        {
        }

        /// <summary>
        /// Returns the value for delta
        /// </summary>
        /// <returns>An unsigned integer representing delta</returns>
        protected override uint GetDelta()
        {
            if (Count == 1)
            {
                if (Start != Stop)
                {
                    return Stop - Start;
                }

                return 0U;
            }

            return (uint)((Stop - Start) / (Count - 1));
        }

        /// <summary>
        /// Returns the increment
        /// </summary>
        /// <returns></returns>
        protected override Func<uint, uint> GetIncrement()
        {
            return d => (d + Delta);
        }

        /// <summary>
        /// Clones the range of unsigned integers
        /// </summary>
        /// <returns>A new UIntRange</returns>
        public UIntRange Clone()
        {
            return new UIntRange(Start, Stop, Count);
        }
    }
}
