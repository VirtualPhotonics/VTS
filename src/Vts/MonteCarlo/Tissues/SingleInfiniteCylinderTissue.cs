using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleInfiniteCylinderTissue class.
    /// </summary>
    public class SingleInfiniteCylinderTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion _infiniteCylinderRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single infinite cylinder tissue
        /// </summary>
        /// <param name="infiniteCylinderRegion">infinite cylinder region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleInfiniteCylinderTissueInput(ITissueRegion infiniteCylinderRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "SingleInfiniteCylinder";
            _infiniteCylinderRegion = infiniteCylinderRegion;
            _layerRegions = layerRegions;
        }

        /// <summary>
        /// SingleInfiniteCylinderTissueInput default constructor provides homogeneous tissue with single infinite cylinder
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public SingleInfiniteCylinderTissueInput()
            : this(
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    1.0,
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
        /// regions of tissue (layers and infinite cylinder)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_infiniteCylinderRegion).ToArray(); } }
        /// <summary>
        /// tissue infinite cylinder region
        /// </summary>
        public ITissueRegion InfiniteCylinderRegion { get { return _infiniteCylinderRegion; } set { _infiniteCylinderRegion = value; } }
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
            var t = new SingleInclusionTissue(InfiniteCylinderRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
