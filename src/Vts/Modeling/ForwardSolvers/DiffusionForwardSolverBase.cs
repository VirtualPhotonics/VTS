using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// SourceConfiguration enum
    /// </summary>
    public enum SourceConfiguration
    {
        /// <summary>
        /// point source
        /// </summary>
        Point,
        /// <summary>
        /// source distributed along line within tissue
        /// </summary>
        Distributed,
        /// <summary>
        /// Gaussian source
        /// </summary>
        Gaussian,
    }
    /// <summary>
    /// ForwardModel enum
    /// </summary>
    public enum ForwardModel
    {
        /// <summary>
        /// standard diffusion approximation
        /// </summary>
        SDA,
        /// <summary>
        /// delta-P1 approximation
        /// </summary>
        DeltaPOne,
    }

    /// <summary>
    /// diffusion based forward solvers class
    /// </summary>
    public abstract class DiffusionForwardSolverBase : ForwardSolverBase
    {
        /// <summary>
        /// diffusion forward solver base
        /// </summary>
        /// <param name="sourceConfiguration">source configuration</param>
        /// <param name="beamDiameter">diameter of source [mm]</param>
        protected DiffusionForwardSolverBase(SourceConfiguration sourceConfiguration, double beamDiameter)
            : base(sourceConfiguration, beamDiameter) { }

        /// <summary>
        /// forward model enum
        /// </summary>
        protected ForwardModel ForwardModel { get; set; }

        #region IForwardSolver Members
        /// <summary>
        /// reflectance as a function of s-d separation
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">s-d separation</param>
        /// <returns>R(rho)</returns>
        public override double ROfRho(OpticalProperties op, double rho)
        {
            return ROfRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }
        /// <summary>
        /// reflectance as a function of rhos 
        /// </summary>
        /// <param name="ops">IEnumerable of optical properties</param>
        /// <param name="rhos">IEnumerable of rho values</param>
        /// <returns>R(rhos)</returns>
        public override IEnumerable<double> ROfRho(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op.N);
                var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op.N);
                foreach (var rho in rhos)
                {
                    yield return StationaryReflectance(dp, rho, fr1, fr2);
                }
            }
        }
        /// <summary>
        /// reflectance as a function of s-d separation and time
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="time">time [ns]</param>
        /// <returns>R(rho,time)</returns>
        public override double ROfRhoAndTime(
            OpticalProperties op, double rho, double time)
        {
            return ROfRhoAndTime(op.AsEnumerable(), rho.AsEnumerable(), time.AsEnumerable()).First();
        }
        /// <summary>
        /// reflectance as a function of rhos and times
        /// </summary>
        /// <param name="ops">IEnumerable of optical properties</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="times">times</param>
        /// <returns>R(rhos,times)</returns>
        public override IEnumerable<double> ROfRhoAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op.N);
                var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op.N);
                foreach (var rho in rhos)
                {
                    foreach (var t in times)
                    {
                        yield return TemporalReflectance(dp, rho, t, fr1, fr2);
                    }
                }
            }
        }
        /// <summary>
        /// reflectance as a function of s-d separation and temporal-frequency
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="ft">temporal-frequency</param>
        /// <returns>R(rho,ft)</returns>
        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return ROfRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        }
        /// <summary>
        /// reflectance as a function of s-d separations and temporal-frequencies
        /// </summary>
        /// <param name="ops">IEnumerable of optical properties</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="fts">temporal-frequencies</param>
        /// <returns>R(rhos,fts)</returns>
        public override IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op.N);
                var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op.N);
                foreach (var rho in rhos)
                {
                    foreach (var ft in fts)
                    {
                        Complex k = ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
                            (dp.cn * dp.D)).SquareRoot();
                        yield return TemporalFrequencyReflectance(dp, rho, k, fr1, fr2);
                    }
                }
            }
        }

        #region Spatial Frequency

        /// <summary>
        /// ROfFx, solves SDA using Cuccia et al JBO, March/April 2009 
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns></returns>
        public override double ROfFx(OpticalProperties op, double fx)
        {
            return ROfFx(op.AsEnumerable(), fx.AsEnumerable()).First();
        }

        /// <summary>
        /// Vectorized ROfFx. Solves SDA using Cuccia et al JBO, March/April 2009 
        /// </summary>
        /// <param name="ops">set of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        public override IEnumerable<double> ROfFx(
            IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var fx in fxs)
                {
                    yield return SFDDiffusionForwardSolver.StationaryOneDimensionalSpatialFrequencyFluence(
                        dp, fx, 0.0) / 2 / dp.A; // this is not the true mathematical derivation given the source condition! but is the WIDELY used in the lit
                }
            }
        }
        /// <summary>
        /// reflectance as a function of spatial-frequency and time
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial-frequency</param>
        /// <param name="time">time [ns]</param>
        /// <returns>R(fx,time)</returns>
        public override double ROfFxAndTime(OpticalProperties op, double fx, double time)
        {
            return ROfFxAndTime(op.AsEnumerable(), fx.AsEnumerable(), time.AsEnumerable()).First();
        }
        /// <summary>
        /// reflectance as a function of spatial-frequencies and times
        /// </summary>
        /// <param name="ops">IEnumerable of optical properties</param>
        /// <param name="fxs">spatial-frequencies</param>
        /// <param name="times">times</param>
        /// <returns>R(fxs,times)</returns>
        public override IEnumerable<double> ROfFxAndTime(
            IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    foreach (var t in times)
                    {
                        yield return
                            2 * Math.PI *
                            HankelTransform.DigitalFilterOfOrderZero(
                                2 * Math.PI * fx, rho => ROfRhoAndTime(op, rho, t));
                    }
                }
            }
        }

        /// <summary>
        /// Modulation frequency-dependent reflectance. Modified from Pham et al, Appl. Opt. Sept 2000 
        /// to include spatial modulation, as described in Cuccia et al, J. Biomed. Opt. March/April 2009
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            double A = CalculatorToolbox.GetCubicAParameter(op.N);
            double wOverC = Math.PI * ft * op.N / GlobalConstants.C;
            double mutr = op.Mua + op.Musp;
            Complex D = 1 / (3 * mutr * (1.0 + Complex.ImaginaryOne * wOverC / mutr));
            Complex mueff =
            (
                3.0 * op.Mua * mutr -
                3.0 * wOverC * wOverC +
                Complex.ImaginaryOne * (1 + op.Mua / mutr) * 3.0 * mutr * wOverC +
                4.0 * Math.PI * Math.PI * fx * fx
            ).SquareRoot();

            Complex temp = mueff * D;

            return (3 * A * op.Musp * D / (temp + 1.0 / 3.0) / (temp + A));
        }

        #endregion Spatial Frequency


        #endregion IForwardSolver Members

        #region Helper Methods
        // protected methods

        /// <summary>
        /// Calculates the reflectance based on the integral of the radiance over the backward hemisphere...
        /// </summary>
        /// <param name="surfaceFluence">diffuse fluence at the surface</param>
        /// <param name="surfaceFlux">diffuse flux at the surface</param>
        /// <param name="mediaRefractiveIndex">refractive index of the medium</param>
        /// <returns></returns>
        protected static double GetBackwardHemisphereIntegralDiffuseReflectance(
            double surfaceFluence, double surfaceFlux, double mediaRefractiveIndex)
        {
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(
                mediaRefractiveIndex);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(
                mediaRefractiveIndex);
            return GetBackwardHemisphereIntegralDiffuseReflectance(surfaceFluence, surfaceFlux,
                fr1, fr2);
        }

        /// <summary>
        /// Calculates the reflectance based on the integral of the radiance over the backward
        /// hemisphere for complex fluence
        /// </summary>
        /// <param name="surfaceFluence">complex diffuse fluence at the surface</param>
        /// <param name="surfaceFlux">complex diffuse flux at the surface</param>
        /// <param name="mediaRefractiveIndex">refractive index of the medium</param>
        /// <returns></returns>
        protected static Complex GetBackwardHemisphereIntegralDiffuseReflectance(
            Complex surfaceFluence, Complex surfaceFlux, double mediaRefractiveIndex)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                surfaceFluence.Real, surfaceFlux.Real, mediaRefractiveIndex) +
                Complex.ImaginaryOne * GetBackwardHemisphereIntegralDiffuseReflectance(
                surfaceFluence.Imaginary, surfaceFlux.Imaginary, mediaRefractiveIndex);
        }

        /// <param name="fluence">Fluence</param>
        /// <param name="flux">Flux</param>
        /// <param name="fr1">1st moment of Fresnel Reflection</param>
        /// <param name="fr2">2nd moment of Fresnel Reflection</param>
        /// <returns></returns>
        protected static double GetBackwardHemisphereIntegralDiffuseReflectance(
            double fluence, double flux, double fr1, double fr2)
        {
            return
                (1 - fr1) / 4 * fluence +
                (fr2 - 1) / 2 * flux;
        }
        /// <summary>
        /// get backward hemisphere integral for diffuse reflectance
        /// </summary>
        /// <param name="fluence">fluence term</param>
        /// <param name="flux">flux term</param>
        /// <param name="fr1"></param>
        /// <param name="fr2"></param>
        /// <returns></returns>
        protected static Complex GetBackwardHemisphereIntegralDiffuseReflectance(
            Complex fluence, Complex flux, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(fluence.Real, flux.Real, fr1, fr2) +
                Complex.ImaginaryOne *
                GetBackwardHemisphereIntegralDiffuseReflectance(fluence.Imaginary, flux.Imaginary, fr1, fr2);
        }


        #endregion Helper Methods

        #region Fluence Solutions
        /// <summary>
        /// fluence as function of s-d separations and depths
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <returns>fluence(rhos,zs)</returns>
        public override IEnumerable<double> FluenceOfRhoAndZ(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var rho in rhos)
                {
                    foreach (var z in zs)
                    {
                        yield return StationaryFluence(rho, z, dp);
                    }
                }
            }
        }
        /// <summary>
        /// fluence as function of s-d separations, depths and times
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <param name="times">times</param>
        /// <returns>fluence(rhos,zs,times)</returns>
        public override IEnumerable<double> FluenceOfRhoAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var rho in rhos)
                {
                    foreach (var z in zs)
                    {
                        foreach (var t in times)
                        {
                            yield return TemporalFluence(dp, rho, z, t);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// fluence as a function of s-d separations, depth z and temporal-frequencies
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <param name="fts">temporal-frequencies</param>
        /// <returns>fluence(rhos,zs,fts)</returns>
        public override IEnumerable<Complex> FluenceOfRhoAndZAndFt(
            IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos,
            IEnumerable<double> zs, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var rho in rhos)
                {
                    foreach (var z in zs)
                    {
                        foreach (var ft in fts)
                        {
                            Complex k = ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
                            (dp.cn * dp.D)).SquareRoot();

                            yield return TemporalFrequencyFluence(dp, rho, z, k).Magnitude;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// fluence as a function of spatial-frequency and depth z
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial-frequencies</param>
        /// <param name="zs">depths</param>
        /// <returns>fluence(fxs,zs)</returns>
        public override IEnumerable<double> FluenceOfFxAndZ(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var fx in fxs)
                {
                    foreach (var z in zs)
                    {
                        yield return SFDDiffusionForwardSolver.StationaryOneDimensionalSpatialFrequencyFluence(
                            dp, fx, z);
                    }
                }
            }
        }
        /// <summary>
        /// fluence as a function of spatial-frequency and depth z and time
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial-frequencies</param>
        /// <param name="zs">depths</param>
        /// <param name="times">times</param>
        /// <returns>fluence(fxs,zs,times)</returns>
        public override IEnumerable<double> FluenceOfFxAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var fx in fxs)
                {
                    foreach (var z in zs)
                    {
                        foreach (var t in times)
                        {
                            yield return
                                HankelTransform.DigitalFilterOfOrderZero(
                                    fx, rho => TemporalFluence(dp, rho, z, t));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// fluence as a function of spatial-frequency and temporal-frequency
        /// </summary>
        /// <param name="ops">IEnumerable of optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="zs">depths</param>
        /// <param name="fts">temporal-frequencies</param>
        /// <returns>fluence(fxs,z,fts)</returns>
        public override IEnumerable<Complex> FluenceOfFxAndZAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        #endregion Fluence Solutions

        #region Abstract methods

        //Reflectance
        /// <summary>
        /// time-independent reflectance
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="fr1"></param>
        /// <param name="fr2"></param>
        /// <returns>R(rho)</returns>
        public abstract double StationaryReflectance(
            DiffusionParameters dp, double rho, double fr1, double fr2);
        /// <summary>
        /// time-dependent reflectance
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="t">time [ns]</param>
        /// <param name="fr1"></param>
        /// <param name="fr2"></param>
        /// <returns>R(rho,time)</returns>
        public abstract double TemporalReflectance(
            DiffusionParameters dp, double rho, double t, double fr1, double fr2);
        /// <summary>
        /// reflectance as a function of s-d separation and temporal-frequency
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="k">temporal-frequency</param>
        /// <param name="fr1"></param>
        /// <param name="fr2"></param>
        /// <returns></returns>
        public abstract Complex TemporalFrequencyReflectance(
            DiffusionParameters dp, double rho, Complex k, double fr1, double fr2);

        //Fluence
        /// <summary>
        /// time-independent fluence 
        /// </summary>
        /// <param name="rho">s-d separation</param>
        /// <param name="z">depth</param>
        /// <param name="dp">diffusion parameters</param>
        /// <returns>fluence(rho,z)</returns>
        public abstract double StationaryFluence(
            double rho, double z, DiffusionParameters dp);
        /// <summary>
        /// temporal fluence
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="z">depth</param>
        /// <param name="t">time</param>
        /// <returns>fluence(rho,z,time)</returns>
        public abstract double TemporalFluence(
            DiffusionParameters dp, double rho, double z, double t);
        /// <summary>
        /// fluence as a function of s-d separation, z depth and temporal-frequency
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="z">depth</param>
        /// <param name="k">temporal frequency</param>
        /// <returns></returns>
        public abstract Complex TemporalFrequencyFluence(
            DiffusionParameters dp, double rho, double z, Complex k);

        #endregion

    }
}
