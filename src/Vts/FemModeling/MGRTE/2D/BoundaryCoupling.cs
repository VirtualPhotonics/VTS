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
        public int[][][] Ri { get; set; }

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: Ro[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] Ro { get; set; }

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: Si[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] Si { get; set; }

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   So[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] So { get; set; }

        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   Ri2[ne][ns][2] --- interpolated weight
        /// </summary>
        public double[][][] Ri2 { get; set; }

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: Ro2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] Ro2 { get; set; }

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: Si2[ne][ns][2] --- interpolated weight   
        /// </summary>
        public double[][][] Si2 { get; set; }

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   So2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] So2 { get; set; }
    };
}
