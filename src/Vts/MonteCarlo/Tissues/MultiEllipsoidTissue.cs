using System;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    public class MultiEllipsoidTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion[] _ellipsoidRegions;
        private ITissueRegion[] _layerRegions;

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
            Regions = LayerRegions.Concat(EllipsoidRegions).ToArray();
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
                        new OpticalProperties(0.1, 1.0, 0.8, 1.4)),
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 40),
                        5.0,
                        0,
                        5.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                },
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 50.0),
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
        public ITissueRegion[] EllipsoidRegions
        {
            get => _ellipsoidRegions;
            set
            {
                _ellipsoidRegions = value;
                if (LayerRegions != null) Regions = LayerRegions.Concat(_ellipsoidRegions).ToArray();
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
                if (EllipsoidRegions != null) Regions = _layerRegions.Concat(EllipsoidRegions).ToArray();
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
            throw new NotImplementedException();
        }
    }
}
