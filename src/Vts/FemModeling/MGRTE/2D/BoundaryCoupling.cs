namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Boundary reflection or refraction coupling structure
    /// </summary>
    public struct BoundaryCoupling
    {
        private int[][][] _ri;
        private int[][][] _ro;
        private int[][][] _si;
        private int[][][] _so;
        private double[][][] _ri2;
        private double[][][] _ro2;
        private double[][][] _si2;
        private double[][][] _so2;
        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   Ri[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] Ri { get { return _ri; } set { _ri = value; } }

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: Ro[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] Ro { get { return _ro; } set { _ro = value; } }

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: Si[ne][ns][2]  --- interpolated direction
        /// </summary>
        public int[][][] Si {  get { return _si; } set { _si = value; } }

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   So[ne][ns][2]  --- interpolated weight
        /// </summary>
        public int[][][] So { get { return _so; } set { _so = value; } }

        /// <summary>
        /// reflection contribution to incoming flux from outgoing flux:   Ri2[ne][ns][2] --- interpolated weight
        /// </summary>
        public double[][][] Ri2 { get { return _ri2; } set { _ri2 = value; } }

        /// <summary>
        /// reflection contribution to outgoing flux from boundary source: Ro2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] Ro2 { get { return _ro2; } set { _ro2 = value; } }

        /// <summary>
        /// refraction contribution to incoming flux from boundary source: Si2[ne][ns][2] --- interpolated weight   
        /// </summary>
        public double[][][] Si2 { get { return _si2; } set { _si2 = value; } }

        /// <summary>
        /// refraction contribution to outgoing flux from outgoing flux:   So2[ne][ns][2] --- interpolated direction
        /// </summary>
        public double[][][] So2 { get { return _so2; } set { _so2 = value; } }
    };
}
