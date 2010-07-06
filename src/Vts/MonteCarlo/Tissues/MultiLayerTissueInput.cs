using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiLayerTissue class.
    /// </summary>
    [KnownType(typeof(LayerRegion))]
    [KnownType(typeof(OpticalProperties))]
    [KnownType(typeof(List<LayerRegion>))]
    [KnownType(typeof(List<ITissueRegion>))]
    public class MultiLayerTissueInput : ITissueInput
    {
        private IList<ITissueRegion> _regions;

        /// <summary>
        /// MultiLayeredTissueInput constructor 
        /// </summary>
        public MultiLayerTissueInput(IList<LayerRegion> regions)
        {
            _regions = regions.Select(region => (ITissueRegion)region).ToList();
        }

        /// <summary>
        /// MultiLayerTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiLayerTissueInput() : this(
                new List<LayerRegion> 
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0, 2),
                        new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                        AbsorptionWeightingType.Discrete),
                    new LayerRegion(
                        new DoubleRange(0.0, 100.0, 2),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                        AbsorptionWeightingType.Discrete),
                    new LayerRegion(
                        new DoubleRange(100.0, double.PositiveInfinity, 2),
                        new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                        AbsorptionWeightingType.Discrete)
                }) {}

        public IList<ITissueRegion> Regions { get { return _regions; } set { _regions = value; } }
    }
}
