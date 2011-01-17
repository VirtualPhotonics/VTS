using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    public abstract class LineSourceBase : SourceBase
    {
        /// <summary>
        /// Creates a radially-isotropic line source of a given length, centered at the specified position, 
        /// and oriented along the specified direction.
        /// </summary>
        /// <param name="center">The center position of the line source</param>
        /// <param name="lineAxis">The axis of the line source. (Must be normalized!)</param>
        /// <param name="length">The length of the line source.</param>
        public LineSourceBase(Position center, Direction lineAxis, double length)
            : base(center, lineAxis)
        {
            Length = length;
        }
        
        /// <summary>
        /// The length of the line
        /// </summary>
        public double Length { get; private set; }
    }
}