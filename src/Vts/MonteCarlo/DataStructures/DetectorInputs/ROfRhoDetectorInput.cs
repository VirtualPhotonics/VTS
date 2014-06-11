//using System;
//using System.Collections.Generic;
//using Vts.Common;

//namespace Vts.MonteCarlo
//{
//    /// <summary>
//    /// DetectorInput for R(r)
//    /// </summary>
//    public class ROfRhoDetectorInput : IDetectorInput
//    {
//        /// <summary>
//        /// constructor for reflectance as a function of rho detector input
//        /// </summary>
//        /// <param name="rho">rho binning</param>
//        /// <param name="name">detector name</param>
//        public ROfRhoDetectorInput(DoubleRange rho, String name)
//        {
//            TallyType = "ROfRho";
//            Name = name;
//            Rho = rho;
//        }
//        /// <summary>
//        /// constructor uses TallyType as name
//        /// </summary>
//        /// <param name="rho">rho binning</param>
//        public ROfRhoDetectorInput(DoubleRange rho) 
//            : this (rho, "ROfRho") {}

//        /// <summary>
//        /// Default constructor uses default rho bins
//        /// </summary>
//        public ROfRhoDetectorInput() 
//            : this(new DoubleRange(0.0, 10, 101),"ROfRho") {}

//        /// <summary>
//        /// detector tally identifier
//        /// </summary>
//        public string TallyType { get; set; }
//        /// <summary>
//        /// detector name, defaults to TallyType.ToString() but can be user specified
//        /// </summary>
//        public string Name { get; set; }
//        /// <summary>
//        /// detector rho binning
//        /// </summary>
//        public DoubleRange Rho { get; set; }
//    }
//}
