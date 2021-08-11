namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// MG-RTE simulation parameters
    /// </summary>
    public class MeshSimulationOptions
    {
        /// <summary>
        /// Constructor for MG-RTE simulation parameters
        /// </summary>
        /// <param name="nExternal">Refractive index of the external medium</param>
        /// <param name="convTolerance">The residual value of the iteration for stopping criterion</param>
        /// <param name="methodMg">The choice of multigrid method</param>
        /// <param name="nIterations">The maximum number of iteration on the finest level in FMG</param>
        /// <param name="nPreIterations">The number of pre-iterations with the suggested value "3". See paper.</param>
        /// <param name="nPostIterations"> The number of post-iterations with the suggested value "3". See paper.</param>
        /// <param name="nCycle">The number of multigrid cycles on each level except the finest level in FMG with the 
        /// suggested value "1".See paper.</param>
        /// <param name="fullMg">The indicator of full multigrid method (FMG) with the suggested value "1". See paper</param>
        /// <param name="startingSmeshLevel">Starting SmeshLevel (should be less than smeshLevel) </param>
        /// <param name="startingAmeshLevel">Starting AmeshLevel (should be less than ameshLevel) </param>
        public MeshSimulationOptions(
            double nExternal,
            double convTolerance,
            int methodMg,
            int nIterations,
            int nPreIterations,
            int nPostIterations,
            int nCycle,
            int fullMg,
            int startingSmeshLevel,
            int startingAmeshLevel
            )
        {
            NExternal = nExternal;
            ConvTolerance = convTolerance;
            MethodMg = methodMg;
            NIterations = nIterations;
            NPreIterations = nPreIterations;
            NPostIterations = nPostIterations;
            NCycle = nCycle;
            FullMg = fullMg;
            StartingSmeshLevel = startingSmeshLevel;
            StartingAmeshLevel = startingAmeshLevel;
        }

        /// <summary>
        /// Initializes a new instance of MG-RTE simulation parameters
        /// </summary>
        /// <param name="nExternal">Refractive index of the external medium</param>
        /// <param name="convTolerance">The residual value of the iteration for stopping criterion</param>
        /// <param name="methodMg">The choice of multigrid method</param>
        /// <param name="nIterations">The maximum number of iteration on the finest level in FMG</param>
        public MeshSimulationOptions(
            double nExternal,
            double convTolerance,
            int methodMg,
            int nIterations)
            : this(nExternal, convTolerance, methodMg, nIterations, 3, 3, 1, 1, 1, 1 ) { }

        /// <summary>
        /// Default constructor for MG-RTE simulation parameters
        /// </summary>
        public MeshSimulationOptions()
            : this(1.0, 1.0e-4, 6, 100, 3, 3, 1, 1, 1, 1) { }

        /// <summary>
        /// Refractive index of the external medium
        /// </summary>
        public double NExternal { get; set; }

        /// <summary>
        /// The residual value of the iteration for stopping criterion
        /// </summary>
        public double ConvTolerance { get; set; }

        /// <summary>
        /// The choice of multigrid method
        /// </summary>
        public int MethodMg { get; set; }

        /// <summary>
        /// The maximum number of iteration on the finest level in FMG
        /// </summary>
        public int NIterations { get; set; }

        /// <summary>
        /// The number of pre-iterations with the suggested value "3", see the paper.
        /// </summary>
        public int NPreIterations { get; set; }

        /// <summary>
        /// The number of post-iterations with the suggested value "3", see the paper.
        /// </summary>
        public int NPostIterations { get; set; }

        /// <summary>
        /// The number of multigrid cycles on each level except the finest level in 
        /// FMG with the suggested value "1", see the paper.
        /// </summary>
        public int NCycle { get; set; }

        /// <summary>
        /// The indicator of full multigrid method (FMG) with the suggested value "1"
        /// </summary>
        public int FullMg { get; set; }

        /// <summary>
        /// Starting SmeshLevel (should be less than smeshLevel)
        /// </summary>
        public int StartingSmeshLevel { get; set; }

        /// <summary>
        /// Starting AmeshLevel (should be less than ameshLevel)
        /// </summary>
        public int StartingAmeshLevel { get; set; }
    }
}
