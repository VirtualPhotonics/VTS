using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This implements Kienle's two-layer SDA solutions described in:
    /// 1) Kienle et al., "Noninvasive determination of the optical properties of two-layered
    /// turbid media", Applied Optics 37(4), 1998.
    /// 2) Kienle et al., "In vivo determination of the optical properties of muscle with time-
    /// resolved reflectance using a layered model, Phys. Med. Biol. 44, 1999 (in particular, the
    /// appendix)
    /// Notes:
    /// 1) this solution assumes that the embedded source is within top layer.
    /// 2) zp = location of embedded isotropic source is determined using layer 1 opt. props.
    /// 3) zb = extrapolated boundary is determined using layer 1 opt. props.
    /// This currently inherits ForwardSolverBase.  In order to provide fluence of ft type solutions,
    /// this would need to inherit DiffusionForwardSolverBase however the methods there assume homogeneous
    /// tissue so not sure best way to implement ft type solutions.
    /// </summary>
    public class TwoLayerSDAForwardSolver : ForwardSolverBase
    {
        /// <summary>
        /// Returns an instance of TwoLayerSDAForwardSolver
        /// </summary>
        public TwoLayerSDAForwardSolver() :
            base(SourceConfiguration.Point, 0.0)
        {
        }

        // this assumes: first region in ITissueRegion[] is top layer of tissue because need to know what OPs 
        // to use for FresnelReflection and so I can define layer thicknesses
        public override double ROfRho(IOpticalPropertyRegion[] regions, double rho)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);

            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }

            return StationaryReflectance(rho, diffusionParameters, layerThicknesses, fr1, fr2);
        }
        public override double ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double time)
        {
            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }
            return ROfRhoAndTime(regions.AsEnumerable(), rho.AsEnumerable(), time.AsEnumerable()).First();
        }

        /// <summary>
        /// Evaluates spatially- and temporally- resolved reflectance at sets of tissue regions, rhos and times
        /// </summary>
        /// <param name="regions">multiple sets of tissue regions</param>
        /// <param name="rhos">rhos</param>
        /// <param name="times">times</param>
        /// <returns>reflectance at specified optical properties, rhos and times</returns>
        public override IEnumerable<double> ROfRhoAndTime(IEnumerable<IOpticalPropertyRegion[]> setsOfRegions,
            IEnumerable<double> rhos, IEnumerable<double> times)
        {
            double[] rOfTime = new double[times.Count()];
            double[] FFTtimeSequence;
            foreach (var regions in setsOfRegions)
            {
                foreach (var rho in rhos)
                {
                    // for fixed time and looping over rhos, this still is slow
                    rOfTime = DetermineROfTimeFromROfFtForFixedRho(rho, regions, out FFTtimeSequence);
                    foreach (var time in times)
                    {
                        yield return Vts.Common.Math.Interpolation.interp1(FFTtimeSequence,
                            rOfTime.ToList(), time);
                    }
                }
            }
        }

        // this method builds an R(rho, ft) array and then uses FFT to generate R(rho, t)
        private double[] DetermineROfTimeFromROfFtForFixedRho(double rho, IOpticalPropertyRegion[] regions, out double[] FFTTimeSequence)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);
            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            int numFreq = 512; // Kienle used 512 and deltaFreq = 0.1
            // Kienle says deltaFrequency depends on source-detector separation
            var deltaFrequency = 0.1; // 100 MHz
            if (rho <= 3)
            {
                deltaFrequency = 0.5; // so far I've found this value works for smaller rho
            }     
            var F = numFreq*deltaFrequency; // 51 GHz
            var deltaTime = 1.0/(numFreq*deltaFrequency); // 0.02 ns => T = 10 ns
            
            // var homoSDA = new PointSourceSDAForwardSolver(); // debug with homo SDA
            // var rOfTime = new Complex[numFreq]; // debug array

            // considerations: 2n datapoint and pad with 0s beyond (deltaTime * numFreq)
            var rOfFt = new Complex[numFreq];
            double[] ft = Enumerable.Range(0, numFreq).Select(x => x * deltaFrequency).ToArray();
            FFTTimeSequence = Enumerable.Range(0, numFreq).Select(x => x*deltaTime).ToArray();

            for (int i = 0; i < numFreq; i++)
            {
                // normalize by F=(numFreq*deltaFrequency)
                rOfFt[i] = TemporalFrequencyReflectance(rho, ft[i], diffusionParameters, layerThicknesses, fr1, fr2) * F;
                // rOfTime[i] = homoSDA.ROfRhoAndTime(regions[1].RegionOP, rho, t[i]); // debug array
            }
            // to debug, use R(t) and FFT to see if result R(ft) is close to rOfFt
            //var dft2 = new MathNet.Numerics.IntegralTransforms.Algorithms.DiscreteFourierTransform();
            //dft2.Radix2Forward(rOfTime, FourierOptions.NoScaling);  // convert to R(ft) to compare with rOfFt
            //var relDiffReal = Enumerable.Zip(rOfTime, rOfFt, (x, y) => Math.Abs((y.Real - x.Real) / x.Real));
            //var relDiffImag = Enumerable.Zip(rOfTime, rOfFt, (x, y) => Math.Abs((y.Imaginary - x.Imaginary) / x.Imaginary));
            //var maxReal = relDiffReal.Max();
            //var maxImag = relDiffImag.Max();
            //var dum1 = maxReal;
            //var dum2 = maxImag;
            //dft2.Radix2Inverse(rOfTime, FourierOptions.NoScaling); // debug convert to R(t)
            // end debug code

            // FFT R(ft) to R(t)
            //var dft = new MathNet.Numerics.IntegralTransforms.Algorithms.DiscreteFourierTransform();            
            //dft.Radix2Inverse(rOfFt, FourierOptions.NoScaling); // convert to R(t)
            Fourier.Radix2Inverse(rOfFt, FourierOptions.NoScaling); 
            var rOfTime = new double[FFTTimeSequence.Length];
            rOfTime = rOfFt.Select(r => r.Real / (numFreq / 2)).ToArray();
            return rOfTime;
        }

        // this method builds an R(fx, ft) array and then uses FFT to generate R(fx, t)
        private double[] DetermineROfTimeFromROfFtForFixedFx(double fx, IOpticalPropertyRegion[] regions, out double[] FFTTimeSequence)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);
            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            int numFreq = 512; // Kienle used 512 and deltaFreq = 0.1
            // Kienle says deltaFrequency depends on source-detector separation
            var deltaFrequency = 0.5; // 500 MHz good for all fx

            var F = numFreq * deltaFrequency; // 51 GHz
            var deltaTime = 1.0 / (numFreq * deltaFrequency); // 0.02 ns => T = 10 ns

            // considerations: 2n datapoint and pad with 0s beyond (deltaTime * numFreq)
            var rOfFt = new Complex[numFreq];
            double[] ft = Enumerable.Range(0, numFreq).Select(x => x * deltaFrequency).ToArray();
            FFTTimeSequence = Enumerable.Range(0, numFreq).Select(x => x * deltaTime).ToArray();

            for (int i = 0; i < numFreq; i++)
            {
                // normalize by F=(numFreq*deltaFrequency)
                rOfFt[i] = SpatialAndTemporalFrequencyReflectance(2 * Math.PI * fx, ft[i], diffusionParameters, layerThicknesses, fr1, fr2) * F;
            }

            // FFT R(ft) to R(t)
            //var dft = new MathNet.Numerics.IntegralTransforms.Algorithms.DiscreteFourierTransform();
            //dft.Radix2Inverse(rOfFt, FourierOptions.NoScaling); // convert to R(t)
            Fourier.Radix2Inverse(rOfFt, FourierOptions.NoScaling);
            var rOfTime = new double[FFTTimeSequence.Length];
            rOfTime = rOfFt.Select(r => r.Real / (numFreq / 2)).ToArray();
            return rOfTime;
        }
        public override Complex ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double ft)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);

            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }

            return TemporalFrequencyReflectance(rho, ft, diffusionParameters, layerThicknesses, fr1, fr2);
        }

        public override double ROfFx(IOpticalPropertyRegion[] regions, double fx)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);

            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }
            return SpatialFrequencyReflectance(2*Math.PI*fx, diffusionParameters, layerThicknesses, fr1, fr2);
        }

        public override double ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double time)
        {
            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }
            return ROfFxAndTime(regions.AsEnumerable(), fx.AsEnumerable(), time.AsEnumerable()).First();
        }
        /// <summary>
        /// Evaluates spatial-frequency and temporally- resolved reflectance at sets of tissue regions, fs and times
        /// </summary>
        /// <param name="regions">multiple sets of tissue regions</param>
        /// <param name="fx">spatial frequencies</param>
        /// <param name="times">times</param>
        /// <returns>reflectance at specified optical properties, rhos and times</returns>
        public override IEnumerable<double> ROfFxAndTime(IEnumerable<IOpticalPropertyRegion[]> setsOfRegions,
            IEnumerable<double> fxs, IEnumerable<double> times)
        {
            double[] rOfTime = new double[times.Count()];
            double[] FFTTimeSequence;
            foreach (var regions in setsOfRegions)
            {
                foreach (var fx in fxs)
                {
                    rOfTime = DetermineROfTimeFromROfFtForFixedFx(fx, regions, out FFTTimeSequence);
                    foreach (var time in times)
                    {
                        yield return Vts.Common.Math.Interpolation.interp1(FFTTimeSequence, rOfTime.ToList(), time);
                    }
                }
            }
        }
        public override Complex ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double ft)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);

            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }
            return SpatialAndTemporalFrequencyReflectance(2 * Math.PI * fx, ft, diffusionParameters, 
                layerThicknesses, fr1, fr2);
        }

        public override IEnumerable<double> FluenceOfRhoAndZ(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {

            foreach (var region in regions)
            {
                var dp = GetDiffusionParameters(region);
                var layerThicknesses = GetLayerThicknesses(region);
                foreach (var rho in rhos)
                {
                    foreach (var z in zs)
                    {
                        yield return StationaryFluence(rho, z, dp, layerThicknesses);
                    }
                }
            }
        }

        private static DiffusionParameters[] GetDiffusionParameters(IOpticalPropertyRegion[] regions)
        {
            var diffusionParameters = new DiffusionParameters[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                diffusionParameters[i] = DiffusionParameters.Create(
                    new OpticalProperties(regions[i].RegionOP.Mua, regions[i].RegionOP.Musp, regions[i].RegionOP.G, regions[i].RegionOP.N),
                    ForwardModel.SDA);
            }
            return diffusionParameters;
        }

        private static double[] GetLayerThicknesses(IOpticalPropertyRegion[] regions)
        {
            var layerThicknesses = new double[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                layerThicknesses[i] = ((ILayerOpticalPropertyRegion)regions[i]).ZRange.Stop;
            }
            return layerThicknesses;
        }


        /// <summary>
        /// Evaluate the stationary radially resolved reflectance with the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object for each tissue region</param>
        /// <param name="rho">radial location</param>
        /// <param name="fr1">Fresnel moment 1, R1</param>
        /// <param name="fr2">Fresnel moment 2, R2</param>
        /// <returns>reflectance</returns>
        public static double StationaryReflectance(double rho, DiffusionParameters[] dp, double[] layerThicknesses,
                                            double fr1, double fr2)
        {
            // this could use GetBackwardHemisphereIntegralDiffuseReflectance possibly? no protected
            return (1 - fr1) / 4 * StationaryFluence(rho, 0.0, dp, layerThicknesses) -
                (fr2 - 1) / 2 * dp[0].D * StationaryFlux(rho, 0.0, dp, layerThicknesses);
        }
        public static double SpatialFrequencyReflectance(double s, DiffusionParameters[] dp, double[] layerThicknesses,
                                    double fr1, double fr2)
        {
            return (1 - fr1) / 4 * Phi1(s, 0.0, dp, layerThicknesses) -
                (fr2 - 1) / 2 * dp[0].D * dPhi1(s, 0.0, dp, layerThicknesses);
        }
        public static Complex TemporalFrequencyReflectance(double rho, double temporalFrequency, 
            DiffusionParameters[] dp, double[] layerThicknesses, double fr1, double fr2)
        {
            return (1 - fr1) / 4 * TemporalFrequencyFluence(rho, 0.0, temporalFrequency, dp, layerThicknesses) -
                (fr2 - 1) / 2 * dp[0].D * TemporalFrequencyZFlux(rho, 0.0, temporalFrequency, dp, layerThicknesses);
        }
        public static Complex SpatialAndTemporalFrequencyReflectance(double s, double temporalFrequency,
            DiffusionParameters[] dp, double[] layerThicknesses, double fr1, double fr2)
        {
            return (1 - fr1) / 4 * TemporalFrequencyPhi1(s, 0.0, temporalFrequency, dp, layerThicknesses) -
                (fr2 - 1) / 2 * dp[0].D * TemporalFrequencydPhi1(s, 0.0, temporalFrequency, dp, layerThicknesses);
        }
        /// <summary>
        /// Evaluate the stationary radially resolved fluence with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParameters for layer 1 and 2</param>
        /// <param name="layerThicknesses">in this class, layer thickness</param>
        /// <returns>fluence</returns>
        private static double StationaryFluence(double rho, double z, DiffusionParameters[] dp, 
            double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            double fluence;
            if (z < layerThickness) // top layer phi1 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(rho, s => Phi1(s, z, dp, layerThicknesses));
            }
            else // bottom layer phi2 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(rho, s => Phi2(s, z, dp, layerThicknesses));
            }           
            return fluence/(2*Math.PI); 
        }

        /// <summary>
        /// Evaluate the stationary radially resolved z-flux with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParameters for layer 1 and 2</param>
        /// <param name="layerThicknesses">thickness of top layer, array but only need first element</param>
        /// <returns></returns>
        private static double StationaryFlux(double rho, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            double flux;
            if (z < layerThickness) // top layer dphi1/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(rho, s => dPhi1(s, z, dp, layerThicknesses));
            }
            else // bottom layer phi2/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(rho, s => dPhi2(s, z, dp, layerThicknesses));
            }
            return flux/(2*Math.PI); 
        }

        public static Complex TemporalFrequencyFluence(double rho,
            double z, double temporalFrequency, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            Complex fluence;
            if (z < layerThickness) // top layer phi1 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => TemporalFrequencyPhi1(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                          HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => TemporalFrequencyPhi1(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                    Complex.ImaginaryOne;
            }
            else // bottome layer phi2 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => TemporalFrequencyPhi2(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                          HankelTransform.DigitalFilterOfOrderZero(
                    rho,s => TemporalFrequencyPhi2(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                    Complex.ImaginaryOne;
            }
            return fluence / (2 * Math.PI);
        }

        public static Complex TemporalFrequencyZFlux(double rho, double z, double temporalFrequency,
            DiffusionParameters[] dp, double[] layerThicknesses )
        {
            var layerThickness = layerThicknesses[0];
            Complex flux;
            if (z < layerThickness) // top layer dphi1/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi1(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                       HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi1(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                            Complex.ImaginaryOne;
            }
            else // bottom layer phi2/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi2(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                       HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi2(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                            Complex.ImaginaryOne;
            }
            return flux / (2 * Math.PI);
        }
        // Note that the "guts" of Phi1 and TemporalFrequencyPhi1 and all other pairs, are equivalent,
        // the only difference is changing alpha1 and alpha2 to be complex
        private static double Phi1(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            double arg;
            if (z < dp[0].zp) // in top layer and above isotropic source
            {
                arg = dp[0].zp - z;
            }
            else // in top layer and below isotropic source
            {
                arg = z - dp[0].zp;
            }
            var dum1 = Math.Exp(-alpha1 * arg) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) -
                       Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) +
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return (dum1 + Da * dum2) / (2 * dp[0].D * alpha1);
        }
        private static Complex TemporalFrequencyPhi1(double s, double z, double ft, 
            DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            double arg;
            if (z < dp[0].zp) // in top layer and above isotropic source
            {
                arg = dp[0].zp - z;
            }
            else // in top layer and below isotropic source
            {
                arg = z - dp[0].zp;
            }
            var dum1 = Complex.Exp(-alpha1 * arg) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) -
                       Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) +
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return (dum1 + Da * dum2) / (2 * dp[0].D * alpha1);
        }
        private static double Phi2(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Math.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Math.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Math.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return dum3 * (dum4 - Da * dum5);
        }
        private static Complex TemporalFrequencyPhi2(double s, double z, double ft, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Complex.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Complex.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Complex.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return dum3 * (dum4 - Da * dum5);
        }
        private static double dPhi1(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            double arg;
            if (z < dp[0].zp) // in top layer and above isotropic source
            {
                arg = dp[0].zp - z;
            }
            else // in top layer and below isotropic source
            {
                arg = z - dp[0].zp;
            }
            var dum1 = Math.Exp(-alpha1 * arg) +
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) +
                       Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) -
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return ((dum1 + Da * dum2) / (2 * dp[0].D));
        }
        private static Complex TemporalFrequencydPhi1(double s, double z, double ft, 
            DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            double arg;
            if (z < dp[0].zp) // in top layer and above isotropic source
            {
                arg = dp[0].zp - z;
            }
            else // in top layer and below isotropic source
            {
                arg = z - dp[0].zp;
            }
            var dum1 = Complex.Exp(-alpha1 * arg) +
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) +
                       Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) -
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return ((dum1 + Da * dum2) / (2 * dp[0].D));
        }
        private static double dPhi2(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Math.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Math.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Math.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return -alpha2 * (dum3 * (dum4 - Da * dum5));
        }
        private static Complex TemporalFrequencydPhi2(double s, double z, double ft, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * ft * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Complex.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Complex.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Complex.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return -alpha2 * (dum3 * (dum4 - Da * dum5));
        }
    }
}
