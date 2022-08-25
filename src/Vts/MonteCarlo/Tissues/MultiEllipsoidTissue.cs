using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    public class MultiEllipsoidTissueInput : TissueInput, ITissueInput
    {

        /// <summary>
        /// allows definition of single ellipsoid tissue
        /// </summary>
        /// <param name="ellipsoidRegions">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public MultiEllipsoidTissueInput(ITissueRegion[] ellipsoidRegions, ITissueRegion[] layerRegions)
        {
            TissueType = "MultiEllipsoid";
            EllipsoidRegions = ellipsoidRegions;
            LayerRegions = layerRegions;
            RegionPhaseFunctionInputs = new Dictionary<string, IPhaseFunctionInput>();
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public MultiEllipsoidTissueInput()
            : this(
                new ITissueRegion[]
                {
                    new EllipsoidTissueRegion(
                        new Position(10, 0, 10), 
                        5.0, 
                        1.0, 
                        5.0,
                        new OpticalProperties(0.1, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey1"),
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 40), 
                        5.0, 
                        0, 
                        5.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2")
                },
                new ITissueRegion[] 
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 50.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey4"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey5")
                })
        {  
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey5", new HenyeyGreensteinPhaseFunctionInput());
   
        }

        /// <summary>
        /// regions of tissue (layers and ellipsoid)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return LayerRegions.Concat(EllipsoidRegions).ToArray(); } }
        /// <summary>
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion[] EllipsoidRegions { get; set; }
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
        /// <param name="regionPhaseFunctions">Phase function dictionary</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        public ITissue CreateTissue(AbsorptionWeightingType awt, IDictionary<string, IPhaseFunction> regionPhaseFunctions, double russianRouletteWeightThreshold)
        {
            throw new NotImplementedException();
        }
     
    }
}
