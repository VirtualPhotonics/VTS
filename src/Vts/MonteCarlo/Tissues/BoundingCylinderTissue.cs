using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to BoundingCylinderTissue class which is comprised of
    /// multilayer tissue bounded laterally by *vertical* cylinder
    /// </summary>
    public class BoundingCylinderTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion _cylinderRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of tissue bounded by capless cylinder height of tissue
        /// </summary>
        /// <param name="caplessCylinderRegion">bounding vertical cylinder region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public BoundingCylinderTissueInput(ITissueRegion caplessCylinderRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "BoundingCylinder";
            CylinderRegion = (CaplessCylinderTissueRegion)caplessCylinderRegion;
            LayerRegions = layerRegions;
            Regions = LayerRegions.Concat(CylinderRegion).ToArray();
        }

        /// <summary>
        /// BoundingCylinderTissueInput default constructor provides homogeneous tissue with bounding cylinder
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public BoundingCylinderTissueInput()
            : this(
                new CaplessCylinderTissueRegion(
                    new Position(0, 0, 50),
                    1.0,
                    100.0,
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
        public ITissueRegion[] Regions { get; private set; }

        /// <summary>
        /// tissue capless cylinder region
        /// </summary>
        public ITissueRegion CylinderRegion
        {
            get => _cylinderRegion;
            set
            {
                _cylinderRegion = value;
                if (LayerRegions != null) Regions = LayerRegions.Concat(_cylinderRegion).ToArray();
            }
        }

        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions
        {
            get => _layerRegions;
            set
            {
                _layerRegions = value;
                if (CylinderRegion != null) Regions = _layerRegions.Concat(CylinderRegion).ToArray();
            }
        }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue class</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new BoundedTissue(CylinderRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
