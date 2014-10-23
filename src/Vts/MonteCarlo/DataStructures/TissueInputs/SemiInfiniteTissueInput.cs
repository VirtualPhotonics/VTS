using System.Runtime.Serialization;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SemiInfiniteTissueInput class.
    /// </summary>
    [KnownType(typeof(LayerRegion))]
    [KnownType(typeof(OpticalProperties))]
    public class SemiInfiniteTissueInput : ITissueInput
    {
        private ITissueRegion[] _regions;

        /// <summary>
        /// constructor for Semi-infinite tissue input
        /// </summary>
        /// <param name="region">tissue region info</param>
        public SemiInfiniteTissueInput(ITissueRegion region)
        {
            _regions = new[] { region };
        }

        /// <summary>
        /// SemiInfiniteTissueInput default constructor provides homogeneous tissue
        /// </summary>
        public SemiInfiniteTissueInput()
            : this(new SemiInfiniteRegion(new OpticalProperties(0.02, 1.0, 0.8, 1.4)))
        {
        }

        /// <summary>
        /// tissue identifier
        /// </summary>
        [IgnoreDataMember]
        public TissueType TissueType { get { return TissueType.SemiInfinite; } }

        /// <summary>
        /// list of tissue regions comprising tissue
        /// </summary>
        public ITissueRegion[] Regions { get { return _regions; } set { _regions = value; } }
    }
}
