namespace Vts
{
    /// <summary>
    /// Random number generator types
    /// </summary>
    public enum RandomNumberGeneratorType
    {
        /// <summary>
        /// 19937 MT by Matsumoto (implemented by Math.NET Numerics)
        /// </summary>
        MersenneTwister,
        /// <summary>
        /// Dynamic creator for parallel processing 
        /// (http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/DC/dc.html)
        /// </summary>
        DynamicCreatorMersenneTwister,
        /// <summary>
        /// MathNet Numerics random number generator, inherited so it can be serialized
        /// </summary>
        SerializableMersenneTwister
    }
}