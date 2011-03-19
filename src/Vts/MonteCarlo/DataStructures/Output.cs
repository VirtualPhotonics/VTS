using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Numerics;
using Vts.IO;
using System.Collections.Generic;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Helper class to surface the results of a Monte Carlo simulation in a user-friendly (strongly-typed) way
    /// </summary>
    public class Output
    {
        public Output(SimulationInput si, IList<IDetector> detectorResults)
        {
            Input = si;
            ResultsDictionary = detectorResults.ToDictionary(d => d.TallyType);
        }

        public double Rd { get { return ((RDiffuseDetector)ResultsDictionary[TallyType.RDiffuse]).Mean; } }
        public double Rd2 { get { return ((RDiffuseDetector)ResultsDictionary[TallyType.RDiffuse]).SecondMoment; } }

        //public double Rtot { get { return ((RTotalDetector)ResultsDictionary[TallyType.RTotal]).Mean; } }
        //public double Rtot2 { get { return ((RTotalDetector)ResultsDictionary[TallyType.RTotal]).SecondMoment; } }

        public double Td { get { return ((TDiffuseDetector)ResultsDictionary[TallyType.TDiffuse]).Mean; } }
        public double Td2 { get { return ((TDiffuseDetector)ResultsDictionary[TallyType.TDiffuse]).SecondMoment; } }

        public double Atot { get { return ((ATotalDetector)ResultsDictionary[TallyType.ATotal]).Mean; } }
        public double Atot2 { get { return ((ATotalDetector)ResultsDictionary[TallyType.ATotal]).SecondMoment; } }

        //public double[] A_z { get { return ((AOfZDetector)ResultsDictionary[TallyType.AOfZ]).Mean; } }
        //public double[] A_z2 { get { return ((AOfZDetector)ResultsDictionary[TallyType.AOfZ]).SecondMoment; } }

        // public double[] A_layer { get { return ((ALayerDetector)ResultsDictionary[TallyType.ALayer]).Mean; } }
        // public double[] A_layer2 { get { return ((ALayerDetector)ResultsDictionary[TallyType.ALayer]).SecondMoment; } }

        //public double[] Flu_z { get { return ((FluenceOfZDetector)ResultsDictionary[TallyType.FluenceOfZ]).Mean; } }
        //public double[] Flu_z2 { get { return ((FluenceOfZDetector)ResultsDictionary[TallyType.FluenceOfZ]).SecondMoment; } }

        public double[] R_r { get { return ((ROfRhoDetector)ResultsDictionary[TallyType.ROfRho]).Mean; } }
        public double[] R_r2 { get { return ((ROfRhoDetector)ResultsDictionary[TallyType.ROfRho]).SecondMoment; } }

        public double[] R_a { get { return ((ROfAngleDetector)ResultsDictionary[TallyType.ROfAngle]).Mean; } }
        public double[] R_a2 { get { return ((ROfAngleDetector)ResultsDictionary[TallyType.ROfAngle]).SecondMoment; } }

        public double[] T_r { get { return ((TOfRhoDetector)ResultsDictionary[TallyType.TOfRho]).Mean; } }
        public double[] T_r2 { get { return ((TOfRhoDetector)ResultsDictionary[TallyType.TOfRho]).SecondMoment; } }

        public double[] T_a { get { return ((TOfAngleDetector)ResultsDictionary[TallyType.TOfAngle]).Mean; } }
        public double[] T_a2 { get { return ((TOfAngleDetector)ResultsDictionary[TallyType.TOfAngle]).SecondMoment; } }

        public double[,] A_rz { get { return ((AOfRhoAndZDetector)ResultsDictionary[TallyType.AOfRhoAndZ]).Mean; } }
        public double[,] A_rz2 { get { return ((AOfRhoAndZDetector)ResultsDictionary[TallyType.AOfRhoAndZ]).SecondMoment; } }

        public double[,] Flu_rz { get { return ((FluenceOfRhoAndZDetector)ResultsDictionary[TallyType.FluenceOfRhoAndZ]).Mean; } }
        public double[,] Flu_rz2 { get { return ((FluenceOfRhoAndZDetector)ResultsDictionary[TallyType.FluenceOfRhoAndZ]).SecondMoment; } }

        public double[,] R_ra { get { return ((ROfRhoAndAngleDetector)ResultsDictionary[TallyType.ROfRhoAndAngle]).Mean; } }
        public double[,] R_ra2 { get { return ((ROfRhoAndAngleDetector)ResultsDictionary[TallyType.ROfRhoAndAngle]).SecondMoment; } }

        public double[,] T_ra { get { return ((TOfRhoAndAngleDetector)ResultsDictionary[TallyType.TOfRhoAndAngle]).Mean; } }
        public double[,] T_ra2 { get { return ((TOfRhoAndAngleDetector)ResultsDictionary[TallyType.TOfRhoAndAngle]).SecondMoment; } }

        public double[,] R_xy { get { return ((ROfXAndYDetector)ResultsDictionary[TallyType.ROfXAndY]).Mean; } }
        public double[,] R_xy2 { get { return ((ROfXAndYDetector)ResultsDictionary[TallyType.ROfXAndY]).SecondMoment; } }

        public double[,] R_rt { get { return ((ROfRhoAndTimeDetector)ResultsDictionary[TallyType.ROfRhoAndTime]).Mean; } }
        public double[,] R_rt2 { get { return ((ROfRhoAndTimeDetector)ResultsDictionary[TallyType.ROfRhoAndTime]).SecondMoment; } }

        public Complex[,] R_rw { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.ROfRhoAndOmega]).Mean; } }
        public Complex[,] R_rw2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.ROfRhoAndOmega]).SecondMoment; } }

        //public double[, ,] A_rzt { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.AOfRhoAndZAndTime]).Mean; } }
        //public double[, ,] A_rzt2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.AOfRhoAndZAndTime]).SecondMoment; } }

        public double[, ,] Flu_rzt { get { return ((FluenceOfRhoAndZAndTimeDetector)ResultsDictionary[TallyType.FluenceOfRhoAndZAndTime]).Mean; } }
        public double[, ,] Flu_rzt2 { get { return ((FluenceOfRhoAndZAndTimeDetector)ResultsDictionary[TallyType.FluenceOfRhoAndZAndTime]).SecondMoment; } }

        public SimulationInput Input { get; private set; }

        public IDictionary<TallyType, IDetector> ResultsDictionary { get; private set; }
    }
}
