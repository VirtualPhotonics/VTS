using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;
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
    [KnownType(typeof(List<OpticalProperties>))]
    [KnownType(typeof(List<LayerRegion>))]
    [KnownType(typeof(List<ITissueRegion>))]
    public class SingleEllipsoidTissueInput : ITissueInput
    {
        private ITissueRegion _ellipsoidRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// SingleEllipsoidTissueInput constructor 
        /// </summary>
        public SingleEllipsoidTissueInput(ITissueRegion ellipsoidRegion, ITissueRegion[] layerRegions)
        {
            _ellipsoidRegion = ellipsoidRegion;
            _layerRegions = layerRegions;
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue
        /// </summary>
        public SingleEllipsoidTissueInput()
            : this(
                new EllipsoidRegion(
                    new Position(0, 0, 1), 
                    0.5, 
                    0.5, 
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                ),
                new ITissueRegion[] 
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                })
        {
        }

        [IgnoreDataMember]
        public TissueType TissueType { get { return TissueType.SingleEllipsoid; } }

        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_ellipsoidRegion).ToArray(); } }

        public ITissueRegion EllipsoidRegion { get { return _ellipsoidRegion; } set { _ellipsoidRegion = value; } }

        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }
    }
}
