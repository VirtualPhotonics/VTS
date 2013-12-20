using System;

namespace Vts.Common
{
    /// <summary>
    /// Class that specifies a range of floating-point values
    /// </summary>
    public class FloatRange : Range<float>
    {
        /// <summary>
        /// Defines the range of floats
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The number of values in the range, inclusive of the endpoints</param>
        public FloatRange(float start, float stop, int number)
            : base(start, stop, number)
        {
        }

        /// <summary>
        /// Defines a range of floats with a count of 2
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        public FloatRange(float start, float stop)
            : this(start, stop, 2)
        {
        }

        /// <summary>
        /// Defines a range of floats from 0 to 1 with a count of 2
        /// </summary>
        public FloatRange()
            : this(0F, 1F, 2)
        {
        }

        /// <summary>
        /// Returns the value for delta
        /// </summary>
        /// <returns>A float representing delta</returns>
        protected override float GetDelta()
        {
            if (Count == 1)
            {
                if (Start != Stop)
                {
                    return Stop - Start;
                }

                return 0f;
            }

            return (Stop - Start) / (Count - 1F);
        }

        /// <summary>
        /// Returns the increment
        /// </summary>
        /// <returns></returns>
        protected override Func<float, float> GetIncrement()
        {
            return d => d + Delta;
        }

        /// <summary>
        /// Clones the range of floats
        /// </summary>
        /// <returns>A new FloatRange</returns>
        public FloatRange Clone()
        {
            return new FloatRange(Start, Stop, Count);
        }
    }
}
