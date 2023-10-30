using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Helper class to surface the results of a Monte Carlo simulation in a user-friendly (strongly-typed) way
    /// </summary>
    public class SimulationOutput
    {
        private readonly IList<IDetector> _detectorResults;
        /// <summary>
        /// Output from a Monte Carlo simulation
        /// </summary>
        /// <param name="si">SimulationInput</param>
        /// <param name="detectorResults">list of IDetector</param>
        public SimulationOutput(SimulationInput si, IList<IDetector> detectorResults)
        {
            Input = si;
            ResultsDictionary = new Dictionary<String, IDetector>();
            foreach (var detector in detectorResults)
            {
                // verification that all detectors have unique names performed in
                // SimulationInputValidation
                ResultsDictionary.Add(detector.Name, detector);
            }
            _detectorResults = detectorResults;
        }
        /// <summary>
        /// Slanted Recessed fiber
        /// </summary>
        public double SlantedFib { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "SlantedRecessedFiber").Name]).Mean; } }
        /// <summary>
        /// Slanted Recessed fiber 2nd moment
        /// </summary>
        public double SlantedFib2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "SlantedRecessedFiber").Name]).SecondMoment; } }
        /// <summary>
        /// Slanted fiber tally count
        /// </summary>
        public double SlantedFib_TallyCount { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "SlantedRecessedFiber").Name]).TallyCount; } }
        /// <summary>
        /// Surface fiber
        /// </summary>
        public double SurFib { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "SurfaceFiber").Name]).Mean; } }
        /// <summary>
        /// Surface fiber 2nd moment
        /// </summary>
        public double SurFib2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "SurfaceFiber").Name]).SecondMoment; } }
        /// <summary>
        /// Surface fiber tally count
        /// </summary>
        public double SurFib_TallyCount { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "SurfaceFiber").Name]).TallyCount; } }

        /// <summary>
        /// Diffuse Reflectance
        /// </summary>
        public double Rd { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RDiffuse").Name]).Mean; } }
        /// <summary>
        /// Diffuse Reflectance 2nd moment
        /// </summary>
        public double Rd2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RDiffuse").Name]).SecondMoment; } }
        /// <summary>
        /// Diffuse Reflectance TallyCount
        /// </summary>
        public long Rd_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RDiffuse").Name]).TallyCount; } }
        /// <summary>
        /// Specular Reflectance
        /// </summary>
        public double Rspec { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RSpecular").Name]).Mean; } }
        /// <summary>
        /// Specular Reflectance 2nd moment
        /// </summary>
        public double Rspec2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RSpecular").Name]).SecondMoment; } }
        /// <summary>
        /// Specular Reflectance Tally Count
        /// </summary>
        public long Rspec_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RSpecular").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation)
        /// </summary>
        public double[] R_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRho").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] R_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRho").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) Tally Count
        /// </summary>
        public long R_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRho").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) recessed in air
        /// </summary>
        public double[] R_rr { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoRecessed").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] R_rr2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho (source-detector separation) Tally Count
        /// </summary>
        public long R_rr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoRecessed").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of angle
        /// </summary>
        public double[] R_a { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfAngle").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of angle 2nd moment
        /// </summary>
        public double[] R_a2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of angle Tally Count
        /// </summary>
        public long R_a_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfAngle").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho and angle
        /// </summary>
        public double[,] R_ra { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndAngle").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] R_ra2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and angle Tally Count
        /// </summary>
        public long R_ra_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndAngle").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho and time
        /// </summary>
        public double[,] R_rt { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndTime").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] R_rt2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and time Tally Count
        /// </summary>
        public long R_rt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndTime").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho and maximum depth attained
        /// </summary>
        public double[,] R_rmd { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndMaxDepth").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and max depth 2nd moment
        /// </summary>
        public double[,] R_rmd2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndMaxDepth").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and max depth Tally Count
        /// </summary>
        public long R_rmd_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndMaxDepth").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho and maximum depth attained recessed in air
        /// </summary>
        public double[,] R_rmdr { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndMaxDepthRecessed").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and max depth 2nd moment
        /// </summary>
        public double[,] R_rmdr2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndMaxDepthRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and max depth Tally Count
        /// </summary>
        public long R_rmdr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndMaxDepthRecessed").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency)
        /// </summary>
        public Complex[,] R_rw { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndOmega").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency) 2nd moment
        /// </summary>
        public Complex[,] R_rw2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndOmega").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of rho and omega (temporal frequency) Tally Count
        /// </summary>
        public long R_rw_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfRhoAndOmega").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y
        /// </summary>
        public double[,] R_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndY").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y 2nd moment
        /// </summary>
        public double[,] R_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndY").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y Tally Count
        /// </summary>
        public long R_xy_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndY").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y recessed in air
        /// </summary>
        public double[,] R_xyr { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYRecessed").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y recessed in air 2nd moment
        /// </summary>
        public double[,] R_xyr2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y recessed in air Tally Count
        /// </summary>
        public long R_xyr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYRecessed").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and time
        /// </summary>
        public double[,,] R_xyt { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTime").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y and time 2nd moment
        /// </summary>
        public double[,,] R_xyt2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and time Tally Count
        /// </summary>
        public long R_xyt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTime").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and time recessed in air
        /// </summary>
        public double[,,] R_xytr { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeRecessed").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y and time recessed in air 2nd moment
        /// </summary>
        public double[,,] R_xytr2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and time recessed in air Tally Count
        /// </summary>
        public long R_xytr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeRecessed").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion
        /// </summary>
        public double[,,,] R_xyts { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregion").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y 2nd moment
        /// </summary>
        public double[,,,] R_xyts2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregion").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion ROfXAndY
        /// </summary>
        public double[,] R_xyts_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregion").Name]).ROfXAndY; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion ROfXAndY 2nd moment
        /// </summary>
        public double[,] R_xyts_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregion").Name]).ROfXAndYSecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and subregion Tally Count
        /// </summary>
        public long R_xyts_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregion").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion recessed in air
        /// </summary>
        public double[,,,] R_xytsr { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregionRecessed").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion recessed ROfXAndY
        /// </summary>
        public double[,] R_xytsr_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregionRecessed").Name]).ROfXAndY; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion recessed ROfXAndY 2nd moment
        /// </summary>
        public double[,] R_xytsr_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregionRecessed").Name]).ROfXAndYSecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion recessed in air 2nd moment
        /// </summary>
        public double[,,,] R_xytsr2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregionRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and time and subregion recessed in air Tally Count
        /// </summary>
        public long R_xytsr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndTimeAndSubregionRecessed").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and theta and phi
        /// </summary>
        public double[,,,] R_xytp { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndThetaAndPhi").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y and theta and phi 2nd moment
        /// </summary>
        public double[,,,] R_xytp2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndThetaAndPhi").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and theta and phi Tally Count
        /// </summary>
        public long R_xytp_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndThetaAndPhi").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and max depth
        /// </summary>
        public double[,,] R_xymd { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndMaxDepth").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y and max depth 2nd moment
        /// </summary>
        public double[,,] R_xymd2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndMaxDepth").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and max depth Tally Count
        /// </summary>
        public long R_xymd_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndMaxDepth").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of x and y and max depth recessed in air
        /// </summary>
        public double[,,] R_xymdr { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndMaxDepthRecessed").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of x and y and max depth recessed in air 2nd moment
        /// </summary>
        public double[,,] R_xymdr2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndMaxDepthRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of x and y and max depth recessed in air Tally Count
        /// </summary>
        public long R_xymdr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfXAndYAndMaxDepthRecessed").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency
        /// </summary>
        public Complex[] R_fx { get { return (Complex[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFx").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency 2nd moment
        /// </summary>
        public Complex[] R_fx2 { get { return (Complex[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFx").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency Tally Count
        /// </summary>
        public long R_fx_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFx").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time
        /// </summary>
        public Complex[,] R_fxt { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndTime").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time 2nd moment
        /// </summary>
        public Complex[,] R_fxt2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time Tally Count
        /// </summary>
        public long R_fxt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndTime").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and angle
        /// </summary>
        public Complex[,] R_fxa { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndAngle").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time 2nd moment
        /// </summary>
        public Complex[,] R_fxa2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and time Tally Count
        /// </summary>
        public long R_fxa_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndAngle").Name]).TallyCount; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and maximum depth
        /// </summary>
        public Complex[,] R_fxmd { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndMaxDepth").Name]).Mean; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and maximum depth 2nd moment
        /// </summary>
        public Complex[,] R_fxmd2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndMaxDepth").Name]).SecondMoment; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency and maximum depth Tally Count
        /// </summary>
        public long R_fxmd_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ROfFxAndMaxDepth").Name]).TallyCount; } }
        /// <summary>
        /// Diffuse Transmittance
        /// </summary>
        public double Td { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TDiffuse").Name]).Mean; } }
        /// <summary>
        /// Diffuse Transmittance 2nd moment
        /// </summary>
        public double Td2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TDiffuse").Name]).SecondMoment; } }
        /// <summary>
        /// Diffuse Transmittance Tally Count
        /// </summary>
        public long Td_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TDiffuse").Name]).TallyCount; } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation)
        /// </summary>
        public double[] T_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfRho").Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation) 2nd moment
        /// </summary>
        public double[] T_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfRho").Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of rho (source-detector separation) Tally Count
        /// </summary>
        public long T_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfRho").Name]).TallyCount; } }
        /// <summary>
        /// Transmittance as a function of angle
        /// </summary>
        public double[] T_a { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfAngle").Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of angle 2nd moment
        /// </summary>
        public double[] T_a2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of angle Tally Count
        /// </summary>
        public long T_a_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfAngle").Name]).TallyCount; } }
        /// <summary>
        /// Transmittance as a function of rho and angle
        /// </summary>
        public double[,] T_ra { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfRhoAndAngle").Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of rho and angle 2nd moment
        /// </summary>
        public double[,] T_ra2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfRhoAndAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of rho and angle Tally Count
        /// </summary>
        public long T_ra_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfRhoAndAngle").Name]).TallyCount; } }
        /// <summary>
        /// Transmittance as a function of x and y
        /// </summary>
        public double[,] T_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndY").Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of x and y 2nd moment
        /// </summary>
        public double[,] T_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndY").Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of x and y Tally Count
        /// </summary>
        public long T_xy_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndY").Name]).TallyCount; } }
        /// <summary>
        /// Transmittance as a function of x and y and time and subregion
        /// </summary>
        public double[,,,] T_xyts { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndYAndTimeAndSubregion").Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of x and y 2nd moment
        /// </summary>
        public double[,,,] T_xyts2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndYAndTimeAndSubregion").Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of x and y and time and subregion ROfXAndY
        /// </summary>
        public double[,] T_xyts_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndYAndTimeAndSubregion").Name]).TOfXAndY; } }
        /// <summary>
        /// Transmittance as a function of x and y and time and subregion ROfXAndY 2nd moment
        /// </summary>
        public double[,] T_xyts_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndYAndTimeAndSubregion").Name]).TOfXAndYSecondMoment; } }
        /// <summary>
        /// Transmittance as a function of x and y and subregion Tally Count
        /// </summary>
        public long T_xyts_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfXAndYAndTimeAndSubregion").Name]).TallyCount; } }
        /// <summary>
        /// Transmittance as a function of spatial frequency
        /// </summary>
        public Complex[] T_fx { get { return (Complex[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfFx").Name]).Mean; } }
        /// <summary>
        /// Transmittance as a function of spatial frequency 2nd moment
        /// </summary>
        public Complex[] T_fx2 { get { return (Complex[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfFx").Name]).SecondMoment; } }
        /// <summary>
        /// Transmittance as a function of spatial frequency Tally Count
        /// </summary>
        public long T_fx_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TOfFx").Name]).TallyCount; } }
        /// <summary>
        /// Total Absorbed Energy
        /// </summary>
        public double Atot { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ATotal").Name]).Mean; } }
        /// <summary>
        /// Total Absorbed Energy 2nd moment
        /// </summary>
        public double Atot2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ATotal").Name]).SecondMoment; } }
        /// <summary>
        /// Total Absorbed Energy Tally Count
        /// </summary>
        public long Atot_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ATotal").Name]).TallyCount; } }
        /// <summary>
        /// Total Absorbed Energy in bounding volume
        /// </summary>
        public double AtotBV { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ATotalBoundingVolume").Name]).Mean; } }
        /// <summary>
        /// Total Absorbed Energy in bounding volume 2nd moment
        /// </summary>
        public double AtotBV2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ATotalBoundingVolume").Name]).SecondMoment; } }
        /// <summary>
        /// Total Absorbed Energy in bounding volume Tally Count
        /// </summary>
        public long AtotBV_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ATotalBoundingVolume").Name]).TallyCount; } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z
        /// </summary>
        public double[,] A_rz { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "AOfRhoAndZ").Name]).Mean; } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z 2nd moment
        /// </summary>
        public double[,] A_rz2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "AOfRhoAndZ").Name]).SecondMoment; } }
        /// <summary>
        /// Absorbed Energy as a function of rho and z Tally Count
        /// </summary>
        public long A_rz_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "AOfRhoAndZ").Name]).TallyCount; } }
        /// <summary>
        /// Absorption as a function of x, y and z
        /// </summary>
        public double[,,] A_xyz { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "AOfXAndYAndZ").Name]).Mean; } }
        /// <summary>
        /// Absorption as a function of x, y and z 2nd moment
        /// </summary>
        public double[,,] A_xyz2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "AOfXAndYAndZ").Name]).SecondMoment; } }
        //public double[, ,] A_rzt { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary["AOfRhoAndZAndTime]).Mean; } }
        //public double[, ,] A_rzt2 { get { return ((ROfRhoAndOmegaDetector)ResultsDictionary["AOfRhoAndZAndTime]).SecondMoment; } }
        /// <summary>
        /// Absorption as a function of x, y and z Tally Count
        /// </summary>
        public long A_xyz_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "AOfXAndYAndZ").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of rho and z
        /// </summary>
        public double[,] Flu_rz { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZ").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of rho and z 2nd moment
        /// </summary>
        public double[,] Flu_rz2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZ").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of rho and z Tally Count
        /// </summary>
        public long Flu_rz_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZ").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of rho, z and time
        /// </summary>
        public double[,,] Flu_rzt { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZAndTime").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of rho, z and time 2nd moment
        /// </summary>
        public double[,,] Flu_rzt2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of rho, z and time Tally Count
        /// </summary>
        public long Flu_rzt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZAndTime").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of x, y and z
        /// </summary>
        public double[,,] Flu_xyz { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZ").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of x, y and z 2nd moment
        /// </summary>
        public double[,,] Flu_xyz2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZ").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of x, y and z Tally Count
        /// </summary>
        public long Flu_xyz_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZ").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of x, y and z
        /// </summary>
        public double[,,,] Flu_xyzt { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of x, y and z 2nd moment
        /// </summary>
        public double[,,,] Flu_xyzt2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of x, y and z Tally Count
        /// </summary>
        public long Flu_xyzt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega (temporal frequency)
        /// </summary>
        public Complex[,,,] Flu_xyzw { get { return (Complex[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega 2nd moment
        /// </summary>
        public Complex[,,,] Flu_xyzw2 { get { return (Complex[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of x, y, z and omega Tally Count
        /// </summary>
        public long Flu_xyzw_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndOmega").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of x, y, z and starting location x,y
        /// </summary>
        public double[,,,,] Flu_xyzxy { get { return (double[,,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of x, y, z and and starting location x,y 2nd moment
        /// </summary>
        public double[,,,,] Flu_xyzxy2 { get { return (double[,,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of x, y, z and starting location x,y Tally Count
        /// </summary>
        public long Flu_xyzxy_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of x, y, z and starting location x,y Count of starting photons
        /// </summary>
        public double[,] Flu_xyzxy_xycount { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY").Name]).StartingXYCount; } }
        /// <summary>
        /// Fluence as a function of rho, z and omega (temporal frequency)
        /// </summary>
        public Complex[,,] Flu_rzw { get { return (Complex[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZAndOmega").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of rho, z and omega 2nd moment
        /// </summary>
        public Complex[,,] Flu_rzw2 { get { return (Complex[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZAndOmega").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of fx and z Tally Count
        /// </summary>
        public long Flu_rzw_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfRhoAndZAndOmega").Name]).TallyCount; } }
        /// <summary>
        /// Fluence as a function of fx (spatial-frequency) and z
        /// </summary>
        public Complex[,] Flu_fxz { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfFxAndZ").Name]).Mean; } }
        /// <summary>
        /// Fluence as a function of fx and z2nd moment
        /// </summary>
        public Complex[,] Flu_fxz2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfFxAndZ").Name]).SecondMoment; } }
        /// <summary>
        /// Fluence as a function of fx and omega Tally Count
        /// </summary>
        public long Flu_fxz_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "FluenceOfFxAndZ").Name]).TallyCount; } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z
        /// </summary>
        public double[] Rad_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfRhoAtZ").Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z 2nd moment
        /// </summary>
        public double[] Rad_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfRhoAtZ").Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of rho (surface tally) at depth Z Tally Count
        /// </summary>
        public long Rad_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfRhoAtZ").Name]).TallyCount; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally)
        /// </summary>
        public double[,,] Rad_rza { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfRhoAndZAndAngle").Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[,,] Rad_rza2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfRhoAndZAndAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) Tally Count
        /// </summary>
        public long Rad_rza_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfRhoAndZAndAngle").Name]).TallyCount; } }
        /// <summary>
        /// Radiance as a function of fx, z and angle (volume tally)
        /// </summary>
        public double[,,] Rad_fxza { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfFxAndZAndAngle").Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) 2nd moment
        /// </summary>
        public double[,,] Rad_fxza2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfFxAndZAndAngle").Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of rho, z and angle (volume tally) Tally Count
        /// </summary>
        public long Rad_fxza_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfFxAndZAndAngle").Name]).TallyCount; } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally)
        /// </summary>
        public double[,,,,] Rad_xyztp { get { return (double[,,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").Name]).Mean; } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally) 2nd moment
        /// </summary>
        public double[,,,,] Rad_xyztp2 { get { return (double[,,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").Name]).SecondMoment; } }
        /// <summary>
        /// Radiance as a function of x, y, z, theta and phi (volume tally) Tally Count
        /// </summary>
        public long Rad_xyztp_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").Name]).TallyCount; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] RefMT_rmt { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] RefMT_rmt2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefMT_rmt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Reflected Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,] RefMT_rmt_frac { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,,] RefMT_xymt { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,,] RefMT_xymt2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefMT_xymt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Reflected Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,,] RefMT_xymt_frac { get { return (double[,,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] TransMT_rmt { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] TransMT_rmt2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransMT_rmt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,] TransMT_rmt_frac { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Transmitted Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,,] TransMT_xymt { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,,] TransMT_xymt2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransMT_xymt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Transmitted Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,,] TransMT_xymt_frac { get { return (double[,,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] RefDynMT_rmt { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] RefDynMT_rmt2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefDynMT_rmt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,] RefDynMT_rmt_frac { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,] RefDynMT_rmt_totofz { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).TotalMTOfZ; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,] RefDynMT_rmt_dynofz { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).DynamicMTOfZ; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Rho and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] RefDynMT_rmt_subrcols { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").Name]).SubregionCollisions; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,,] RefDynMT_xymt { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,,] RefDynMT_xymt2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefDynMT_xymt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,] RefDynMT_xymt_frac { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,,] RefDynMT_xymt_totofz { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).TotalMTOfZ; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,,] RefDynMT_xymt_dynofz { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).DynamicMTOfZ; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of X, Y and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] RefDynMT_xymt_subrcols { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").Name]).SubregionCollisions; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT
        /// </summary>
        public Complex[,] RefDynMT_fxmt { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public Complex[,] RefDynMT_fxmt2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long RefDynMT_fxmt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public Complex[,,] RefDynMT_fxmt_frac { get { return (Complex[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with Total MT of Z
        /// </summary>
        public Complex[,] RefDynMT_fxmt_totofz { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).TotalMTOfZ; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public Complex[,] RefDynMT_fxmt_dynofz { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).DynamicMTOfZ; } }
        /// <summary>
        /// Reflected Dynamic Momentum Transfer of Fx and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] RefDynMT_fxmt_subrcols { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").Name]).SubregionCollisions; } }

        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,] TransDynMT_rmt { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,] TransDynMT_rmt2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransDynMT_rmt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,] TransDynMT_rmt_frac { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,] TransDynMT_rmt_totofz { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).TotalMTOfZ; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,] TransDynMT_rmt_dynofz { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).DynamicMTOfZ; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Rho and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] TransDynMT_rmt_subrcols { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).SubregionCollisions; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT
        /// </summary>
        public double[,,] TransDynMT_xymt { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public double[,,] TransDynMT_xymt2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransDynMT_xymt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public double[,,,] TransDynMT_xymt_frac { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Total MT of Z
        /// </summary>
        public double[,,] TransDynMT_xymt_totofz { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).TotalMTOfZ; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public double[,,] TransDynMT_xymt_dynofz { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).DynamicMTOfZ; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of X, Y and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] TransDynMT_xymt_subrcols { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").Name]).SubregionCollisions; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT
        /// </summary>
        public Complex[,] TransDynMT_fxmt { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT 2nd moment
        /// </summary>
        public Complex[,] TransDynMT_fxmt2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT Tally Count
        /// </summary>
        public long TransDynMT_fxmt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with a histogram of MT fractional MT
        /// </summary>
        public Complex[,,] TransDynMT_fxmt_frac { get { return (Complex[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).FractionalMT; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with Total MT of Z
        /// </summary>
        public Complex[,] TransDynMT_fxmt_totofz { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).TotalMTOfZ; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with Dynamic MT of Z
        /// </summary>
        public Complex[,] TransDynMT_fxmt_dynofz { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).DynamicMTOfZ; } }
        /// <summary>
        /// Transmitted Dynamic Momentum Transfer of Fx and Tissue SubRegion with SubregionCollisions
        /// </summary>
        public double[,] TransDynMT_fxmt_subrcols { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").Name]).SubregionCollisions; } }

        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time
        /// </summary>
        public double[,,] RefTime_rs_hist { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").Name]).Mean; } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time 2nd moment
        /// </summary>
        public double[,,] RefTime_rs_hist2 { get { return (double[,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").Name]).SecondMoment; } }
        /// <summary>
        /// Reflected Time of Rho and Tissue SubRegion with a histogram of Time Tally Count
        /// </summary>
        public long RefTime_rs_hist_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "ReflectedTimeOfRhoAndSubregionHist").Name]).TallyCount; } }
        /// <summary>
        /// pMC Total Absorbed Energy
        /// </summary>
        public double pMC_Atot { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCATotal").Name]).Mean; } }
        /// <summary>
        /// Total Absorbed Energy 2nd moment
        /// </summary>
        public double pMC_Atot2 { get { return (double)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCATotal").Name]).SecondMoment; } }
        /// <summary>
        /// Total Absorbed Energy Tally Count
        /// </summary>
        public long pMC_Atot_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCATotal").Name]).TallyCount; } }

        /// <summary>
        /// perturbation MC Reflectance as a function of rho 
        /// </summary>
        public double[] pMC_R_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRho").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho 2nd moment
        /// </summary>
        public double[] pMC_R_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRho").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho Tally Count
        /// </summary>
        public long pMC_R_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRho").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho recessed in air
        /// </summary>
        public double[] pMC_R_rr { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoRecessed").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho recessed in air 2nd moment 
        /// </summary>
        public double[] pMC_R_rr2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho recessed in air Tally Count
        /// </summary>
        public long pMC_R_rr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoRecessed").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time
        /// </summary>
        public double[,] pMC_R_rt { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoAndTime").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time 2nd moment
        /// </summary>
        public double[,] pMC_R_rt2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time Tally Count
        /// </summary>
        public long pMC_R_rt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoAndTime").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time recessed in air
        /// </summary>
        public double[,] pMC_R_rtr { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoAndTimeRecessed").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time recessed in air 2nd moment
        /// </summary>
        public double[,] pMC_R_rtr2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoAndTimeRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of rho and time recessed in air Tally Count
        /// </summary>
        public long pMC_R_rtr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfRhoAndTimeRecessed").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y
        /// </summary>
        public double[,] pMC_R_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndY").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y 2nd moment
        /// </summary>
        public double[,] pMC_R_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndY").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y Tally Count
        /// </summary>
        public long pMC_R_xy_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndY").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x, y, time and tissue subregion
        /// </summary>
        public double[,,,] pMC_R_xyts { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregion").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y and time and subregion ROfXAndY
        /// </summary>
        public double[,] pMC_R_xyts_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregion").Name]).ROfXAndY; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y and time and subregion ROfXAndY 2nd moment
        /// </summary>
        public double[,] pMC_R_xyts_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregion").Name]).ROfXAndYSecondMoment; } }

        /// <summary>
        /// perturbation MC Reflectance as a function of x, y, time and tissue subregion 2nd moment
        /// </summary>
        public double[,,,] pMC_R_xyts2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregion").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x, y, time and tissue subregion Tally Count
        /// </summary>
        public long pMC_R_xyts_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregion").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x, y, time and tissue subregion recessed in air
        /// </summary>
        public double[,,,] pMC_R_xytsr { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregionRecessed").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y and time and subregion recessed ROfXAndY
        /// </summary>
        public double[,] pMC_R_xytsr_xy { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregionRecessed").Name]).ROfXAndY; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x and y and time and subregion recessed ROfXAndY 2nd moment
        /// </summary>
        public double[,] pMC_R_xytsr_xy2 { get { return (double[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregionRecessed").Name]).ROfXAndYSecondMoment; } }

        /// <summary>
        /// perturbation MC Reflectance as a function of x, y, time and tissue subregion recessed in air 2nd moment
        /// </summary>
        public double[,,,] pMC_R_xytsr2 { get { return (double[,,,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregionRecessed").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of x, y, time and tissue subregion recessed in air Tally Count
        /// </summary>
        public long pMC_R_xytsr_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfXAndYAndTimeAndSubregionRecessed").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Transmittance as a function of rho 
        /// </summary>
        public double[] pMC_T_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCTOfRho").First().Name]).Mean; } }
        /// <summary>
        /// perturbation MC Transmittance as a function of rho 2nd moment
        /// </summary>
        public double[] pMC_T_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCTOfRho").First().Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Transmittance as a function of rho Tally Count
        /// </summary>
        public long pMC_T_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.Where(d => d.TallyType == "pMCTOfRho").First().Name]).TallyCount; } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua
        /// </summary>
        public double[] dMCdMua_R_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "dMCdROfRhodMua").Name]).Mean; } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua 2nd moment
        /// </summary>
        public double[] dMCdMua_R_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "dMCdROfRhodMua").Name]).SecondMoment; } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mua Tally Count
        /// </summary>
        public long dMCdMua_R_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "dMCdROfRhodMua").Name]).TallyCount; } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus
        /// </summary>
        public double[] dMCdMus_R_r { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "dMCdROfRhodMus").Name]).Mean; } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus 2nd moment
        /// </summary>
        public double[] dMCdMus_R_r2 { get { return (double[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "dMCdROfRhodMus").Name]).SecondMoment; } }
        /// <summary>
        /// differential MC Reflectance as a function of rho wrt to mus Tally Count
        /// </summary>
        public long dMCdMus_R_r_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "dMCdROfRhodMus").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx
        /// </summary>
        public Complex[] pMC_R_fx { get { return (Complex[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfFx").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx 2nd moment
        /// </summary>
        public Complex[] pMC_R_fx2 { get { return (Complex[])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfFx").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx Tally Count
        /// </summary>
        public long pMC_R_fx_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfFx").Name]).TallyCount; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx and time
        /// </summary>
        public Complex[,] pMC_R_fxt { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfFxAndTime").Name]).Mean; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx and time 2nd moment
        /// </summary>
        public Complex[,] pMC_R_fxt2 { get { return (Complex[,])((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfFxAndTime").Name]).SecondMoment; } }
        /// <summary>
        /// perturbation MC Reflectance as a function of fx and time Tally count
        /// </summary>
        public long pMC_R_fxt_TallyCount { get { return (long)((dynamic)ResultsDictionary[_detectorResults.First(d => d.TallyType == "pMCROfFxAndTime").Name]).TallyCount; } }

        /// <summary>
        /// Simulation Input that generated this SimulationOutput
        /// </summary>
        public SimulationInput Input { get; private set; }
        /// <summary>
        /// Dictionary holding detector results as specified in SimulationInput
        /// </summary>
        public IDictionary<String, IDetector> ResultsDictionary { get; private set; }

        /// <summary>
        /// method that calls GetDetector to get detectors from a list of detector names
        /// </summary>
        /// <param name="detectorNames">list of detector names strings</param>
        /// <returns>list of IDetector</returns>
        public IEnumerable<IDetector> GetDetectors(IEnumerable<string> detectorNames)
        {
            foreach (var detectorName in detectorNames)
            {
                var detector = GetDetector(detectorName);

                if (detector == null) continue;
                yield return detector;
            }
        }
        /// <summary>
        /// method to get detector from detector name
        /// </summary>
        /// <param name="detectorName">detector name string</param>
        /// <returns>IDetector</returns>
        public IDetector GetDetector(string detectorName)
        {
            IDetector detector;

            ResultsDictionary.TryGetValue(detectorName, out detector);

            return detector;
        }

    }
}
