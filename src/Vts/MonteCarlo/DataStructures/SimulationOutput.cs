using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public double Rd { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RDiffuse").First().Name]).Mean); } }
        /// <summary>
        /// Diffuse Reflectance 2nd moment
        /// </summary>
        public double Rd2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RDiffuse").First().Name]).SecondMoment); } }
        /// <summary>
        /// Specular Reflectance
        /// </summary>
        public double Rspec { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RSpecular").First().Name]).Mean); } }
        /// <summary>
        /// Specular Reflectance 2nd moment
        /// </summary>
        public double Rspec2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RSpecular").First().Name]).SecondMoment); } }

        //public double Rtot { get { return ((RTotalDetector)ResultsDictionary["RTotal]).Mean; } }
        //public double Rtot2 { get { return ((RTotalDetector)ResultsDictionary["RTotal]).SecondMoment; } }
        /// <summary>
        /// Diffuse Transmittance
        /// </summary>
        public double Td { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TDiffuse").First().Name]).Mean); } }
        /// <summary>
        /// Diffuse Transmittance 2nd moment
        /// </summary>
        public double Td2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TDiffuse").First().Name]).SecondMoment); } }
        /// <summary>
        /// Total Absorbed Energy
        /// </summary>
        public double Atot { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ATotal").First().Name]).Mean); } }
        /// <summary>
        /// Total Absorbed Energy 2nd moment
        /// </summary>
        public double Atot2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ATotal").First().Name]).SecondMoment); } }

        //public double[] A_z { get { return ((AOfZDetector)ResultsDictionary["AOfZ]).Mean; } }
        //public double[] A_z2 { get { return ((AOfZDetector)ResultsDictionary["AOfZ]).SecondMoment; } }

        // public double[] A_layer { get { return ((ALayerDetector)ResultsDictionary["ALayer]).Mean; } }
        // public double[] A_layer2 { get { return ((ALayerDetector)ResultsDictionary["ALayer]).SecondMoment; } }

        //public double[] Flu_z { get { return ((FluenceOfZDetector)ResultsDictionary["FluenceOfZ]).Mean; } }
        //public double[] Flu_z2 { get { return ((FluenceOfZDetector)ResultsDictionary["FluenceOfZ]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation)
        /// </summary>
        public double[] R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRho").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] R_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRho").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of angle
        /// </summary>
        public double[] R_a { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfAngle").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of angle 2nd moment
        /// </summary>
        public double[] R_a2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation)
        /// </summary>
        public double[] T_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRho").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] T_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRho").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of angle
        /// </summary>
        public double[] T_a { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfAngle").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of angle 2nd moment
        /// </summary>
        public double[] T_a2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z
        /// </summary>
        public double[,] A_rz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfRhoAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z 2nd moment
        /// </summary>
        public double[,] A_rz2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfRhoAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of rho and z
        /// </summary>
        public double[,] Flu_rz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of rho and z 2nd moment
        /// </summary>
        public double[,] Flu_rz2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho and angle
        /// </summary>
        public double[,] R_ra { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] R_ra2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of rho and angle
        /// </summary>
        public double[,] T_ra { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRhoAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] T_ra2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRhoAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of x and y
        /// </summary>
        public double[,] T_xy { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfXAndY").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of x and y 2nd moment
        /// </summary>
        public double[,] T_xy2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfXAndY").First().Name]).SecondMoment); } }
        
        /// <summary>
        /// Reflectance as a function of x and y
        /// </summary>
        public double[,] R_xy { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfXAndY").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of x and y 2nd moment
        /// </summary>
        public double[,] R_xy2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfXAndY").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho and time
        /// </summary>
        public double[,] R_rt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndTime").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] R_rt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency)
        /// </summary>
        public Complex[,] R_rw { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndOmega").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency) 2nd moment
        /// </summary>
        public Complex[,] R_rw2 { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndOmega").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency
        /// </summary>
        public Complex[] R_fx { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFx").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency 2nd moment
        /// </summary>
        public Complex[] R_fx2 { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFx").First().Name]).SecondMoment); } }

        //public double[, ,] A_rzt { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary["AOfRhoAndZAndTime]).Mean; } }
        //public double[, ,] A_rzt2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary["AOfRhoAndZAndTime]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of rho, z and time
        /// </summary>
        public double[, ,] Flu_rzt { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndTime").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of rho, z and time 2nd moment
        /// </summary>
        public double[, ,] Flu_rzt2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of x, y and z
        /// </summary>
        public double[, ,] Flu_xyz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of x, y and z 2nd moment
        /// </summary>
        public double[, ,] Flu_xyz2 { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega
        /// </summary>
        public Complex[, , ,] Flu_xyzo { get { return ((Complex[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega 2nd moment
        /// </summary>
        public Complex[, , ,] Flu_xyzo2 { get { return ((Complex[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").First().Name]).SecondMoment); } }
        /// <summary>
        /// Absorption as a function of x, y and z
        /// </summary>
        public double[, ,] A_xyz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfXAndYAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Absorption as a function of x, y and z 2nd moment
        /// </summary>
        public double[, ,] A_xyz2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfXAndYAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z
        /// </summary>
        public double[] Rad_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAtZ").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z 2nd moment
        /// </summary>
        public double[] Rad_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAtZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally)
        /// </summary>
        public double[, ,] Rad_rza { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[, ,] Rad_rza2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of fx, z and angle (volume tally)
        /// </summary>
        public double[, ,] Rad_fxza { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfFxAndZAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[, ,] Rad_fxza2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfFxAndZAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally)
        /// </summary>
        public double[, , , ,] Rad_xyztp { get { return ((double[, , , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally) 2nd moment
        /// </summary>
        public double[, , , ,] Rad_xyztp2 { get { return ((double[, , , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] RefMT_rmt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] RefMT_rmt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, , ,] RefMT_rmt_frac { get { return ((double[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,,] RefMT_xymt { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,,] RefMT_xymt2 { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,,] RefMT_xymt_frac { get { return ((double[,,,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] TransMT_rmt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] TransMT_rmt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, , ,] TransMT_rmt_frac { get { return ((double[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Transmitted Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,,] TransMT_xymt { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,,] TransMT_xymt2 { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,,] TransMT_xymt_frac { get { return ((double[,,,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] RefDynMT_rmt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] RefDynMT_rmt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, ,] RefDynMT_rmt_frac { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,] RefDynMT_rmt_totofz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,] RefDynMT_rmt_dynofz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).DynamicMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[, ,] RefDynMT_xymt { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[, ,] RefDynMT_xymt2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, , ,] RefDynMT_xymt_frac { get { return ((double[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,,] RefDynMT_xymt_totofz { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,,] RefDynMT_xymt_dynofz { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).DynamicMTOfZ); } }   
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] TransDynMT_rmt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] TransDynMT_rmt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, ,] TransDynMT_rmt_frac { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,] TransDynMT_rmt_totofz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,] TransDynMT_rmt_dynofz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).DynamicMTOfZ); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[, ,] TransDynMT_xymt { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[, ,] TransDynMT_xymt2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, , ,] TransDynMT_xymt_frac { get { return ((double[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[, ,] TransDynMT_xymt_totofz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[, ,] TransDynMT_xymt_dynofz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).DynamicMTOfZ); } }   
        
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time
        /// </summary>
        public double[, ,] RefTime_rs_hist { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time 2nd moment
        /// </summary>
        public double[, ,] RefTime_rs_hist2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").First().Name]).SecondMoment); } }

        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time
        /// </summary>
        public double[,] pMC_R_rt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRhoAndTime").First().Name]).Mean); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] pMC_R_rt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRhoAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho 
        /// </summary>
        public double[] pMC_R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRho").First().Name]).Mean); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho 2nd moment
        /// </summary>
        public double[] pMC_R_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRho").First().Name]).SecondMoment); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua
        /// </summary>
        public double[] dMCdMua_R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMua").First().Name]).Mean); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus
        /// </summary>
        public double[] dMCdMus_R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMus").First().Name]).Mean); } }
       
        /// <summary>
        /// Simulation Input that generated this SimulationOutput
        /// </summary>
        public SimulationInput Input { get; private set; }
        /// <summary>
        /// Dictionary holding detector results as specified in SimulationInput
        /// </summary>
        public IDictionary<String, IDetector> ResultsDictionary { get; private set; }

        public IEnumerable<IDetector> GetDetectors(IEnumerable<string> detectorNames)
        {
            foreach (var detectorName in detectorNames)
            {
                var detector = GetDetector(detectorName);

                if (detector != null)
                {
                    yield return detector;
                }
            }
        }

        public IDetector GetDetector(string detectorName)
        {
            IDetector detector;

            ResultsDictionary.TryGetValue(detectorName, out detector);

            return detector;
        }

    }
}
