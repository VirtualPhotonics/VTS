using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiInfiniteCylinderTissue class.
    /// </summary>
    public class MultiInfiniteCylinderTissueInput : TissueInput, ITissueInput
    {
        /// <summary>
        /// allows definition of single InfiniteCylinder tissue
        /// </summary>
        /// <param name="infiniteCylinderRegions">InfiniteCylinder region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public MultiInfiniteCylinderTissueInput(ITissueRegion[] infiniteCylinderRegions, ITissueRegion[] layerRegions)
        {
            TissueType = "MultiInfiniteCylinder";
            InfiniteCylinderRegions = infiniteCylinderRegions;
            LayerRegions = layerRegions;
            Regions = LayerRegions.Concat(InfiniteCylinderRegions).ToArray();
        }

        /// <summary>
        /// SingleInfiniteCylinderTissueInput default constructor provides homogeneous tissue with single InfiniteCylinder
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public MultiInfiniteCylinderTissueInput()
            : this(
                [
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        1.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 5),
                        1.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                ],
                [
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 50.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                ])
        {
        }

        /// <summary>
        /// regions of tissue (layers and InfiniteCylinder)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get; private set; }

        /// <summary>
        /// tissue InfiniteCylinder region
        /// </summary>
        public ITissueRegion[] InfiniteCylinderRegions
        {
            get;
            set
            {
                field = value;
                if (LayerRegions != null) Regions = LayerRegions.Concat(field).ToArray();
            }
        }

        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions
        {
            get;
            set
            {
                field = value;
                if (InfiniteCylinderRegions != null) Regions = field.Concat(InfiniteCylinderRegions).ToArray();
            }
        }

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
            var t = new MultiInclusionTissue(InfiniteCylinderRegions, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
