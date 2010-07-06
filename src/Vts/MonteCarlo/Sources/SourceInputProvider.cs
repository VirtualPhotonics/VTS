using Vts.Common;

namespace Vts.MonteCarlo.Sources
{   
    /// <summary>
    /// Implements ISourceInput.  Defines commonly used ISourceImput
    /// implementations (e.g. CollimatedPointSourceInput).
    /// </summary>
    public class SourceInputProvider : ISourceInput
    {
        /// <summary>
        /// CollimatedPointSource
        /// </summary>
        public static ISourceInput CollimatedPointSourceInput()
        {
            return new PointSourceInput(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                new DoubleRange(0.0, 0, 1)
            );
        }
    }
}
