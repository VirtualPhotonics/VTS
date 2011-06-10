using System;

namespace Vts.MonteCarlo.Rng
{
    /// <summary>
    /// Represents the data required for serializing and deserializing
    /// a Mersenne Twister random number generator
    /// </summary>
    public class MersenneTwisterSerializationInfo
    {
        public uint[] MT { get; set; }
        public int MTI { get; set; }
    }
}
