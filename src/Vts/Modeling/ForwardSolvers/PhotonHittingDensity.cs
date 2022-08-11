using MathNet.Numerics;
using Meta.Numerics.Analysis;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Vts.Modeling.ForwardSolvers.Extensions
{
    /// <summary>
    /// The <see cref="Extensions"/> namespace contains the extension methods for photon hitting density map generation in the Virtual Tissue Simulator
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Extension methods for photon hitting density map generation
    /// </summary>
    public static class PhotonHittingDensityExtensions
    {
        // Steady-State Domain...
        ///// <summary>
        ///// Steady-State centerline Photon Hitting Density by the Green's function multiplication
        ///// </summary>
        ///// <param name="ops">optical properties object</param>
        ///// <param name="rhos">Source Detector separation</param>
        ///// <param name="rProbe">Radial distance from source to "interrogation" location</param>
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

        /// <summary>
        /// temporal-frequency photon hitting density
        /// </summary>
        /// <param name="myForwardSolver">forward solver</param>
        /// <param name="timeModulationFrequency">temporal-frequency</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rhoPrimes">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <returns>Photon hitting density as a function of rhos, zs and ft</returns>
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

        /// <summary>
        /// time-independent fluence for photon hitting density
        /// </summary>
        /// <param name="myForwardSolver">forward solver</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rhoPrimes">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <returns>Photon hitting density as a function of rhos and zs</returns>
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
        /// <summary>
        /// time-dependent photon hitting density from point source
        /// </summary>
        /// <param name="myForwardSolver">forward solver</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rProbes">Radial distances from source to "interrogation" location</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <param name="ts">times</param>
        /// <returns>Photon hitting density as a function of rhos, zs and times</returns>
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
                                Func<double, double> integrandConvolve =
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

        /// <summary>
        /// temporal frequency photon hitting density from point source
        /// </summary>
        /// <param name="myForwardSolver">forward solver</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rProbes">Radial distances from source to "interrogation" location</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <param name="fts">temporal-frequencies</param>
        /// <returns>Photon hitting density as a function of rhos, zs and fts</returns>
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
