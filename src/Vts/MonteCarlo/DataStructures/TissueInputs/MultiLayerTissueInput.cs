using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiLayerTissue class.
    /// </summary>
    [KnownType(typeof(LayerRegion))]
    [KnownType(typeof(OpticalProperties))]
    [KnownType(typeof(List<OpticalProperties>))]
    [KnownType(typeof(List<LayerRegion>))]
    [KnownType(typeof(ITissueRegion[]))]
    public class MultiLayerTissueInput : ITissueInput
    {
        private ITissueRegion[] _regions;

        /// <summary>
        /// constructor for Multi-layer tissue input
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        public MultiLayerTissueInput(ITissueRegion[] regions)
        {
            _regions = regions;
        }

        /// <summary>
        /// MultiLayerTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiLayerTissueInput()
            : this(
                new ITissueRegion[]
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                })
        {
        }
        /// <summary>
        /// tissue identifier
        /// </summary>
        [IgnoreDataMember]
        public TissueType TissueType { get { return TissueType.MultiLayer; } }
        /// <summary>
        /// list of tissue regions comprising tissue
        /// </summary>
        public ITissueRegion[] Regions { get { return _regions; } set { _regions = value; } }
    }
}
