using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Helper class to surface the results of a Monte Carlo simulation in a user-friendly (strongly-typed) way
    /// </summary>
    public class Output
    {
        private IList<IDetector> _detectorResults;
        public Output(SimulationInput si, IList<IDetector> detectorResults)
        {
            int count = 1;
            Input = si;
            ResultsDictionary = new Dictionary<String, IDetector>();
            foreach (var detector in detectorResults)
            {
                try
                {
                    ResultsDictionary.Add(detector.Name, detector);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problem adding detector results to dictionary.\n\nDetails:\n\n" + e + "\n");
                    if (e is ArgumentException)
                    {
                        Console.WriteLine("detector with that name already exists in dictionary\n");
                        Console.WriteLine("Adding detector with name = " + detector.Name + count + " instead.\n");
                        string newName = detector.Name + count;
                        ResultsDictionary.Add(newName, detector);
                        ++count;
                    }
                }
            }
            //ResultsDictionary = detectorResults.ToDictionary(d => d.Name);
            _detectorResults = detectorResults;
        }

        public double Rd { get { return ((RDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RDiffuse).First().Name]).Mean; } }
        public double Rd2 { get { return ((RDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RDiffuse).First().Name]).SecondMoment; } }
        
        public double Rspec { get { return ((RSpecularDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RSpecular).First().Name]).Mean; } }
        public double Rspec2 { get { return ((RSpecularDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RSpecular).First().Name]).SecondMoment; } }

        //public double Rtot { get { return ((RTotalDetector)ResultsDictionary[TallyType.RTotal]).Mean; } }
        //public double Rtot2 { get { return ((RTotalDetector)ResultsDictionary[TallyType.RTotal]).SecondMoment; } }

        public double Td { get { return ((TDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TDiffuse).First().Name]).Mean; } }
        public double Td2 { get { return ((TDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TDiffuse).First().Name]).SecondMoment; } }

        public double Atot { get { return ((ATotalDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ATotal).First().Name]).Mean; } }
        public double Atot2 { get { return ((ATotalDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ATotal).First().Name]).SecondMoment; } }

        //public double[] A_z { get { return ((AOfZDetector)ResultsDictionary[TallyType.AOfZ]).Mean; } }
        //public double[] A_z2 { get { return ((AOfZDetector)ResultsDictionary[TallyType.AOfZ]).SecondMoment; } }

        // public double[] A_layer { get { return ((ALayerDetector)ResultsDictionary[TallyType.ALayer]).Mean; } }
        // public double[] A_layer2 { get { return ((ALayerDetector)ResultsDictionary[TallyType.ALayer]).SecondMoment; } }

        //public double[] Flu_z { get { return ((FluenceOfZDetector)ResultsDictionary[TallyType.FluenceOfZ]).Mean; } }
        //public double[] Flu_z2 { get { return ((FluenceOfZDetector)ResultsDictionary[TallyType.FluenceOfZ]).SecondMoment; } }

        public double[] R_r { get { return ((ROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRho).First().Name]).Mean; } }
        public double[] R_r2 { get { return ((ROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRho).First().Name]).SecondMoment; } }

        public double[] R_a { get { return ((ROfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfAngle).First().Name]).Mean; } }
        public double[] R_a2 { get { return ((ROfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfAngle).First().Name]).SecondMoment; } }

        public double[] T_r { get { return ((TOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRho).First().Name]).Mean; } }
        public double[] T_r2 { get { return ((TOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRho).First().Name]).SecondMoment; } }

        public double[] T_a { get { return ((TOfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfAngle).First().Name]).Mean; } }
        public double[] T_a2 { get { return ((TOfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfAngle).First().Name]).SecondMoment; } }

        public double[,] A_rz { get { return ((AOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.AOfRhoAndZ).First().Name]).Mean; } }
        public double[,] A_rz2 { get { return ((AOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.AOfRhoAndZ).First().Name]).SecondMoment; } }

        public double[,] Flu_rz { get { return ((FluenceOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZ).First().Name]).Mean; } }
        public double[,] Flu_rz2 { get { return ((FluenceOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZ).First().Name]).SecondMoment; } }

        public double[,] R_ra { get { return ((ROfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndAngle).First().Name]).Mean; } }
        public double[,] R_ra2 { get { return ((ROfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndAngle).First().Name]).SecondMoment; } }

        public double[,] T_ra { get { return ((TOfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRhoAndAngle).First().Name]).Mean; } }
        public double[,] T_ra2 { get { return ((TOfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRhoAndAngle).First().Name]).SecondMoment; } }

        public double[,] R_xy { get { return ((ROfXAndYDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfXAndY).First().Name]).Mean; } }
        public double[,] R_xy2 { get { return ((ROfXAndYDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfXAndY).First().Name]).SecondMoment; } }

        public double[,] R_rt { get { return ((ROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndTime).First().Name]).Mean; } }
        public double[,] R_rt2 { get { return ((ROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndTime).First().Name]).SecondMoment; } }

        public Complex[,] R_rw { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndOmega).First().Name]).Mean; } }
        public Complex[,] R_rw2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndOmega).First().Name]).SecondMoment; } }

        //public double[, ,] A_rzt { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.AOfRhoAndZAndTime]).Mean; } }
        //public double[, ,] A_rzt2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.AOfRhoAndZAndTime]).SecondMoment; } }

        public double[, ,] Flu_rzt { get { return ((FluenceOfRhoAndZAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZAndTime).First().Name]).Mean; } }
        public double[, ,] Flu_rzt2 { get { return ((FluenceOfRhoAndZAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZAndTime).First().Name]).SecondMoment; } }
        
        public double[] Curr_r { get { return ((RadianceOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfRho).First().Name]).Mean; } }
        public double[] Curr_r2 { get { return ((RadianceOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfRho).First().Name]).SecondMoment; } }

        public double[,] pMC_R_rt { get { return ((pMCROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRhoAndTime).First().Name]).Mean; } }
        public double[,] pMC_R_rt2 { get { return ((pMCROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRhoAndTime).First().Name]).SecondMoment; } }
        
        public double[] pMC_R_r { get { return ((pMCROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRho).First().Name]).Mean; } }
        public double[] pMC_R_r2 { get { return ((pMCROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRho).First().Name]).SecondMoment; } }

        public SimulationInput Input { get; private set; }

        public IDictionary<String, IDetector> ResultsDictionary { get; private set; }

    }
}
