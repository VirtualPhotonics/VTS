using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Helper class to surface the results of a Monte Carlo simulation in a user-friendly (strongly-typed) way
    /// </summary>
    public class SimulationOutput
    {
        private IList<IDetector> _detectorResults;
        /// <summary>
        /// Output from a Monte Carlo simulation
        /// </summary>
        /// <param name="si">SimulationInput</param>
        /// <param name="detectorResults">list of IDetector</param>
        public SimulationOutput(SimulationInput si, IList<IDetector> detectorResults)
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
        /// <summary>
        /// Diffuse Reflectance
        /// </summary>
        public double Rd { get { return ((RDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RDiffuse).First().Name]).Mean; } }
        /// <summary>
        /// Diffuse Reflectance 2nd moment
        /// </summary>
        public double Rd2 { get { return ((RDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RDiffuse).First().Name]).SecondMoment; } }
        /// <summary>
        /// Specular Reflectance
        /// </summary>
        public double Rspec { get { return ((RSpecularDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RSpecular).First().Name]).Mean; } }
        /// <summary>
        /// Specular Reflectance 2nd moment
        /// </summary>
        public double Rspec2 { get { return ((RSpecularDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RSpecular).First().Name]).SecondMoment; } }

        //public double Rtot { get { return ((RTotalDetector)ResultsDictionary[TallyType.RTotal]).Mean; } }
        //public double Rtot2 { get { return ((RTotalDetector)ResultsDictionary[TallyType.RTotal]).SecondMoment; } }
        /// <summary>
        /// Diffuse Transmittance
        /// </summary>
        public double Td { get { return ((TDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TDiffuse).First().Name]).Mean; } }
        /// <summary>
        /// Diffuse Transmittance 2nd moment
        /// </summary>
        public double Td2 { get { return ((TDiffuseDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TDiffuse).First().Name]).SecondMoment; } }
        /// <summary>
        /// Total Absorbed Energy
        /// </summary>
        public double Atot { get { return ((ATotalDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ATotal).First().Name]).Mean; } }
        /// <summary>
        /// Total Absorbed Energy 2nd moment
        /// </summary>
        public double Atot2 { get { return ((ATotalDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ATotal).First().Name]).SecondMoment; } }

        //public double[] A_z { get { return ((AOfZDetector)ResultsDictionary[TallyType.AOfZ]).Mean; } }
        //public double[] A_z2 { get { return ((AOfZDetector)ResultsDictionary[TallyType.AOfZ]).SecondMoment; } }

        // public double[] A_layer { get { return ((ALayerDetector)ResultsDictionary[TallyType.ALayer]).Mean; } }
        // public double[] A_layer2 { get { return ((ALayerDetector)ResultsDictionary[TallyType.ALayer]).SecondMoment; } }

        //public double[] Flu_z { get { return ((FluenceOfZDetector)ResultsDictionary[TallyType.FluenceOfZ]).Mean; } }
        //public double[] Flu_z2 { get { return ((FluenceOfZDetector)ResultsDictionary[TallyType.FluenceOfZ]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation)
        /// </summary>
        public double[] R_r { get { return ((ROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRho).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] R_r2 { get { return ((ROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRho).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of angle
        /// </summary>
        public double[] R_a { get { return ((ROfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfAngle).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of angle 2nd moment
        /// </summary>
        public double[] R_a2 { get { return ((ROfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfAngle).First().Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation)
        /// </summary>
        public double[] T_r { get { return ((TOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRho).First().Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] T_r2 { get { return ((TOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRho).First().Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of angle
        /// </summary>
        public double[] T_a { get { return ((TOfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfAngle).First().Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of angle 2nd moment
        /// </summary>
        public double[] T_a2 { get { return ((TOfAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfAngle).First().Name]).SecondMoment; } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z
        /// </summary>
        public double[,] A_rz { get { return ((AOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.AOfRhoAndZ).First().Name]).Mean; } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z 2nd moment
        /// </summary>
        public double[,] A_rz2 { get { return ((AOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.AOfRhoAndZ).First().Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of rho and z
        /// </summary>
        public double[,] Flu_rz { get { return ((FluenceOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZ).First().Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of rho and z 2nd moment
        /// </summary>
        public double[,] Flu_rz2 { get { return ((FluenceOfRhoAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZ).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and angle
        /// </summary>
        public double[,] R_ra { get { return ((ROfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndAngle).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] R_ra2 { get { return ((ROfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndAngle).First().Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of rho and angle
        /// </summary>
        public double[,] T_ra { get { return ((TOfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRhoAndAngle).First().Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] T_ra2 { get { return ((TOfRhoAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.TOfRhoAndAngle).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y
        /// </summary>
        public double[,] R_xy { get { return ((ROfXAndYDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfXAndY).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y 2nd moment
        /// </summary>
        public double[,] R_xy2 { get { return ((ROfXAndYDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfXAndY).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and time
        /// </summary>
        public double[,] R_rt { get { return ((ROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndTime).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] R_rt2 { get { return ((ROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndTime).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency)
        /// </summary>
        public Complex[,] R_rw { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndOmega).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency) 2nd moment
        /// </summary>
        public Complex[,] R_rw2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfRhoAndOmega).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency
        /// </summary>
        public Complex[] R_fx { get { return ((ROfFxDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfFx).First().Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency 2nd moment
        /// </summary>
        public Complex[] R_fx2 { get { return ((ROfFxDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ROfFx).First().Name]).SecondMoment; } }

        //public double[, ,] A_rzt { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.AOfRhoAndZAndTime]).Mean; } }
        //public double[, ,] A_rzt2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary[TallyType.AOfRhoAndZAndTime]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of rho, z and time
        /// </summary>
        public double[, ,] Flu_rzt { get { return ((FluenceOfRhoAndZAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZAndTime).First().Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of rho, z and time 2nd moment
        /// </summary>
        public double[, ,] Flu_rzt2 { get { return ((FluenceOfRhoAndZAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfRhoAndZAndTime).First().Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of x, y and z
        /// </summary>
        public double[, ,] Flu_xyz { get { return ((FluenceOfXAndYAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfXAndYAndZ).First().Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of x, y and z 2nd moment
        /// </summary>
        public double[, ,] Flu_xyz2 { get { return ((FluenceOfXAndYAndZDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.FluenceOfXAndYAndZ).First().Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of rho (surface tally)
        /// </summary>
        public double[] Rad_r { get { return ((RadianceOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfRho).First().Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) 2nd moment
        /// </summary>
        public double[] Rad_r2 { get { return ((RadianceOfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfRho).First().Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally)
        /// </summary>
        public double[, ,] Rad_rza { get { return ((RadianceOfRhoAndZAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfRhoAndZAndAngle).First().Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[, ,] Rad_rza2 { get { return ((RadianceOfRhoAndZAndAngleDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfRhoAndZAndAngle).First().Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally)
        /// </summary>
        public double[,,,,] Rad_xyztp { get { return ((RadianceOfXAndYAndZAndThetaAndPhiDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfXAndYAndZAndThetaAndPhi).First().Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally) 2nd moment
        /// </summary>
        public double[,,,,] Rad_xyztp2 { get { return ((RadianceOfXAndYAndZAndThetaAndPhiDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.RadianceOfXAndYAndZAndThetaAndPhi).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] RefMT_rs_hist { get { return ((ReflectedMTOfRhoAndSubregionHistDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ReflectedMTOfRhoAndSubregionHist).First().Name]).Mean; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] RefMT_rs_hist2 { get { return ((ReflectedMTOfRhoAndSubregionHistDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ReflectedMTOfRhoAndSubregionHist).First().Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,] RefMT_rs_frac { get { return ((ReflectedMTOfRhoAndSubregionHistDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ReflectedMTOfRhoAndSubregionHist).First().Name]).FractionalMT; } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time
        /// </summary>
        public double[, ,] RefTime_rs_hist { get { return ((ReflectedTimeOfRhoAndSubregionHistDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ReflectedTimeOfRhoAndSubregionHist).First().Name]).Mean; } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time 2nd moment
        /// </summary>
        public double[, ,] RefTime_rs_hist2 { get { return ((ReflectedTimeOfRhoAndSubregionHistDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.ReflectedTimeOfRhoAndSubregionHist).First().Name]).SecondMoment; } }

        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time
        /// </summary>
        public double[,] pMC_R_rt { get { return ((pMCROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRhoAndTime).First().Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] pMC_R_rt2 { get { return ((pMCROfRhoAndTimeDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRhoAndTime).First().Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho 
        /// </summary>
        public double[] pMC_R_r { get { return ((pMCROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRho).First().Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho 2nd moment
        /// </summary>
        public double[] pMC_R_r2 { get { return ((pMCROfRhoDetector)ResultsDictionary[_detectorResults.Where(d => d.TallyType == TallyType.pMCROfRho).First().Name]).SecondMoment; } }
        /// <summary>
        /// Simulation Input that generated this SimulationOutput
        /// </summary>
        public SimulationInput Input { get; private set; }
        /// <summary>
        /// Dictionary holding detector results as specified in SimulationInput
        /// </summary>
        public IDictionary<String, IDetector> ResultsDictionary { get; private set; }

    }
}
