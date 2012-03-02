using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// MG-RTE simulation parameters
    /// </summary>
    public class MeshSimulationInput : BindableObject
    {
        private double _nExternal;
        private double _convTolerance;
        private int _methodMg;
        private int _nIterations;
        private int _nPreIterations;
        private int _nPostIterations;
        private int _nCycle;
        private int _fullMg;


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
        public MeshSimulationInput(
            double nExternal,
            double convTolerance,
            int methodMg,
            int nIterations,
            int nPreIterations,
            int nPostIterations,
            int nCycle,
            int fullMg
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
        }

        /// <summary>
        /// Initializes a new instance of MG-RTE simulation parameters
        /// </summary>
        /// <param name="nExternal">Refractive index of the external medium</param>
        /// <param name="convTolerance">The residual value of the iteration for stopping criterion</param>
        /// <param name="methodMg">The choice of multigrid method</param>
        /// <param name="nIterations">The maximum number of iteration on the finest level in FMG</param>
        public MeshSimulationInput(
            double nExternal,
            double convTolerance,
            int methodMg,
            int nIterations)
            : this(nExternal, convTolerance, methodMg, nIterations, 3, 3, 1, 1) { }

        /// <summary>
        /// Default constructor for MG-RTE simulation parameters
        /// </summary>
        public MeshSimulationInput()
            : this(1.0, 1.0e-4, 6, 100, 3, 3, 1, 1) { }

        /// <summary>
        /// Refractive index of the external medium
        /// </summary>
        public double NExternal
        {
            get { return _nExternal; }
            set { _nExternal = value; this.OnPropertyChanged("NExternal"); }
        }

        /// <summary>
        /// The residual value of the iteration for stopping criterion
        /// </summary>
        public double ConvTolerance
        {
            get { return _convTolerance; }
            set { _convTolerance = value; this.OnPropertyChanged("ConvTolerance"); }
        }

        /// <summary>
        /// The choice of multigrid method
        /// </summary>
        public int MethodMg
        {
            get { return _methodMg; }
            set { _methodMg = value; this.OnPropertyChanged("MethodMg"); }
        }

        /// <summary>
        /// The maximum number of iteration on the finest level in FMG
        /// </summary>
        public int NIterations
        {
            get { return _nIterations; }
            set { _nIterations = value; this.OnPropertyChanged("NIterations"); }
        }

        /// <summary>
        /// The number of pre-iterations with the suggested value "3", see the paper.
        /// </summary>
        public int NPreIterations
        {
            get { return _nPreIterations; }
            set { _nPreIterations = value; this.OnPropertyChanged("NPreIterations"); }
        }

        /// <summary>
        /// The number of post-iterations with the suggested value "3", see the paper.
        /// </summary>
        public int NPostIterations
        {
            get { return _nPostIterations; }
            set { _nPostIterations = value; this.OnPropertyChanged("NPostIterations"); }
        }

        /// <summary>
        /// The number of multigrid cycles on each level except the finest level in 
        /// FMG with the suggested value "1", see the paper.
        /// </summary>
        public int NCycle
        {
            get { return _nCycle; }
            set { _nCycle = value; this.OnPropertyChanged("NCycle"); }
        }

        /// <summary>
        /// The indicator of full multigrid method (FMG) with the suggested value "1"
        /// </summary>
        public int FullMg
        {
            get { return _fullMg; }
            set { _fullMg = value; this.OnPropertyChanged("FullMg"); }
        }
    }
}
