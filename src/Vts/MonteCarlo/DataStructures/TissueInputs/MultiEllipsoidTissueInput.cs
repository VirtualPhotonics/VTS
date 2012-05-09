using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    [KnownType(typeof(EllipsoidRegion))]
    [KnownType(typeof(LayerRegion))]
    [KnownType(typeof(OpticalProperties))]
    [KnownType(typeof(HenyeyGreensteinPhaseFunctionInput))]
    [KnownType(typeof(LookupTablePhaseFunctionInput))]
    [KnownType(typeof(BidirectionalPhaseFunctionInput))]
    [KnownType(typeof(List<OpticalProperties>))]
    [KnownType(typeof(List<LayerRegion>))]
    [KnownType(typeof(List<ITissueRegion>))]
    public class MultiEllipsoidTissueInput : ITissueInput
    {
        private ITissueRegion[] _ellipsoidRegions;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single ellipsoid tissue
        /// </summary>
        /// <param name="ellipsoidRegion">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public MultiEllipsoidTissueInput(
            ITissueRegion[] ellipsoidRegions, 
            ITissueRegion[] layerRegions)
        {
            _ellipsoidRegions = ellipsoidRegions;
            _layerRegions = layerRegions;
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public MultiEllipsoidTissueInput()
            : this(
                new ITissueRegion[]
                {
                    new EllipsoidRegion(
                        new Position(0, 0, 1), 
                        0.5, 
                        0.5, 
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        new HenyeyGreensteinPhaseFunctionInput()),
                    new EllipsoidRegion(
                        new Position(0, 0, 2), 
                        0.5, 
                        0.5, 
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        new HenyeyGreensteinPhaseFunctionInput())
                },
                new ITissueRegion[] 
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        new HenyeyGreensteinPhaseFunctionInput()),
                    new LayerRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        new HenyeyGreensteinPhaseFunctionInput()),
                    new LayerRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        new HenyeyGreensteinPhaseFunctionInput())
                })
        {
        }
        /// <summary>
        /// tissue type
        /// </summary>
        [IgnoreDataMember]
        public TissueType TissueType { get { return TissueType.MultiEllipsoid; } }
        /// <summary>
        /// regions of tissue (layers and ellipsoid)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_ellipsoidRegions).ToArray(); } }
        /// <summary>
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion[] EllipsoidRegions { get { return _ellipsoidRegions; } set { _ellipsoidRegions = value; } }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }
      
    }
}
