using System;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;
using Meta.Numerics.Functions;

namespace Vts.Modeling.ForwardSolvers.Extensions
{
    public static class PhotonHittingDensityExtensions
    {
        // Steady-State Domain...
        ///// <summary>
        ///// Steady-State centerline Photon Hitting Density by the Green's function multiplication
        ///// </summary>
        ///// <param name="ops">optical properties object</param>
        ///// <param name="rhos">Source Detector separation</param>
        ///// <param name="rProbe">Radial distance from source to "iterogation" location</param>
        ///// <param name="z">Depth being probed</param>
        ///// <returns>The Photon Hitting Density at specified location</returns>
        //public IEnumerable<double> SteadyStatePointSourceCenterlinePHD(IEnumerable<OpticalProperties> ops, IEnumerable<double> rProbes,
        //     IEnumerable<double> rhos, IEnumerable<double> zs)
        //{
        //    foreach (var op in ops)
        //    {
        //        DiffusionParameters dp = DiffusionParameters.Create(op, ForwardModel.SDA);
        //        foreach (var rProbe in rProbes)
        //        {
        //            foreach (var rho in rhos)
        //            {
        //                foreach (var z in zs)
        //                {
        //                    var r11 = DiffusionBase.Radius1(rProbe, z, dp.zp);
        //                    var r12 = DiffusionBase.Radius2(rProbe, z, dp.zp, dp.zb);
        //                    var r21 = DiffusionBase.Radius1(rho - rProbe, z, 0.0);
        //                    var r22 = DiffusionBase.Radius2(rho - rProbe, z, 0.0, dp.zb);
        //                    var fluence1 = DiffusionBase.SteadyStatePointSourceImageGreensFunction(dp, r11, r12);
        //                    var fluence2 = DiffusionBase.SteadyStatePointSourceImageGreensFunction(dp, r21, r22);
        //                    yield return (fluence1 * fluence2);
        //                }
        //            }
        //        }
        //    }
        //}


        public static IEnumerable<Complex> TimeFrequencyDomainFluence2SurfacePointPHD(
            this IForwardSolver myForwardSolver,
            double timeModulationFrequency,
            IEnumerable<OpticalProperties> ops,
             IEnumerable<double> rhoPrimes, IEnumerable<double> zs)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, ForwardModel.SDA);
                Complex k =
                (
                    (op.Mua * dp.cn + Complex.ImaginaryOne * timeModulationFrequency * 2 * Math.PI) /
                    (dp.D * dp.cn)
                ).SquareRoot();
                foreach (var rho in rhoPrimes)
                {
                    foreach (var z in zs)
                    {
                        var r21 = CalculatorToolbox.GetRadius(rho, z);
                        var r22 = CalculatorToolbox.GetRadius(rho, z + 2 * dp.zb);
                        yield return
                            DiffusionGreensFunctions.TemporalFrequencyPointSourceImageGreensFunction(dp, r21, r22, k);
                    }
                }
            }
        }


        public static IEnumerable<double> SteadyStateFluence2SurfacePointPHD(
            this IForwardSolver myForwardSolver,
            IEnumerable<OpticalProperties> ops,
             IEnumerable<double> rhoPrimes, IEnumerable<double> zs)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, ForwardModel.SDA);
                foreach (var rho in rhoPrimes)
                {
                    foreach (var z in zs)
                    {
                        var r21 = CalculatorToolbox.GetRadius(rho, z);
                        var r22 = CalculatorToolbox.GetRadius(rho, z + 2 * dp.zb);
                        yield return
                            DiffusionGreensFunctions.StationaryPointSourceImageGreensFunction(dp, r21, r22);
                    }
                }
            }
        }

        public static IEnumerable<double> TemporalPointSourceCenterlinePHD(
            this IForwardSolver myForwardSolver,
            IEnumerable<OpticalProperties> ops, IEnumerable<double> rProbes,
            IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, ForwardModel.SDA);
                foreach (var rProbe in rProbes)
                {
                    foreach (var rho in rhos)
                    {
                        foreach (var z in zs)
                        {
                            var r11 = CalculatorToolbox.GetRadius(rProbe, z - dp.zp);
                            var r12 = CalculatorToolbox.GetRadius(rProbe, z + dp.zp + 2 * dp.zb);
                            var r21 = CalculatorToolbox.GetRadius(rho - rProbe, z);
                            var r22 = CalculatorToolbox.GetRadius(rho - rProbe, z + 2 * dp.zb);

                            foreach (var t in ts)
                            {
                                Meta.Numerics.Function<double, double> integrandConvolve =
                                    tau =>
                                    {
                                        return 
                                            DiffusionGreensFunctions.TemporalPointSourceImageGreensFunction
                                            (dp, r11, r12, tau) * 
                                            DiffusionGreensFunctions.TemporalPointSourceImageGreensFunction
                                            (dp, r21, r22, t - tau);
                                    };
                                yield return FunctionMath.Integrate(
                                    integrandConvolve, Meta.Numerics.Interval.FromEndpoints(0.0, t));
                            }
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// Modified from a code of Fred's written in Matlab
        ///// </summary>
        ///// <param name="dp">diffusion parameters object</param>
        ///// <param name="k">complex diffusion constant, ((mua*cn +i*(ft*2*pi))/(D*cn)).^0.5</param>
        ///// <param name="rho">source-detector separation</param>
        ///// <param name="rProbe">radius from source</param>
        ///// <param name="y">omitted</param>
        ///// <param name="z">depth</param>
        ///// <param name="ft">temporal frequency</param>
        ///// <returns></returns>
        //public static double DepthProbFrequencyDomainPhotonMigration(DiffusionParameters dp, Complex k,
        //    double rho, double rProbe, double z, double ft)
        //{
        //    var r11 = DiffusionBase.Radius1(rho, z, dp.zp);
        //    var r12 = DiffusionBase.Radius2(rho, z, dp.zp, dp.zb);
        //    var r21 = DiffusionBase.Radius1(rProbe - rho, z, 0.0);
        //    var r22 = DiffusionBase.Radius2(rProbe - rho, z, 0.0, dp.zb);
        //    var phi1 = SDAForwardSolver.TemporalFrequencyFluence(dp, k, r11, r12);
        //    var phi2 = SDAForwardSolver.TemporalFrequencyFluence(dp, k, r21, r22);
        //    return (phi1 * phi2).Modulus; // see Kienle and Patterson, JOSA A 14(1), 246-254,1997
        //}

        public static IEnumerable<double> TemporalFrequencyPointSourceCenterlinePHD(
            this IForwardSolver myForwardSolver, IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rProbes, IEnumerable<double> rhos, 
            IEnumerable<double> zs, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {
                DiffusionParameters dp = DiffusionParameters.Create(op, ForwardModel.SDA);
                foreach (var rProbe in rProbes)
                {
                    foreach (var rho in rhos)
                    {
                        foreach (var z in zs)
                        {
                            var r11 = CalculatorToolbox.GetRadius(rProbe, z - dp.zp);
                            var r12 = CalculatorToolbox.GetRadius(rProbe, z + dp.zp + 2 * dp.zb);
                            var r21 = CalculatorToolbox.GetRadius(rho - rProbe, z);
                            var r22 = CalculatorToolbox.GetRadius(rho - rProbe, z + 2 * dp.zb);

                            foreach (var ft in fts)
                            {
                                Complex k = 
                                    (
                                        (op.Mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) / 
                                        (dp.D * dp.cn)
                                    ).SquareRoot();
                                var phi1 = DiffusionGreensFunctions.TemporalFrequencyPointSourceImageGreensFunction(
                                        dp, r11, r12, k);
                                var phi2 = DiffusionGreensFunctions.TemporalFrequencyPointSourceImageGreensFunction(
                                    dp, r21, r22, k);

                                yield return (phi1 * phi2).Magnitude;
                            }
                        }
                    }
                }
            }
        }

    }
}
