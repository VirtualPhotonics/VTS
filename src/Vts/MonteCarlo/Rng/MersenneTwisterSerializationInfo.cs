
namespace Vts.MonteCarlo.Rng
{
    /// <summary>
    /// Represents the data required for serializing and deserializing
    /// a Mersenne Twister random number generator
    /// </summary>
    public class MersenneTwisterSerializationInfo
    {
        /// <summary>
        /// unsigned integer variable saved in order to resume series if interrupted
        /// </summary>
        public uint[] MT { get; set; }
        /// <summary>
        /// integer variable saved in order to resume series if interrupted
        /// </summary>
        public int MTI { get; set; }
    }
}
