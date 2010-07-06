using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used SimulationInput classes.  For
    /// example, a point source through homogenous tissue and all detector tallies.
    /// </summary>
    public class SimulationInputProvider : SimulationInput
    {
        /// <summary>
        /// PointSourceHomogeneous
        /// </summary>
        public static SimulationInput PointSourceHomogeneous()
        {
            return new SimulationInput()
            {
                N = (long)1e6, 
                OutputFileName = "PointSourceHomogeneous",
                SourceInput = new PointSourceInput(),
                TissueInput = new MultiLayerTissueInput(),
                DetectorInput = new DetectorInput()
            };
        }
    }
}
