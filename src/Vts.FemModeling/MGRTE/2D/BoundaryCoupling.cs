namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Boundary reflection or refraction coupling structure
    /// </summary>
    public struct BoundaryCoupling
    {
        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   Ri[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] Ri;

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: Ro[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] Ro;

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: Si[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] Si;

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   So[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] So;

        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   Ri2[ne][ns][2] --- interpolated weight
        /// </summary>
        public double[][][] Ri2;

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: Ro2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] Ro2;

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: Si2[ne][ns][2] --- interpolated weight   
        /// </summary>
        public double[][][] Si2;

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   So2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] So2;
    };
}
