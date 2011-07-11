namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public struct LightSource
    {
        //  Case 1: point source with angular dependence
        //  Case 2: isotropic point source
        //  Case 3: isotropic source prescribed everywhere in space ([nt][3])
        public int S_TYPE;        // type of internal source
        public int n;                  // number of point sources in Case 1 and 2; "nt" in Case 3.
        public int[] a;                 // angular direction: a[source.n] for Case 1
        public int[] t;                 // spatial position (triangle): t[source.n] for Case 1 and 2
        public double[,] i;             // source intensity: i[source.n][3].

        //  Case 1: point source with angular dependence
        //  Case 2: isotropic point source
        //  Case 3: isotropic source prescribed everywhere at boundary ([ne][2])
        public int BS_TYPE;       //type of boundary source
        public int n2;                 // number of point sources in Case 1 and 2; "ne" in Case 3.
        public int[] a2;                // angular direction: a[source.n2] for Case 1
        public int[] e;                 // spatial position (boundary edge): e[source.n2] for Case 1 and 2
        public double[,] i2;            // source intensity: i[source.n2][2].
    };
}
