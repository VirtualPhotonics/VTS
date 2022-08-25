using System.Collections.Generic;
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

        /// <summary>
        /// allows definition of single infinite cylinder tissue
        /// </summary>
        /// <param name="infiniteCylinderRegion">infinite cylinder region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleInfiniteCylinderTissueInput(ITissueRegion infiniteCylinderRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "SingleInfiniteCylinder";
            InfiniteCylinderRegion = infiniteCylinderRegion;
            LayerRegions = layerRegions;
            RegionPhaseFunctionInputs = new Dictionary<string, IPhaseFunctionInput>();
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
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                    "HenyeyGreensteinKey1"
                ),
                new ITissueRegion[] 
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                })
        {
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
        }

        /// <summary>
        /// regions of tissue (layers and infinite cylinder)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return LayerRegions.Concat(InfiniteCylinderRegion).ToArray(); } }
        /// <summary>
        /// tissue infinite cylinder region
        /// </summary>
        public ITissueRegion InfiniteCylinderRegion { get; set; }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get; set; }
        /// <summary>
        /// dictionary of region phase function inputs
        /// </summary>
        public IDictionary<string, IPhaseFunctionInput> RegionPhaseFunctionInputs { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="regionPhaseFunctions">dictionary of region phase functions</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, 
            IDictionary<string, IPhaseFunction> regionPhaseFunctions, 
            double russianRouletteWeightThreshold)
        {
            var t = new SingleInclusionTissue(InfiniteCylinderRegion, LayerRegions);

            t.Initialize(awt, regionPhaseFunctions, russianRouletteWeightThreshold);

            return t;
        }
    }
}
