using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleCylinderTissue class.
    /// </summary>
    public class SingleCylinderTissueInput : TissueInput, ITissueInput
    {

        /// <summary>
        /// allows definition of single cylinder tissue
        /// </summary>
        /// <param name="cylinderRegion">cylinder region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleCylinderTissueInput(ITissueRegion cylinderRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "SingleCylinder";
            CylinderRegion = cylinderRegion;
            LayerRegions = layerRegions;
        }

        /// <summary>
        /// SingleCylinderTissueInput default constructor provides homogeneous tissue with single Cylinder
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public SingleCylinderTissueInput()
            : this(
                new CylinderTissueRegion(
                    new Position(0, 0, 3),
                    2,
                    2,
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
        /// regions of tissue (layers and Cylinder)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return LayerRegions.Concat(CylinderRegion).ToArray(); } }
        /// <summary>
        /// tissue Cylinder region
        /// </summary>
        public ITissueRegion CylinderRegion { get; set; }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new SingleInclusionTissue(CylinderRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
