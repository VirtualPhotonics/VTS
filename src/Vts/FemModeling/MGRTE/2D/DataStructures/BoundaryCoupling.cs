namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public struct BoundaryCoupling  // boundary reflection or refraction coupling (see "bc_reflection")
    {
        public int[][][] ri;                  // reflection contribution to incoming flux from outgoing flux:   ri[ne][ns][2]  --- interpolated direction
        public int[][][] ro;                  // reflection contribution to outgoing flux from boundary source: ro[ne][ns][2]  --- interpolated weight
        public int[][][] si;                  // refraction contribution to incoming flux from boundary source: si[ne][ns][2]  --- interpolated direction
        public int[][][] so;                  // refraction contribution to outgoing flux from outgoing flux:   so[ne][ns][2]  --- interpolated weight

        public double[][][] ri2;              // reflection contribution to incoming flux from outgoing flux:   ri2[ne][ns][2] --- interpolated weight
        public double[][][] ro2;              // reflection contribution to outgoing flux from boundary source: ro2[ne][ns][2] --- interpolated direction
        public double[][][] si2;              // refraction contribution to incoming flux from boundary source: si2[ne][ns][2] --- interpolated weight         
        public double[][][] so2;              // refraction contribution to outgoing flux from outgoing flux:   so2[ne][ns][2] --- interpolated direction
    };
}
