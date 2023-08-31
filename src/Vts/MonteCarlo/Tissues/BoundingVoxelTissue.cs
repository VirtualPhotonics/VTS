using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to BoundingVoxelTissue class which is comprised of
    /// multilayer tissue bounded laterally by a voxel
    /// </summary>
    public class BoundingVoxelTissueInput : TissueInput, ITissueInput
    {

        /// <summary>
        /// allows definition of tissue bounded by capless voxel height of tissue
        /// </summary>
        /// <param name="caplessVoxelRegion">bounding vertical cylinder region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public BoundingVoxelTissueInput(ITissueRegion caplessVoxelRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "BoundingVoxel";
            VoxelRegion = (CaplessVoxelTissueRegion)caplessVoxelRegion;
            LayerRegions = layerRegions;
        }

        /// <summary>
        /// BoundingVoxelTissueInput default constructor provides homogeneous tissue with bounding 
        /// voxel with height same as MultiLayerTissue thickness
        /// </summary>
        public BoundingVoxelTissueInput()
            : this(
                new CaplessVoxelTissueRegion(
                    new DoubleRange(-10.0, 10),
                    new DoubleRange(-10.0, 10),
                    new DoubleRange(0.0, 10),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                ),
                new ITissueRegion[] 
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 10.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                })
        {
        }

        /// <summary>
        /// regions of tissue (layers and bounding voxel)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions => LayerRegions.Concat(VoxelRegion).ToArray();

        /// <summary>
        /// tissue capless cylinder region
        /// </summary>
        public ITissueRegion VoxelRegion { get; set; }
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
        /// <returns>instantiated tissue class</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new BoundedTissue(VoxelRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }
}
