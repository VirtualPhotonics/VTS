using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// SourceConfiguration enum
    /// </summary>
    public enum SourceConfiguration
    {
        Point,
        Distributed,
        Gaussian,
    }
    /// <summary>
    /// ForwardModel enum
    /// </summary>
    public enum ForwardModel
    {
        SDA,
        DeltaPOne,
    }

    public abstract class DiffusionForwardSolverBase : ForwardSolverBase
    {
        protected DiffusionForwardSolverBase(SourceConfiguration sourceConfiguration, double beamDiameter)
            : base(sourceConfiguration, beamDiameter) { }

        protected ForwardModel ForwardModel { get; set; }

        #region IForwardSolver Members

        public override double ROfRho(OpticalProperties op, double rho)
        {
            return ROfRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }

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

        public override double ROfRhoAndTime(
            OpticalProperties op, double rho, double t)
        {
            return ROfRhoAndTime(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> ROfRhoAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> ts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op.N);
                var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op.N);
                foreach (var rho in rhos)
                {
                    foreach (var t in ts)
                    {
                        yield return TemporalReflectance(dp, rho, t, fr1, fr2);
                    }
                }
            }
        }

        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return ROfRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        }

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

        public override double ROfFxAndTime(OpticalProperties op, double fx, double t)
        {
            return ROfFxAndTime(op.AsEnumerable(), fx.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> ROfFxAndTime(
            IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts)
        {
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    foreach (var t in ts)
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

        protected static Complex GetBackwardHemisphereIntegralDiffuseReflectance(
            Complex fluence, Complex flux, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(fluence.Real, flux.Real, fr1, fr2) +
                Complex.ImaginaryOne *
                GetBackwardHemisphereIntegralDiffuseReflectance(fluence.Imaginary, flux.Imaginary, fr1, fr2);
        }


        #endregion Helper Methods

        #region Fluence Solutions

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

        public override IEnumerable<double> FluenceOfRhoAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var rho in rhos)
                {
                    foreach (var z in zs)
                    {
                        foreach (var t in ts)
                        {
                            yield return TemporalFluence(dp, rho, z, t);
                        }
                    }
                }
            }
        }


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

        public override IEnumerable<double> FluenceOfFxAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
                foreach (var fx in fxs)
                {
                    foreach (var z in zs)
                    {
                        foreach (var t in ts)
                        {
                            yield return
                                HankelTransform.DigitalFilterOfOrderZero(
                                    fx, rho => TemporalFluence(dp, rho, z, t));
                        }
                    }
                }
            }
        }

        public override IEnumerable<Complex> FluenceOfFxAndZAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
            //foreach (var op in ops)
            //{
            //    foreach (var fx in fxs)
            //    {
            //        foreach (var z in zs)
            //        {
            //            foreach (var ft in fts)
            //            {
            //                yield return 
            //            }
            //        }
            //    }
            //}
        }

        #endregion Fluence Solutions

        #region Abstract methods

        //Reflectance
        public abstract double StationaryReflectance(
            DiffusionParameters dp, double rho, double fr1, double fr2);
        public abstract double TemporalReflectance(
            DiffusionParameters dp, double rho, double t, double fr1, double fr2);
        public abstract Complex TemporalFrequencyReflectance(
            DiffusionParameters dp, double rho, Complex k, double fr1, double fr2);

        //Fluence
        public abstract double StationaryFluence(
            double rho, double z, DiffusionParameters dp);
        public abstract double TemporalFluence(
            DiffusionParameters dp, double rho, double z, double t);
        public abstract Complex TemporalFrequencyFluence(
            DiffusionParameters dp, double rho, double z, Complex k);

        #endregion

    }
}
