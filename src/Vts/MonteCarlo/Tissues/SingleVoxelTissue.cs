using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleVoxelTissue class.
    /// </summary>
    public class SingleVoxelTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion _voxelRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single voxel tissue
        /// </summary>
        /// <param name="voxelRegion">voxel region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleVoxelTissueInput(ITissueRegion voxelRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "SingleVoxel";
            VoxelRegion = voxelRegion;
            LayerRegions = layerRegions;
            Regions = LayerRegions.Concat(VoxelRegion).ToArray();
        }

        /// <summary>
        /// SingleVoxelTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public SingleVoxelTissueInput()
            : this(
                new VoxelTissueRegion(
                    new DoubleRange(-5, 5), 
                    new DoubleRange(-5, 5), 
                    new DoubleRange(1, 6),
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
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion VoxelRegion
        {
            get => _voxelRegion;
            set
            {
                _voxelRegion = value;
                if (LayerRegions != null) Regions = LayerRegions.Concat(_voxelRegion).ToArray();
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
                if (VoxelRegion != null) Regions = _layerRegions.Concat(VoxelRegion).ToArray();
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
            var t = new SingleInclusionTissue(VoxelRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
