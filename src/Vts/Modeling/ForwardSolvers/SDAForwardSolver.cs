using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Meta.Numerics.Functions;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    public class SDAForwardSolver //: DiffusionBase
    {
        public SDAForwardSolver(SourceConfiguration sourceConfiguration, double beamDiameter)
            : base(sourceConfiguration, beamDiameter)
        {
            this.ForwardModel = ForwardModel.SDA;
        }

        public SDAForwardSolver() : this(SourceConfiguration.Distributed, 0.0) { }

        #region override methods (should not be necessary following one more round of code refactoring/cleanup
        public override double StationaryReflectance(DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            return 0;
        }

        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            return 0;
        }

        public override double TemporalReflectance(DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            return 0;
        }
        public override double TemporalFluence(DiffusionParameters dp, double rho, double z, double t)
        {
            return 0;
        }
        #endregion

        #region IForwardSolver Members

        #region Real Domain

        /// <summary>
        /// from JOSA 14(1) 1997
        /// </summary>
        /// <param name="op">Optical properties object</param>
        /// <param name="rho">rho</param>
        /// <param name="ft"></param>
        /// <param name="t">time</param>
        /// <returns></returns>
        /// <summary>
        /// from Kienle AO 37(4), 1998f
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="rhos"></param>
        /// <param name="fts"></param>
        /// <returns></returns>
        //public override Complex RofRhoAndFt(OpticalProperties op, double rho, double ft)
        //{
        //    return RofRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        //}

        //public override IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        //{
        //    switch (SourceConfiguration)
        //    {
        //        case SourceConfiguration.Point:
        //        default:
        //            return TemporalFrequencyPointSourceReflectance(ops, rhos, fts);
        //        case SourceConfiguration.Distributed:
        //            return TemporalFrequencyDistributedSourceReflectance(ops, rhos, fts);
        //        case SourceConfiguration.Gaussian:
        //            return (0.0 + 0.0 * Complex.ImaginaryOne).AsEnumerable();// TemporalFrequencyDistributedSourceReflectanceWithMarshak(ops, rhos, fts);
        //    }
        //}

        //private IEnumerable<Complex> TemporalFrequencyPointSourceReflectance(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        //{
        //    foreach (var op in ops)
        //    {
        //        DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
        //        var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op.N);
        //        var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op.N);
        //        foreach (var rho in rhos)
        //        {
        //            var r1 = CalculatorToolbox.GetRadius(rho, -dp.zp);
        //            var r2 = CalculatorToolbox.GetRadius(rho, dp.zp + 2 * dp.zb);

        //            foreach (var ft in fts)
        //            {
        //                Complex k = ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
        //                    (dp.cn * dp.D)).SquareRoot();
        //                Complex Intensity = GetBackwardHemisphereIntegralDiffuseReflectance(
        //                    TemporalFrequencyPointSourceFluence(dp, k, r1, r2),
        //                    -TemporalFrequencyPointSourceFlux(dp, k, r1, r2),
        //                    fr1, fr2);
        //                yield return Intensity;//TODO: make this work with Complex return value (ie. both amplitude & phase info)
        //            }
        //        }
        //    }
        //}

        //private IEnumerable<Complex> TemporalFrequencyDistributedSourceReflectance(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        //{
        //    foreach (var op in ops)
        //    {
        //        DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
        //        var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op.N);
        //        var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op.N);
        //        foreach (var rho in rhos)
        //        {
        //            foreach (var ft in fts)
        //            {
        //                Complex k = ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) / (dp.cn * dp.D)).SquareRoot();
        //                Complex Intensity = GetBackwardHemisphereIntegralDiffuseReflectance(
        //                     TemporalFrequencyDistributedSourceFluence(dp, k, rho, 0.0),
        //                     -TemporalFrequencyDistributedSourceFlux(dp, k, rho, 0.0),
        //                     fr1, fr2);
        //                yield return Intensity; //TODO: make this work with Complex return value (ie. both amplitude & phase info)
        //            }
        //        }
        //    }
        //}
        #endregion

        //#region Spatial Frequency

        ///// <summary>
        ///// RofFx, solves SDA using Cuccia et al JBO, March/April 2009 
        ///// </summary>
        ///// <param name="op"></param>
        ///// <param name="fx"></param>
        ///// <returns></returns>
        //public override double RofFx(OpticalProperties op, double fx)
        //{
        //    return RofFx(op.AsEnumerable(), fx.AsEnumerable()).First();
        //}

        ///// <summary>
        ///// Vectorized RofFx. Solves SDA using Cuccia et al JBO, March/April 2009 
        ///// </summary>
        ///// <param name="ops"></param>
        ///// <param name="fxs"></param>
        ///// <returns></returns>
        //public override IEnumerable<double> RofFx(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        //{
        //    double fourPiSq = 4.0 * Math.PI * Math.PI;
        //    foreach (var op in ops)
        //    {
        //        var Afactor = 3 / 2 / CalculatorToolbox.GetCubicAParameter(op.N);
        //        var mutr = op.Mua + op.Musp;
        //        var mueffSq = 3.0 * op.Mua * mutr;
        //        foreach (var fx in fxs)
        //        {
        //            var mueffPrime = Math.Sqrt(mueffSq + fourPiSq * fx * fx);
        //            var ratio = mueffPrime / mutr;
        //            yield return Afactor * (op.Musp / mutr) / (ratio + 1) / (ratio + Afactor);
        //        }
        //    }
        //}


        //public override double RofFxAndT(OpticalProperties op, double fx, double t)
        //{
        //    return RofFxAndT(op.AsEnumerable(), fx.AsEnumerable(), t.AsEnumerable()).First();
        //}

        //public override IEnumerable<double> RofFxAndT(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts)
        //{
        //    foreach (var op in ops)
        //    {
        //        foreach (var fx in fxs)
        //        {
        //            foreach (var t in ts)
        //            {
        //                yield return
        //                    HankelTransform.DigitalFitlerOfOrderZero(
        //                        fx, rho => RofRhoAndT(op, rho, t));
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Modulation frequency-dependent reflectance. Modified from Pham et al, Appl. Opt. Sept 2000 
        ///// to include spatial modulation, as described in Cuccia et al, J. Biomed. Opt. March/April 2009
        ///// </summary>
        ///// <param name="op"></param>
        ///// <param name="fx"></param>
        ///// <param name="ft"></param>
        ///// <returns></returns>
        //public override Complex RofFxAndFt(OpticalProperties op, double fx, double ft)
        //{
        //    double A = CalculatorToolbox.GetCubicAParameter(op.N);
        //    double wOverC = Math.PI * ft * op.N / GlobalConstants.C;
        //    double mutr = op.Mua + op.Musp;
        //    Complex D = 1 / (3 * mutr * (1.0 + Complex.ImaginaryOne * wOverC / mutr));
        //    Complex mueff =
        //    (
        //        3.0 * op.Mua * mutr -
        //        3.0 * wOverC * wOverC +
        //        Complex.ImaginaryOne * (1 + op.Mua / mutr) * 3.0 * mutr * wOverC +
        //        4.0 * Math.PI * Math.PI * fx * fx
        //    ).SquareRoot();

        //    Complex temp = mueff * D;

        //    return (3 * A * op.Musp * D / (temp + 1.0 / 3.0) / (temp + A));
        //}

        //#endregion Spatial Frequency

        //public override IEnumerable<double> FluenceofRhoAndFt(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos,
        //    IEnumerable<double> zs, IEnumerable<double> fts)
        //{
        //    if (SourceConfiguration == SourceConfiguration.Distributed)
        //    {
        //        return TemporalFrequencyDistributedSourceFluence(ops, rhos, zs, fts);
        //    }
        //    else
        //    {
        //        return TemporalFrequencyPointSourceFluence(ops, rhos, zs, fts);
        //    }
        //}


        #endregion IForwardSolver Members

        #region Fluence models

        // Temporal Frequency domain...

        //private IEnumerable<double> TemporalFrequencyPointSourceFluence(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos,
        //    IEnumerable<double> zs, IEnumerable<double> fts)
        //{
        //    foreach (var op in ops)
        //    {
        //        DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
        //        foreach (var rho in rhos)
        //        {
        //            foreach (var z in zs)
        //            {
        //                var r1 = CalculatorToolbox.GetRadius(rho, z - dp.zp);
        //                var r2 = CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb);
        //                foreach (var ft in fts)
        //                {
        //                    Complex k =
        //                        ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
        //                        (dp.D * dp.cn)).SquareRoot();
        //                    yield return TemporalFrequencyPointSourceFluence(dp, k, r1, r2).Magnitude;
        //                }
        //            }
        //        }
        //    }
        //}

        //private IEnumerable<double> TemporalFrequencyDistributedSourceFluence(
        //    IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos,
        //    IEnumerable<double> zs, IEnumerable<double> fts)
        //{
        //    foreach (var op in ops)
        //    {
        //        DiffusionParameters dp = DiffusionParameters.Create(op, this.ForwardModel);
        //        foreach (var rho in rhos)
        //        {
        //            foreach (var z in zs)
        //            {
        //                foreach (var ft in fts)
        //                {
        //                    Complex k =
        //                        ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
        //                        (dp.D * dp.cn)).SquareRoot();

        //                    yield return TemporalFrequencyDistributedSourceFluence(dp, k, rho, z).Magnitude;
        //                }
        //            }
        //        }
        //    }
        //}

        //private Complex TemporalFrequencyDistributedSourceFluence(
        //    DiffusionParameters dp, Complex k, double rho, double z)
        //{

        //    Meta.Numerics.Function<double, double> realIntegrand = zp =>
        //    {
        //        var r1 = CalculatorToolbox.GetRadius(rho, z - zp);
        //        var r2 = CalculatorToolbox.GetRadius(rho, z + zp + 2 * dp.zb);
        //        return RealTemporalFrequencyPointSourceFluence(
        //            dp.D, k.Real, k.Imaginary, r1, r2) * Math.Exp(-dp.mutTilde * zp);
        //    };

        //    Meta.Numerics.Function<double, double> imaginaryIntegrand = zp =>
        //    {

        //        var r1 = CalculatorToolbox.GetRadius(rho, z - zp);
        //        var r2 = CalculatorToolbox.GetRadius(rho, z + zp + 2 * dp.zb);
        //        return ImaginaryTemporalFrequencyPointSourceFluence(
        //            dp.D, k.Real, k.Imaginary, r1, r2) * Math.Exp(-dp.mutTilde * zp);
        //    };

        //    return
        //        (FunctionMath.Integrate(
        //             realIntegrand,
        //             Meta.Numerics.Interval.FromEndpoints(0.0, Double.PositiveInfinity)) -
        //         Complex.ImaginaryOne *
        //         FunctionMath.Integrate(
        //            imaginaryIntegrand,
        //            Meta.Numerics.Interval.FromEndpoints(0.0, Double.PositiveInfinity))
        //        ) * dp.musTilde;
        //}

        //private Complex TemporalFrequencyDistributedSourceFlux(
        //    DiffusionParameters dp, Complex k, double rho, double z)
        //{
        //    DiffusionParameters dpLocalForReal = DiffusionParameters.Copy(dp);
        //    DiffusionParameters dpLocalForImag = DiffusionParameters.Copy(dp);

        //    Meta.Numerics.Function<double, double> realIntegrand =
        //        zp =>
        //        {
        //            dpLocalForReal.zp = zp;
        //            var r1 = CalculatorToolbox.GetRadius(rho, z - zp);
        //            var r2 = CalculatorToolbox.GetRadius(rho, z + zp + 2 * dp.zb);
        //            return RealTemporalFrequencyPointSourceFlux(dpLocalForReal, k.Real, k.Imaginary, r1, r2) *
        //                Math.Exp(-dpLocalForReal.mutTilde * zp);
        //        };
        //    Meta.Numerics.Function<double, double> imaginaryIntegrand =
        //        zp =>
        //        {
        //            dpLocalForImag.zp = zp;
        //            var r1 = CalculatorToolbox.GetRadius(rho, z - zp);
        //            var r2 = CalculatorToolbox.GetRadius(rho, z + zp + 2 * dp.zb);
        //            return ImaginaryTemporalFrequencyPointSourceFlux(dpLocalForImag, k.Real, k.Imaginary, r1, r2) *
        //                Math.Exp(-dpLocalForImag.mutTilde * zp);
        //        };

        //    return
        //        (FunctionMath.Integrate(
        //             realIntegrand,
        //             Meta.Numerics.Interval.FromEndpoints(0.0, Double.PositiveInfinity)) -
        //         Complex.ImaginaryOne *
        //         FunctionMath.Integrate(
        //             imaginaryIntegrand,
        //             Meta.Numerics.Interval.FromEndpoints(0.0, Double.PositiveInfinity))
        //        ) * dp.musTilde;
        //}

        ///// <summary>
        ///// Modulation temporal frequency-dependent fluence/flux from Kienle PMB 42 (1997), 1801 - 1819
        ///// </summary>
        ///// <param name="op"></param>
        ///// <param name="rho"></param>
        ///// <param name="ft"></param>
        ///// <param name="z"></param>
        ///// <returns></returns>
        //public static Complex TemporalFrequencyPointSourceFluence(
        //    OpticalProperties op, double rho, double ft, double z)
        //{
        //    DiffusionParameters dp = DiffusionParameters.Create(op, ForwardModel.SDA);
        //    Complex k = ((op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) / (dp.D * dp.cn)).SquareRoot();
        //    var r1 = CalculatorToolbox.GetRadius(rho, z - dp.zp);
        //    var r2 = CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb);
        //    return TemporalFrequencyPointSourceFluence(dp, k, r1, r2);
        //}

        //public static Complex TemporalFrequencyPointSourceFluence(
        //    DiffusionParameters dp, Complex k, double r1, double r2)
        //{
        //    var kImagR1 = k.Imaginary * r1;
        //    var kImagR2 = k.Imaginary * r2;

        //    Complex phiSource =
        //        Math.Exp(-k.Real * r1) / r1 * (Math.Cos(kImagR1) -
        //        Complex.ImaginaryOne * Math.Sin(kImagR1));
        //    Complex phiImage =
        //        Math.Exp(-k.Real * r2) / r2 * (Math.Cos(kImagR2) -
        //        Complex.ImaginaryOne * Math.Sin(kImagR2));

        //    return (phiSource - phiImage) / (4.0 * Math.PI * dp.D);
        //}

        //private static double RealTemporalFrequencyPointSourceFluence(
        //    double D, double kReal, double kImag, double r1, double r2)
        //{
        //    var phiSource = Math.Exp(-kReal * r1) / r1 * Math.Cos(kImag * r1);
        //    var phiImage = Math.Exp(-kReal * r2) / r2 * Math.Cos(kImag * r2);
        //    return (phiSource - phiImage) / (4.0 * Math.PI * D);
        //}

        //private static double ImaginaryTemporalFrequencyPointSourceFluence(
        //    double D, double kReal, double kImag, double r1, double r2)
        //{
        //    var phiSource = Math.Exp(-kReal * r1) / r1 * Math.Sin(kImag * r1);
        //    var phiImage = Math.Exp(-kReal * r2) / r2 * Math.Sin(kImag * r2);
        //    return (phiSource - phiImage) / (4.0 * Math.PI * D);
        //}

        //private static Complex TemporalFrequencyPointSourceFlux(
        //    DiffusionParameters dp, Complex k, double r1, double r2)
        //{
        //    var kImagR1 = k.Imaginary * r1;
        //    var kImagR2 = k.Imaginary * r2;
        //    var fluxSource = dp.zp * (k + 1 / r1) * Math.Exp(-k.Real * r1) / r1 / r1 *
        //        (Math.Cos(kImagR1) - Complex.ImaginaryOne * Math.Sin(kImagR1));
        //    var fluxImage = (dp.zp + 2 * dp.zb) * (k + 1 / r2) * Math.Exp(-k.Real * r2) / r2 / r2 *
        //        (Math.Cos(kImagR2) - Complex.ImaginaryOne * Math.Sin(kImagR2));
        //    return (fluxSource + fluxImage) / 4 / Math.PI;
        //}

        //private static double RealTemporalFrequencyPointSourceFlux(
        //    DiffusionParameters dp, double kReal, double kImag, double r1, double r2)
        //{
        //    var kImagR1 = kImag * r1;
        //    var kImagR2 = kImag * r2;
        //    var fluxSource = dp.zp * Math.Exp(-kReal * r1) / r1 / r1 *
        //        ((kReal + 1 / r1) * Math.Cos(kImagR1) + kImag * Math.Sin(kImagR1));
        //    var fluxImage = (dp.zp + 2 * dp.zb) * Math.Exp(-kReal * r2) / r2 / r2 *
        //        ((kReal + 1 / r2) * Math.Cos(kImagR2) + kImag * Math.Sin(kImagR2));
        //    return (fluxSource + fluxImage) / 4 / Math.PI;
        //}

        //private static double ImaginaryTemporalFrequencyPointSourceFlux(
        //    DiffusionParameters dp, double kReal, double kImag, double r1, double r2)
        //{
        //    var kImagR1 = kImag * r1;
        //    var kImagR2 = kImag * r2;
        //    var fluxSource = dp.zp * Math.Exp(-kReal * r1) / r1 / r1 *
        //        ((kReal + 1 / r1) * Math.Sin(kImagR1) - kImag * Math.Cos(kImagR1));
        //    var fluxImage = (dp.zp + 2 * dp.zb) * Math.Exp(-kReal * r2) / r2 / r2 *
        //        ((kReal + 1 / r2) * Math.Sin(kImagR2) - kImag * Math.Cos(kImagR2));
        //    return (fluxSource + fluxImage) / 4 / Math.PI;
        //}

        #endregion


        public override Complex TemporalFrequencyReflectance(DiffusionParameters dp, double rho, Complex k, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }

        public override double TemporalFrequencyFluence(DiffusionParameters dp, double rho, double z, Complex k)
        {
            throw new NotImplementedException();
        }
    }
}
