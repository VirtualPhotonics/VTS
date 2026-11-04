using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiInfiniteCylinderTissue class.
    /// Defines a tissue geometry comprised of a list of infinite cylinders with axis along the y-axis,
    /// embedded within *multiple* (non-air) layers of a layered slab.  An example would be a two layer
    /// phantom with multiple infinite cylinders in each layer. The layers are assumed to be infinite in the
    /// x and y directions. The infinite cylinders can be located anywhere within the layers, but can not
    /// overlap with each other or with the layer boundaries.  The tissue is assumed to be
    /// surrounded by air above and below, which must be specified as the first and last layers in the
    /// list of LayerTissueRegion objects.
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
        /// tissue InfiniteCylinder regions
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
