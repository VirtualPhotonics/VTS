using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    public class SingleEllipsoidTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion _ellipsoidRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single ellipsoid tissue
        /// </summary>
        /// <param name="ellipsoidRegion">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleEllipsoidTissueInput(ITissueRegion ellipsoidRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "SingleEllipsoid";
            _ellipsoidRegion = ellipsoidRegion;
            _layerRegions = layerRegions;
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public SingleEllipsoidTissueInput()
            : this(
                new EllipsoidTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
                    0.5,
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                ),
                new ITissueRegion[] 
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                })
        {
        }

        /// <summary>
        /// regions of tissue (layers and ellipsoid)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_ellipsoidRegion).ToArray(); } }
        /// <summary>
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion EllipsoidRegion { get { return _ellipsoidRegion; } set { _ellipsoidRegion = value; } }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }

        /// <summary>
        ///// Required factory method to create the corresponding 
        ///// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns></returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new SingleInclusionTissue(EllipsoidRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
