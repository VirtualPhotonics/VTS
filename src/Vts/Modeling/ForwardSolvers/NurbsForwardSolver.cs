using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Forward solver based on the Scaled Monte Carlo approach, proposed by Kienle and Patterson,
    /// used to evaluate the reflectance of a semi-infinite homogenous medium with g = 0.8 and n = 1.4.
    /// The reference time and space resolved reflectance, and the reference spatial frequency and
    /// time resolved reflectance are held in a NurbsGenerator class which computes the interpolation
    /// necessary to evaluate the reflectance in the specific domain.
    /// The interpolation is based on NURBS surfaces theory. The main reference used to implement
    /// this forward solver is 'The NURBS Book' by Las Piegl and Wayne Tiller.
    /// </summary>
    public class NurbsForwardSolver : ForwardSolverBase
    {
        #region fields

        private readonly INurbs _rdGenerator;
        private readonly INurbs _sfdGenerator;

        /// <summary>
        /// speed of light
        /// </summary>
        public static readonly double v =  GlobalConstants.C / 1.4;
        private static readonly OpticalProperties OpReference = new(0.0, 1, 0.8, 1.4);

        #endregion fields

        #region Properties
        /// <summary>
        /// The used FT is analytical or discrete based on this boolean value. Defaults to false
        /// </summary>
        public bool AnalyticIntegration { get; set; } = false;
        #endregion

        #region constructor

        /// <summary>
        /// Constructor which creates an instance of NurbsForwardSolver setting
        /// the NurbsGenerators to the values passed as Input.
        /// </summary>
        /// <param name="rdGenerator">real domain NurbsGenerator</param>
        /// <param name="sfdGenerator">spatial frequency domain generator</param>
        public NurbsForwardSolver(INurbs rdGenerator, INurbs sfdGenerator)                                                               
        {
            _rdGenerator = rdGenerator;
            _sfdGenerator = sfdGenerator;
        }

        /// <summary>
        /// Default class constructor called by solver factory.
        /// </summary>
        public NurbsForwardSolver()
            : this(new NurbsGenerator(NurbsGeneratorType.RealDomain),
                   new NurbsGenerator(NurbsGeneratorType.SpatialFrequencyDomain))
        {

        }
        
        /// <summary>
        /// Constructor used to create an instance of NurbsForwardSolver
        /// with the same stub NurbsGenerator for all the NurbsGenerators.
        /// Used for Unit Tests of the class.
        /// </summary>
        /// <param name="generator">stub NurbsGenerator</param>
        public NurbsForwardSolver(INurbs generator)
            : this(generator, generator)
        {

        }

        #endregion constructor

        #region IForwardSolver methods

        #region Real Domain

        /// <summary>
        /// Calls its vectorized version to evaluate the steady state reflectance at 
        /// a source detector separation rho, for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">source detector separation</param>
        /// <returns>space resolved reflectance</returns>
        public override double ROfRho(OpticalProperties op, double rho)
        {
            return ROfRho(op.AsEnumerable(), rho.AsEnumerable()).FirstOrDefault();
        }
        /// <summary>
        /// Returns the steady state reflectance for the specified optical properties
        /// at source detector separations rhos.
        /// The radial distance rho is scaled to the reference space to evaluate rho_ref. 
        /// If rho_ref is on the reference surface the reference rho-time resolved 
        /// reflectance is scaled and the isoparametric Nurbs curve is integrated
        /// analytically  over time. To evaluate the integral of the reflectance out of 
        /// the time range it evaluates the linear approximation of the logarithm of
        /// the tail of the curve and integrates it from tMax to infinity.
        /// If rho_ref is out of range the method returns 0.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separation</param>
        /// <returns>spatially resolved reflectance</returns>
        public override IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos)
        {
            foreach (var op in ops)
            {
                var scalingFactor = GetScalingFactor(op, 2);

                foreach (var rho in rhos)
                {
                    var rhoRef = rho * op.Musp / OpReference.Musp;
                    var exponentialTerm = op.Mua * v * OpReference.Musp / op.Musp;

                    if (rhoRef <= _rdGenerator.SpaceValues.MaxValue && rhoRef >= 0)
                    {
                        var integralValue = _rdGenerator.EvaluateNurbsCurveIntegral(rhoRef,exponentialTerm);                   
                        integralValue += ExtrapolateIntegralValueOutOfRange(_rdGenerator, rhoRef,op);
                        integralValue = CheckIfValidOutput(integralValue);
                        yield return integralValue * scalingFactor;
                    }
                    else
                    {
                        yield return 0.0;
                    }
                }
            }
        }

        /// <summary>
        /// Calls its vectorized version to evaluate the time and space resolved reflectance
        /// at a source detector separation rho and at time t, for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">source detector separation</param>
        /// <param name="time">time</param>
        /// <returns>spatial and temporal resolved reflectance</returns>
        public override double ROfRhoAndTime(OpticalProperties op, double rho, double time)
        {   
            return ROfRhoAndTime(op.AsEnumerable(), rho.AsEnumerable(), time.AsEnumerable()).FirstOrDefault();
        }
        /// <summary>
        /// Returns the reflectance at radial distance rho and time t scaling the
        /// reference rho-time resolved reflectance.
        /// The returned value is forced to zero if the time t is smaller then the
        /// minimal time of flight required to reach a detector at a distance rho. 
        /// If a point of the reference reflectance outside the time range of the
        /// surface is required, the value is extrapolated using the linear 
        /// approximation of the logarithm of R for two points placed at the end of
        /// the time range [Tmax - 0.1ns, Tmax].
        /// If the required point is outside the radial range a linear extrapolation 
        /// is used, based on the value of R at [0.95*RhoMax, RhoMax].
        /// If the required point is outside both ranges a linear combination of the
        /// two extrapolations is adopted.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separation</param>
        /// <param name="times">time</param>
        /// <returns>space and time resolved reflectance at rho and t</returns>
        public override IEnumerable<double> ROfRhoAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                var scalingFactor = GetScalingFactor(op, 3);

                foreach (var rho in rhos)
                {
                    var rhoRef = rho * op.Musp / OpReference.Musp;

                    foreach (var t in times)
                    {
                        var tRef = t * op.Musp / OpReference.Musp;
                        var scaledValue = tRef < _rdGenerator.GetMinimumValidTime(rhoRef) ? 0.0 : _rdGenerator.ComputeSurfacePoint(tRef, rhoRef);

                        if ((rhoRef > _rdGenerator.SpaceValues.MaxValue ||
                               tRef > _rdGenerator.TimeValues.MaxValue) &&
                               tRef > _rdGenerator.GetMinimumValidTime(rhoRef))
                        {
                            scaledValue = _rdGenerator.ComputePointOutOfSurface(tRef, rhoRef,
                                                                                 scaledValue);
                        }

                        scaledValue = CheckIfValidOutput(scaledValue);
                        
                        yield return scalingFactor * scaledValue * Math.Exp(-op.Mua * v * t);
                    }
                }
            }
        }
        
        /// <summary>
        ///  Calls its vectorized version to evaluate the temporal frequency and space resolved
        ///  reflectance at a source detector separation rho for a modulation frequency ft,
        ///  for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">source detector separation</param>
        /// <param name="ft">modulation frequency</param>
        /// <returns>Reflectance intensity</returns>
        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return ROfRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).FirstOrDefault();
        }

        /// <summary>
        ///  Evaluates the temporal frequency and space resolved reflectance at a source 
        ///  detector separation rho for a modulation frequency ft,for the specified 
        ///  optical properties. It calculates the Fourier transform of the NURBS
        ///  curve R(t) at the required source detector separation.
        ///  The used FT is analytical or discrete according to the Boolean value <see cref="AnalyticIntegration"/>
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separation</param>
        /// <param name="fts">modulation frequency</param>
        /// <returns>Reflectance intensity</returns>
        public override IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {   
                // Added a global public variable so that this code can be reached
                if (AnalyticIntegration)
                {
                    foreach (var rho in rhos)
                    {
                        var exponentialTerm = op.Mua * v * OpReference.Musp / op.Musp;
                        var rhoRef = rho * op.Musp / OpReference.Musp;
                        if (rhoRef <= _rdGenerator.SpaceValues.MaxValue)
                        {
                            foreach (var ft in fts)
                            {
                                yield return GetScalingFactor(op,2) * _rdGenerator.EvaluateNurbsCurveFourierTransform(rhoRef, exponentialTerm, ft * OpReference.Musp / op.Musp);
                            }
                        }
                        else
                        {
                            foreach (var ft in fts)
                            {
                                yield return new Complex(0.0, 0.0);
                            }
                        }
                    }
                }
                else
                {
                    var time = _rdGenerator.NativeTimes.ToArray();
                    for (var i = 0; i < time.Length; i++)
                    {
                        time[i] = time[i] * OpReference.Musp / op.Musp;
                    }
                    var deltaT = GetDeltaT(time);
                    
                    foreach (var rho in rhos)
                    {
                        if (rho * OpReference.Musp / op.Musp <= _rdGenerator.SpaceValues.MaxValue)
                        {
                            var ROfT = ROfRhoAndTime(op.AsEnumerable(), rho.AsEnumerable(), time);

                            foreach (var ft in fts)
                            {
                                yield return LinearDiscreteFourierTransform.GetFourierTransform(time.ToArray(), ROfT.ToArray(), deltaT.ToArray(), ft);  
                            }
                        }
                        else
                        {
                            foreach (var ft in fts)
                            {
                                yield return new Complex(0.0, 0.0);
                            }
                        }
                    }
                }
            }
        }
        
        #endregion Real Domain

        #region Spatial Frequency Domain

        /// <summary>
        /// Calls its vectorized version to evaluate the spatial frequency
        /// resolved reflectance for the spatial frequency fx, for the 
        /// specified optical properties. 
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <returns>spatial frequency resolved reflectance</returns>
        public override double ROfFx(OpticalProperties op, double fx)
        {
            return ROfFx(op.AsEnumerable(), fx.AsEnumerable()).FirstOrDefault();
        }

        /// <summary>
        /// Returns the spatial frequency resolved reflectance at fx applying the scaling on
        /// the reference fx-time resolved reflectance.
        /// Than integrates analytically the isoparametric NURBS curve over time if fx is on the
        /// surface.
        /// If fx is out of range it returns 0.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <returns>spatial frequency resolved reflectance</returns>
        public override IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    var fxRef = fx * OpReference.Musp / op.Musp;
                    var exponentialTerm = op.Mua * v * OpReference.Musp / op.Musp;

                    if (fxRef <= _sfdGenerator.SpaceValues.MaxValue && fxRef >= 0.0)
                    {
                        var integralValue = _sfdGenerator.EvaluateNurbsCurveIntegral(fxRef, exponentialTerm);
                        integralValue = CheckIfValidOutput(integralValue);
                        yield return integralValue;
                    }
                    else
                    {
                        yield return 0.0;
                    }
                }
            }

        }

        /// <summary>
        /// Calls its vectorized version to evaluate the time and space resolved reflectance
        /// for a spatial frequency, fx, and at time, t, for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="time">time</param>
        /// <returns>spatial frequency and time resolved reflectance</returns>
        public override double ROfFxAndTime(OpticalProperties op, double fx, double time)
        {
            return ROfFxAndTime(op.AsEnumerable(), fx.AsEnumerable(), time.AsEnumerable()).FirstOrDefault();
        }

        /// <summary>
        /// Returns the reflectance at spatial frequency, fx, and time, t, scaling the 
        /// reference fx-time resolved reflectance.
        /// If a point of the reference reflectance outside the time/spatial frequency range 
        /// of the surface is required, the value is extrapolated using the first derivative
        /// along the time/spatial frequency dimension.
        /// If the required point is outside both ranges a linear combination of the
        /// two derivatives is used.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <param name="times">time</param>
        /// <returns>spatial frequency and time resolved reflectance</returns>
        public override IEnumerable<double> ROfFxAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                var scalingFactor = GetScalingFactor(op, 1);

                foreach (var fx in fxs)
                {
                    var fxRef = fx * OpReference.Musp / op.Musp;

                    foreach (var t in times)
                    {
                        var tRef = t * op.Musp / OpReference.Musp;
                        if (fxRef > _sfdGenerator.SpaceValues.MaxValue || tRef > _sfdGenerator.TimeValues.MaxValue)
                        {
                            yield return 0.0;
                        }
                        else
                        {
                            var scaledValue = _sfdGenerator.ComputeSurfacePoint(tRef, fxRef);

                            scaledValue = CheckIfValidOutput(scaledValue);

                            yield return scalingFactor * scaledValue * Math.Exp(-op.Mua * v * t);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls its vectorized overload to evaluate the spatial frequency and temporal 
        /// frequency resolved reflectance.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>spatial frequency and temporal frequency resolved reflectance</returns>
        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return ROfFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }

        /// <summary>
        /// Evaluates the spatial frequency and temporal frequency resolved reflectance
        /// calculating the Fourier transform of the NURBS curve R(t) at the
        /// required spatial frequency for the specified optical properties. 
        /// The computed FT is analytical or discrete according to the Boolean value <see cref="AnalyticIntegration"/>.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <param name="fts">temporal frequency</param>
        /// <returns>spatial frequency and temporal frequency resolved reflectance</returns>
        public override IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {
                double fxRef;
                if (AnalyticIntegration)
                {
                    foreach (var fx in fxs)
                    {
                        fxRef = fx * OpReference.Musp / op.Musp;
                        var exponentialTerm = op.Mua * v * OpReference.Musp / op.Musp;

                        if (fxRef <= _sfdGenerator.SpaceValues.MaxValue)
                        {
                            foreach (var ft in fts)
                            {
                                var transformedValue = _sfdGenerator.EvaluateNurbsCurveFourierTransform(fxRef, exponentialTerm, ft * OpReference.Musp / op.Musp);
                                yield return Math.PI * transformedValue;
                            }
                        }
                        else
                        {
                            foreach (var ft in fts)
                            {
                                yield return new Complex(0.0, 0.0);
                            }
                        }
                    }
                }
                else
                {
                    var time = _sfdGenerator.TimeKnotSpanPolynomialCoefficients.Select(span => span.GetKnotSpanMidTime());
                    var deltaT = _sfdGenerator.TimeKnotSpanPolynomialCoefficients.Select(span => span.GetKnotSpanDeltaT());

                    foreach (var fx in fxs)
                    {
                        fxRef = fx * op.Musp / OpReference.Musp;
                        if (fxRef <= _sfdGenerator.SpaceValues.MaxValue)
                        {
                            var ROfT = ROfFxAndTime(op.AsEnumerable(), fxRef.AsEnumerable(), time);

                            foreach (var ft in fts)
                            {
                                yield return LinearDiscreteFourierTransform.GetFourierTransform(time.ToArray(), ROfT.ToArray(), deltaT.ToArray(), ft * OpReference.Musp / op.Musp);    
                            }
                        }
                        else
                        {
                            foreach (var ft in fts)
                            {
                                yield return new Complex(0.0, 0.0);
                            }
                        }
                    }
                }
            }
        }

        #endregion Spatial Frequency Domain

        #region not implemented
        /// <summary>
        /// Evaluates the radial resolved fluence.
        /// <remarks>Not implemented.</remarks>
        /// </summary>
        /// <param name="ops">optical properties of the medium</param>
        /// <param name="rhos">source-detector separation (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>Fluence(rho,z)</returns>
        public override IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the temporal and radial resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">set of optical properties for the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Fluence(rho,z,t)</returns>
        public override IEnumerable<double> FluenceOfRhoAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the temporal frequency and radial resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">set of optical properties for the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">temporal frequency (GHz)</param>
        /// <returns>Fluence(rho,z,ft)</returns>
        public override IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the spatial frequency resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>Fluence(fx,z)</returns>
        public override IEnumerable<double> FluenceOfFxAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Evaluates the spatial frequency and time resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">set of optical properties for the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Fluence as a function of fxs, zs, and times</returns>
        public override IEnumerable<double> FluenceOfFxAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the spatial frequency and temporal frequency resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">set of optical properties for the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">temporal frequencies (GHz)</param>
        /// <returns>Fluence as a function of fxs, zs and fts</returns>
        public override IEnumerable<Complex> FluenceOfFxAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
        #endregion not implemented

        #endregion IForwardSolver methods

        #region public methods
        /// <summary>
        /// Returns zero if the input value is smaller then zero or if it is <see cref="double.NaN"/>.
        /// Negative value are not possible for the measured reflectance.
        /// The values calculated with the NURBS could be negative when the time
        /// point is very close to the 'physical' beginning of the curve R(t) due
        /// to oscillations of the interpolations used to capture the ascent of the curve.
        /// </summary>
        /// <param name="value">double precision number</param>
        /// <returns>zero or the input value</returns>
        public static double CheckIfValidOutput(double value)
        {
            if (value is < 0.0 or double.NaN)
            {
                value = 0.0;
            }
            return value;
        }
        #endregion

        #region private methods

        private static double[] GetDeltaT(IReadOnlyList<double> t)
        {
            var deltaT = new double[t.Count];
            var T = new double[t.Count + 1];
            T[0] = t[0];
            for (var i = 1; i < T.Length - 1; i++)
            {
                T[i] = t[i - 1] + (t[i] - t[i - 1]) / 2.0;
            }
            T[T.Length - 1] = 2.0 * t[t.Count - 1] - T[T.Length - 2];
            for (var i = 0; i < deltaT.Length; i++)
            {
                deltaT[i] = T[i + 1] - T[i];
            }
            return deltaT;
        }
        
        /// <summary>
        /// Returns the constant scaling factor for the different reflectance domain.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="power">domain dependent scaling factor power</param>
        /// <returns>scaling factor</returns>
        private static double GetScalingFactor(OpticalProperties op, int power)
        {
            return Math.Pow(op.Musp / OpReference.Musp, power);
        }

        /// <summary>
        /// Extrapolates the linear decay of the log of the tail of the curve and integrates
        /// analytically from tMax to infinity to evaluate the steady state signal
        /// </summary>
        /// <param name="generator">NurbsGenerator</param>
        /// <param name="spaceRef">spatial coordinate</param>
        /// <param name="op">optical Properties</param>
        /// <returns>Integral value of the curve extrapolated outside the time range</returns>
        private static double ExtrapolateIntegralValueOutOfRange(INurbs generator, double spaceRef, OpticalProperties op)
        {
            const double deltaT = 0.01; //ns
            GetScalingFactor(op, 3);
            var lR2 = Math.Log10(generator.ComputeSurfacePoint(generator.TimeValues.MaxValue, spaceRef));
            var lR1 = Math.Log10(generator.ComputeSurfacePoint(generator.TimeValues.MaxValue - deltaT, spaceRef));
            var slope = (lR2 - lR1) / (deltaT);
            var intercept = -slope * generator.TimeValues.MaxValue + lR1;
            var area = -Math.Pow(10.0, intercept + slope * generator.TimeValues.MaxValue)
                * Math.Exp(-op.Mua * v * generator.TimeValues.MaxValue) / (slope - op.Mua);
            return area;
        }
        #endregion public methods
    }
}
