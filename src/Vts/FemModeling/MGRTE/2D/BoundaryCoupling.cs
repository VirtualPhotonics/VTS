using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Boundary reflection or refraction coupling structure
    /// </summary>
    public struct BoundaryCoupling
    {
        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   ri[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] ri;

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: ro[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] ro;

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: si[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] si;

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   so[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] so;

        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   ri2[ne][ns][2] --- interpolated weight
        /// </summary>
        public double[][][] ri2;

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: ro2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] ro2;

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: si2[ne][ns][2] --- interpolated weight   
        /// </summary>
        public double[][][] si2;

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   so2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] so2;
    };
}
