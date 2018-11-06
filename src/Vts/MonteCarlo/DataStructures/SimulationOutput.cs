using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NLog.LayoutRenderers;

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
        /// Diffuse Reflectance TallyCount
        /// </summary>
        public long Rd_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RDiffuse").First().Name]).TallyCount); } }
        /// <summary>
        /// Specular Reflectance
        /// </summary>
        public double Rspec { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RSpecular").First().Name]).Mean); } }
        /// <summary>
        /// Specular Reflectance 2nd moment
        /// </summary>
        public double Rspec2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RSpecular").First().Name]).SecondMoment); } }
        /// <summary>
        /// Specular Reflectance Tally Count
        /// </summary>
        public long Rspec_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RSpecular").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation)
        /// </summary>
        public double[] R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRho").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] R_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRho").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) Tally Count
        /// </summary>
        public long R_r_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRho").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of angle
        /// </summary>
        public double[] R_a { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfAngle").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of angle 2nd moment
        /// </summary>
        public double[] R_a2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of angle Tally Count
        /// </summary>
        public long R_a_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfAngle").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of rho and angle
        /// </summary>
        public double[,] R_ra { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] R_ra2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho and angle Tally Count
        /// </summary>
        public long R_ra_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndAngle").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of rho and time
        /// </summary>
        public double[,] R_rt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndTime").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] R_rt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho and time Tally Count
        /// </summary>
        public long R_rt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndTime").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency)
        /// </summary>
        public Complex[,] R_rw { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndOmega").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency) 2nd moment
        /// </summary>
        public Complex[,] R_rw2 { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndOmega").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency) Tally Count
        /// </summary>
        public long R_rw_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfRhoAndOmega").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of x and y
        /// </summary>
        public double[,] R_xy { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfXAndY").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of x and y 2nd moment
        /// </summary>
        public double[,] R_xy2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfXAndY").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of x and y Tally Count
        /// </summary>
        public long R_xy_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfXAndY").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency
        /// </summary>
        public Complex[] R_fx { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFx").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency 2nd moment
        /// </summary>
        public Complex[] R_fx2 { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFx").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency Tally Count
        /// </summary>
        public long R_fx_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFx").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time
        /// </summary>
        public Complex[,] R_fxt { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFxAndTime").First().Name]).Mean); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time 2nd moment
        /// </summary>
        public Complex[,] R_fxt2 { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFxAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time Tally Count
        /// </summary>
        public long R_fxt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ROfFxAndTime").First().Name]).TallyCount); } }
        /// <summary>
        /// Diffuse Transmittance
        /// </summary>
        public double Td { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TDiffuse").First().Name]).Mean); } }
        /// <summary>
        /// Diffuse Transmittance 2nd moment
        /// </summary>
        public double Td2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TDiffuse").First().Name]).SecondMoment); } }
        /// <summary>
        /// Diffuse Transmittance Tally Count
        /// </summary>
        public long Td_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TDiffuse").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation)
        /// </summary>
        public double[] T_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRho").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] T_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRho").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation) Tally Count
        /// </summary>
        public long T_r_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRho").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmittance as a function of angle
        /// </summary>
        public double[] T_a { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfAngle").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of angle 2nd moment
        /// </summary>
        public double[] T_a2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of angle Tally Count
        /// </summary>
        public long T_a_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfAngle").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmittance as a function of rho and angle
        /// </summary>
        public double[,] T_ra { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRhoAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] T_ra2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRhoAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of rho and angle Tally Count
        /// </summary>
        public long T_ra_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfRhoAndAngle").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmittance as a function of x and y
        /// </summary>
        public double[,] T_xy { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfXAndY").First().Name]).Mean); } }
        /// <summary>
        /// Transmittance as a function of x and y 2nd moment
        /// </summary>
        public double[,] T_xy2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfXAndY").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmittance as a function of x and y Tally Count
        /// </summary>
        public long T_xy_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfXAndY").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmittance as a function of spatial frequency
        /// </summary>
        public Complex[] T_fx { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfFx").First().Name]).Mean); } }
        /// <summary>
        /// Transmitance as a function of spatial frequency 2nd moment
        /// </summary>
        public Complex[] T_fx2 { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfFx").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitance as a function of spatial frequency Tally Count
        /// </summary>
        public long T_fx_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TOfFx").First().Name]).TallyCount); } }
        /// <summary>
        /// Total Absorbed Energy
        /// </summary>
        public double Atot { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ATotal").First().Name]).Mean); } }
        /// <summary>
        /// Total Absorbed Energy 2nd moment
        /// </summary>
        public double Atot2 { get { return ((double)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ATotal").First().Name]).SecondMoment); } }
        /// <summary>
        /// Total Absorbed Energy Tally Count
        /// </summary>
        public long Atot_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ATotal").First().Name]).TallyCount); } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z
        /// </summary>
        public double[,] A_rz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfRhoAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z 2nd moment
        /// </summary>
        public double[,] A_rz2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfRhoAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z Tally Count
        /// </summary>
        public long A_rz_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfRhoAndZ").First().Name]).TallyCount); } }
        /// <summary>
        /// Absorption as a function of x, y and z
        /// </summary>
        public double[, ,] A_xyz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfXAndYAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Absorption as a function of x, y and z 2nd moment
        /// </summary>
        public double[, ,] A_xyz2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfXAndYAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Absorption as a function of x, y and z Tally Count
        /// </summary>
        public long A_xyz_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "AOfXAndYAndZ").First().Name]).TallyCount); } }
        /// <summary>
        /// Fluence as a function of rho and z
        /// </summary>
        public double[,] Flu_rz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of rho and z 2nd moment
        /// </summary>
        public double[,] Flu_rz2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of rho and z Tally Count
        /// </summary>
        public long Flu_rz_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZ").First().Name]).TallyCount); } }
        /// <summary>
        /// Fluence as a function of rho, z and time
        /// </summary>
        public double[, ,] Flu_rzt { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndTime").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of rho, z and time 2nd moment
        /// </summary>
        public double[, ,] Flu_rzt2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of rho, z and time Tally Count
        /// </summary>
        public long Flu_rzt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndTime").First().Name]).TallyCount); } }
        /// <summary>
        /// Fluence as a function of x, y and z
        /// </summary>
        public double[, ,] Flu_xyz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of x, y and z 2nd moment
        /// </summary>
        public double[, ,] Flu_xyz2 { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of x, y and z Tally Count
        /// </summary>
        public long Flu_xyz_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZ").First().Name]).TallyCount); } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega (temporal frequency)
        /// </summary>
        public Complex[, , ,] Flu_xyzw { get { return ((Complex[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega 2nd moment
        /// </summary>
        public Complex[, , ,] Flu_xyzw2 { get { return ((Complex[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega Tally Count
        /// </summary>
        public long Flu_xyzw_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").First().Name]).TallyCount); } }
        /// <summary>
        /// Fluence as a function of rho, z and omega (temporal frequency)
        /// </summary>
        public Complex[,,] Flu_rzw { get { return ((Complex[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndOmega").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of rho, z and omega 2nd moment
        /// </summary>
        public Complex[,,] Flu_rzw2 { get { return ((Complex[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndOmega").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of fx and z Tally Count
        /// </summary>
        public long Flu_rzw_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfRhoAndZAndOmega").First().Name]).TallyCount); } }
        /// <summary>
        /// Fluence as a function of fx (spatial-frequency) and z
        /// </summary>
        public Complex[,] Flu_fxz { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfFxAndZ").First().Name]).Mean); } }
        /// <summary>
        /// Fluence as a function of fx and z2nd moment
        /// </summary>
        public Complex[,] Flu_fxz2 { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfFxAndZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Fluence as a function of fx and omega Tally Count
        /// </summary>
        public long Flu_fxz_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "FluenceOfFxAndZ").First().Name]).TallyCount); } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z
        /// </summary>
        public double[] Rad_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAtZ").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z 2nd moment
        /// </summary>
        public double[] Rad_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAtZ").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z Tally Count
        /// </summary>
        public long Rad_r_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAtZ").First().Name]).TallyCount); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally)
        /// </summary>
        public double[, ,] Rad_rza { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[, ,] Rad_rza2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) Tally Count
        /// </summary>
        public long Rad_rza_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First().Name]).TallyCount); } }
        /// <summary>
        /// Radiance as a function of fx, z and angle (volume tally)
        /// </summary>
        public double[, ,] Rad_fxza { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfFxAndZAndAngle").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[, ,] Rad_fxza2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfFxAndZAndAngle").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) Tally Count
        /// </summary>
        public long Rad_fxza_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfFxAndZAndAngle").First().Name]).TallyCount); } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally)
        /// </summary>
        public double[, , , ,] Rad_xyztp { get { return ((double[, , , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First().Name]).Mean); } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally) 2nd moment
        /// </summary>
        public double[, , , ,] Rad_xyztp2 { get { return ((double[, , , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First().Name]).SecondMoment); } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally) Tally Count
        /// </summary>
        public long Rad_xyztp_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] RefMT_rmt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] RefMT_rmt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefMT_rmt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First().Name]).TallyCount); } }
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
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefMT_xymt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First().Name]).TallyCount); } }
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
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransMT_rmt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").First().Name]).TallyCount); } }
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
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransMT_xymt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").First().Name]).TallyCount); } }
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
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefDynMT_rmt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).TallyCount); } }
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
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] RefDynMT_rmt_subrcols { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First().Name]).SubregionCollisions); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[, ,] RefDynMT_xymt { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[, ,] RefDynMT_xymt2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefDynMT_xymt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).TallyCount); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, , ,] RefDynMT_xymt_frac { get { return ((double[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,,] RefDynMT_xymt_totofz { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,,] RefDynMT_xymt_dynofz { get { return ((double[,,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).DynamicMTOfZ); } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] RefDynMT_xymt_subrcols { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First().Name]).SubregionCollisions); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] TransDynMT_rmt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] TransDynMT_rmt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransDynMT_rmt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, ,] TransDynMT_rmt_frac { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,] TransDynMT_rmt_totofz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,] TransDynMT_rmt_dynofz { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).DynamicMTOfZ); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] TransDynMT_rmt_subrcols { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).SubregionCollisions); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[, ,] TransDynMT_xymt { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[, ,] TransDynMT_xymt2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransDynMT_xymt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).TallyCount); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[, , ,] TransDynMT_xymt_frac { get { return ((double[, , ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First().Name]).FractionalMT); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[, ,] TransDynMT_xymt_totofz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).TotalMTOfZ); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[, ,] TransDynMT_xymt_dynofz { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).DynamicMTOfZ); } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] TransDynMT_xymt_subrcols { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First().Name]).SubregionCollisions); } }
  
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time
        /// </summary>
        public double[, ,] RefTime_rs_hist { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").First().Name]).Mean); } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time 2nd moment
        /// </summary>
        public double[, ,] RefTime_rs_hist2 { get { return ((double[, ,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").First().Name]).SecondMoment); } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time Tally Count
        /// </summary>
        public long RefTime_rs_hist_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").First().Name]).TallyCount); } }

        /// <summary>
        /// perturbation MC Reflectance as a function of rho 
        /// </summary>
        public double[] pMC_R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRho").First().Name]).Mean); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho 2nd moment
        /// </summary>
        public double[] pMC_R_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRho").First().Name]).SecondMoment); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho Tally Count
        /// </summary>
        public long pMC_R_r_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRho").First().Name]).TallyCount); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time
        /// </summary>
        public double[,] pMC_R_rt { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRhoAndTime").First().Name]).Mean); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] pMC_R_rt2 { get { return ((double[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRhoAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time Tally Count
        /// </summary>
        public long pMC_R_rt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfRhoAndTime").First().Name]).TallyCount); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua
        /// </summary>
        public double[] dMCdMua_R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMua").First().Name]).Mean); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua 2nd moment
        /// </summary>
        public double[] dMCdMua_R_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMua").First().Name]).SecondMoment); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua Tally Count
        /// </summary>
        public long dMCdMua_R_r_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMua").First().Name]).TallyCount); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus
        /// </summary>
        public double[] dMCdMus_R_r { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMus").First().Name]).Mean); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus 2nd moment
        /// </summary>
        public double[] dMCdMus_R_r2 { get { return ((double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMus").First().Name]).SecondMoment); } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus Tally Count
        /// </summary>
        public long dMCdMus_R_r_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "dMCdROfRhodMus").First().Name]).TallyCount); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx
        /// </summary>
        public Complex[] pMC_R_fx { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfFx").First().Name]).Mean); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx 2nd moment
        /// </summary>
        public Complex[] pMC_R_fx2 { get { return ((Complex[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfFx").First().Name]).SecondMoment); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx Tally Count
        /// </summary>
        public long pMC_R_fx_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfFx").First().Name]).TallyCount); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx and time
        /// </summary>
        public Complex[,] pMC_R_fxt { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfFxAndTime").First().Name]).Mean); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx and time 2nd moment
        /// </summary>
        public Complex[,] pMC_R_fxt2 { get { return ((Complex[,])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfFxAndTime").First().Name]).SecondMoment); } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx and time Tally count
        /// </summary>
        public long pMC_R_fxt_TallyCount { get { return ((long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCROfFxAndTime").First().Name]).TallyCount); } }

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
