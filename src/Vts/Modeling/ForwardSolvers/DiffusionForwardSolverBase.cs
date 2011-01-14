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

        public override double RofRho(OpticalProperties op, double rho)
        {
            return RofRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofRho(
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

        public override double RofRhoAndT(
            OpticalProperties op, double rho, double t)
        {
            return RofRhoAndT(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofRhoAndT(
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

        public override Complex RofRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return RofRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        }

        public override IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops,
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
        /// RofFx, solves SDA using Cuccia et al JBO, March/April 2009 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="fx"></param>
        /// <returns></returns>
        public override double RofFx(OpticalProperties op, double fx)
        {
            return RofFx(op.AsEnumerable(), fx.AsEnumerable()).First();
        }

        /// <summary>
        /// Vectorized RofFx. Solves SDA using Cuccia et al JBO, March/April 2009 
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="fxs"></param>
        /// <returns></returns>
        public override IEnumerable<double> RofFx(
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

        public override double RofFxAndT(OpticalProperties op, double fx, double t)
        {
            return RofFxAndT(op.AsEnumerable(), fx.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofFxAndT(
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
                            HankelTransform.DigitalFitlerOfOrderZero(
                                2 * Math.PI * fx, rho => RofRhoAndT(op, rho, t));
                    }
                }
            }
        }

        /// <summary>
        /// Modulation frequency-dependent reflectance. Modified from Pham et al, Appl. Opt. Sept 2000 
        /// to include spatial modulation, as described in Cuccia et al, J. Biomed. Opt. March/April 2009
        /// </summary>
        /// <param name="op"></param>
        /// <param name="fx"></param>
        /// <param name="ft"></param>
        /// <returns></returns>
        public override Complex RofFxAndFt(OpticalProperties op, double fx, double ft)
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

        /// <param name="fluence"></param>
        /// <param name="flux"></param>
        /// <param name="FR1">1st moment of Fresnel Reflection</param>
        /// <param name="FR2">2nd moment of Fresnel Reflection</param>
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

        public override IEnumerable<double> FluenceofRho(
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

        public override IEnumerable<double> FluenceofRhoAndT(
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


        public override IEnumerable<double> FluenceofRhoAndFt(
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

        public override IEnumerable<double> FluenceofFx(
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

        public override IEnumerable<double> FluenceofFxAndT(
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
                                HankelTransform.DigitalFitlerOfOrderZero(
                                    fx, rho => TemporalFluence(dp, rho, z, t));
                        }
                    }
                }
            }
        }

        public override IEnumerable<double> FluenceofFxAndFt(
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
