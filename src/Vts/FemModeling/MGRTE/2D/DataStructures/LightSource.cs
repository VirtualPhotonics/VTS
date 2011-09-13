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

        /// <summary>
        /// Source type: 0: Internal,  1: External 
        /// </summary>
        public bool sourceType;

        /// <summary>
        /// Number of internal sources
        /// </summary>
        public int nInt;

        /// <summary>
        /// Minimum polar angle for internal sources
        /// </summary>
        public double []angMinInt;

        /// <summary>
        /// Maximum polar angle for internal sources
        /// </summary>
        public double[] angMaxInt;

        /// <summary>
        /// Number of external sources
        /// </summary>
        public int nExt;

        /// <summary>
        /// Minimum polar angle for external sources
        /// </summary>
        public double[] angMinExt;

        /// <summary>
        /// Maximum polar angle for external sources
        /// </summary>
        public double[] angMaxExt;
    };
}
