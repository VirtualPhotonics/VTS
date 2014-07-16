using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Forward solver based on the Scaled Monte Carlo approach, proposed by Kienle and Patterson,
    /// used to evaluate the reflectance of a semi-infinite homogenous medium with g = 0.8 and n = 1.4.
    /// The reference time and space resolved reflectance, and the reference spatial frequancy and
    /// time resolved reflectance are held in a NurbsGenerator class which computes the interpolation
    /// necessary to evaluate the reflectance in the specific domain.
    /// The interpolation is based on NURBS surfaces theory. The main reference used to implement
    /// this forward solver is 'The NURBS Book' by Las Piegl and Wayne Tiller.
    /// </summary>
    public class NurbsForwardSolver : ForwardSolverBase
    {
        #region fields

        private INurbs _rdGenerator;
        private INurbs _sfdGenerator;

        public static readonly double v =  GlobalConstants.C / 1.4;
        private static readonly OpticalProperties _opReference =
                                                 new OpticalProperties(0.0, 1, 0.8, 1.4);

        #endregion fields

        #region constructor

        /// <summary>
        /// Constructor which creates an istance of NurbsForwardSolver setting
        /// the NurbsGenerators to the values passed as Input.
        /// </summary>
        /// <param name="rdGenerator">real domain NurbsGenerator</param>
        /// <param name="sfdGenerator">spatial frequancy domain generator</param>
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
        /// Constructor used to create an istance of NurbsForwardSolver
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
        /// reflectance is scaled and the isoprametric Nurbs curve is integrated
        /// analitically  over time. To evaluate the integral of the reflectance out of 
        /// the time range it evaluates the linear approximation of the logarithm of
        /// the tail of the curve and integrates it from tMax to infinity.
        /// If rho_ref is out of range the method returns 0.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separation</param>
        /// <returns>space resolved reflectance</returns>
        public override IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos)
        {
            
            double scalingFactor;
            double rho_ref;
            double integralValue;

            foreach (var op in ops)
            {
                scalingFactor = GetScalingFactor(op, 2);

                foreach (var rho in rhos)
                {
                    rho_ref = rho * op.Musp / _opReference.Musp;
                    double exponentialTerm = op.Mua * v * _opReference.Musp / op.Musp;

                    if (rho_ref <= _rdGenerator.SpaceValues.MaxValue && rho_ref >= 0)
                    {
                        integralValue = _rdGenerator.EvaluateNurbsCurveIntegral(rho_ref,exponentialTerm);                   
                        integralValue += ExtrapolateIntegralValueOutOfRange(_rdGenerator, rho_ref,op);
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
        /// <param name="t">time</param>
        /// <returns>spatial and temporal resolved reflectance</returns>
        public override double ROfRhoAndTime(OpticalProperties op, double rho, double t)
        {   
            return ROfRhoAndTime(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).FirstOrDefault();
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
        /// If the required point is outside the radial range a linear extarpolation 
        /// is used, based on the value of R at [0.95*RhoMax, RhoMax].
        /// If the required point is outside both ranges a linear combination of the
        /// two extrapolations is adopted.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separation</param>
        /// <param name="ts">time</param>
        /// <returns>space and time resolved reflectance at rho and t</returns>
        public override IEnumerable<double> ROfRhoAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> ts)
        {
            double scalingFactor;
            double rho_ref;
            double t_ref;
            double scaledValue;

            foreach (var op in ops)
            {
                scalingFactor = GetScalingFactor(op, 3);

                foreach (var rho in rhos)
                {
                    rho_ref = rho * op.Musp / _opReference.Musp;
                    
                    foreach (var t in ts)
                    {
                        t_ref = t * op.Musp / _opReference.Musp;
                        if (t_ref < _rdGenerator.GetMinimumValidTime(rho_ref))
                        {  
                            scaledValue = 0.0;
                        }
                        else
                        {
                            scaledValue = _rdGenerator.ComputeSurfacePoint(t_ref, rho_ref);
                        }

                        if ((rho_ref > _rdGenerator.SpaceValues.MaxValue ||
                               t_ref > _rdGenerator.TimeValues.MaxValue) &&
                               t_ref > _rdGenerator.GetMinimumValidTime(rho_ref))
                        {
                            scaledValue = _rdGenerator.ComputePointOutOfSurface(t_ref, rho_ref,
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
        /// <returns>reflectance intensity</returns>
        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return ROfRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).FirstOrDefault();
        }
        /// <summary>
        ///  Evaluates the temporal frequency and space resolved reflectance at a source 
        ///  detector separation rho for a modulation frequency ft,for the specified 
        ///  optical properties. It calculates the Fourier transform of the NURBS
        ///  curve R(t) at the required source detector separation.
        ///  The used FT is analitycal or discrete according to the boolean value 'analyticIntegration'.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separation</param>
        /// <param name="fts">modulation frequency</param>
        /// <returns>reflectance intensity</returns>
        public override IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            bool analyticIntegration = false;
            double rho_ref;

            foreach (var op in ops)
            {   
                if (analyticIntegration)
                {
                    foreach (var rho in rhos)
                    {
                        double exponentialTerm = op.Mua * v * _opReference.Musp / op.Musp;
                        rho_ref = rho * op.Musp / _opReference.Musp;
                        if (rho_ref <= _rdGenerator.SpaceValues.MaxValue)
                        {
                            foreach (var ft in fts)
                            {
                                yield return GetScalingFactor(op,2) * _rdGenerator.EvaluateNurbsCurveFourierTransform(rho_ref, exponentialTerm, ft * _opReference.Musp / op.Musp);
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
                    //var time = _rdGenerator.NativeTimes;
                    var time = _rdGenerator.NativeTimes.ToArray();
                    for (int i = 0; i < time.Length; i++)
                    {
                        time[i] = time[i] * _opReference.Musp / op.Musp;
                    }
                    var deltaT = GetDeltaT(time);
                    
                    foreach (var rho in rhos)
                    {
                        if (rho * _opReference.Musp / op.Musp <= _rdGenerator.SpaceValues.MaxValue)
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
        /// resolved reflectance for the spatial frequancy fx, for the 
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
        /// Returns the spatial frequancy resolved reflectance at fx applying the scaling on
        /// the reference fx-time resolved reflectance.
        /// Than integrates analitically the isoprametric NURBS curve over time if fx is on the
        /// surface.
        /// If fx is out of range it returns 0.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <returns>spatial frequency resolved reflectance</returns>
        public override IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            double fx_ref;
            double integralValue;
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    fx_ref = fx * _opReference.Musp / op.Musp;
                    double exponentialterm = op.Mua * v * _opReference.Musp / op.Musp;

                    if (fx_ref <= _sfdGenerator.SpaceValues.MaxValue && fx_ref >= 0.0)
                    {
                        integralValue = _sfdGenerator.EvaluateNurbsCurveIntegral(fx_ref, exponentialterm);
                        integralValue = CheckIfValidOutput(integralValue);
                        yield return integralValue;
                    }
                    else
                    {
                        yield return 0.0;
                    }
                }
            }

            //foreach (var op in ops)
            //{
            //    double[] time = _sfdGenerator.NativeTimes;
            //    for (int i = 0; i < time.Length; i++)
            //    {
            //        time[i] = time[i] * _opReference.Musp / op.Musp;
            //    }
            //    var deltaT = GetDeltaT(time);
            //    foreach (var fx in fxs)
            //    {
            //        double integralValue = 0.0;
            //        var ROfT = ROfFxAndTime(op.AsEnumerable(), fx.AsEnumerable(), time).ToArray();
            //        for (int i = 0; i < ROfT.Length; i++)
            //        {
            //            integralValue += ROfT[i] * deltaT[i];
            //        }
            //        yield return integralValue;
            //    }             
            //}
        }

        /// <summary>
        /// Calls its vectorized version to evaluate the time and space resolved reflectance
        /// for a spatial frequancy, fx, and at time, t, for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="t">time</param>
        /// <returns>spatial frequency and time resolved reflectance</returns>
        public override double ROfFxAndTime(OpticalProperties op, double fx, double t)
        {
            return ROfFxAndTime(op.AsEnumerable(), fx.AsEnumerable(), t.AsEnumerable()).FirstOrDefault();
        }
        /// <summary>
        /// Returns the reflectance at spatial frequency, fx, and time, t, scaling the 
        /// reference fx-time resolved reflectance.
        /// If a point of the reference reflectance outside the time/spatial frequancy range 
        /// of the surface is required, the value is extrapolated using the first derivative
        /// along the time/spatial frequency dimension.
        /// If the required point is outside both ranges a linear combination of the
        /// two derivatives is used.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <param name="ts">time</param>
        /// <returns>spatial frequency and time resolved reflectance</returns>
        public override IEnumerable<double> ROfFxAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts)
        {
            double scalingFactor;
            double fx_ref;
            double t_ref;
            double scaledValue;

            foreach (var op in ops)
            {
                scalingFactor = GetScalingFactor(op, 1);

                foreach (var fx in fxs)
                {
                    fx_ref = fx * _opReference.Musp / op.Musp;
                    
                    foreach (var t in ts)
                    {
                        t_ref = t * op.Musp / _opReference.Musp;
                        if (fx_ref > _sfdGenerator.SpaceValues.MaxValue || t_ref > _sfdGenerator.TimeValues.MaxValue)
                        {
                            yield return 0.0;
                        }
                        else
                        {
                            scaledValue = _sfdGenerator.ComputeSurfacePoint(t_ref, fx_ref);

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
        /// <param name="ft">temporal frequancy</param>
        /// <returns>spatial frequency and temporal frequancy resolved reflectance</returns>
        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return ROfFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }
        /// <summary>
        /// Evaluates the spatial frequency and temporal frequency resolved reflectance
        /// calculating the Fourier transform of the NURBS curve R(t) at the
        /// required spatial frequency for the specified optical properties. 
        /// The computed FT is analitycal or discrete according to the boolean value 'analyticIntegration'.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <param name="fts">temporal frequancy</param>
        /// <returns>spatial frequency and temporal frequancy resolved reflectance</returns>
        public override IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            bool analyticIntegration = false;
            double fx_ref;
            Complex transformedValue;

            foreach (var op in ops)
            {
                if (analyticIntegration)
                {
                    foreach (var fx in fxs)
                    {
                        fx_ref = fx * _opReference.Musp / op.Musp;
                        double exponentialterm = op.Mua * v * _opReference.Musp / op.Musp;

                        if (fx_ref <= _sfdGenerator.SpaceValues.MaxValue)
                        {
                            foreach (var ft in fts)
                            {
                                transformedValue = _sfdGenerator.EvaluateNurbsCurveFourierTransform(fx_ref, exponentialterm, ft * _opReference.Musp / op.Musp);
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
                        fx_ref = fx * op.Musp / _opReference.Musp;
                        if (fx_ref <= _sfdGenerator.SpaceValues.MaxValue)
                        {
                            var ROfT = ROfFxAndTime(op.AsEnumerable(), fx_ref.AsEnumerable(), time);

                            foreach (var ft in fts)
                            {
                                yield return LinearDiscreteFourierTransform.GetFourierTransform(time.ToArray(), ROfT.ToArray(), deltaT.ToArray(), ft * _opReference.Musp / op.Musp);    
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
        /// <returns></returns>
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
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceOfRhoAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
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
        /// <returns></returns>
        public override IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the spatial frequancy resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceOfFxAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Evaluates the spatial frequancy and time resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">set of optical properties for the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceOfFxAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the spatial frequancy and temporal frequency resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops">set of optical properties for the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">temporal frequencies (GHz)</param>
        /// <returns></returns>
        public override IEnumerable<Complex> FluenceOfFxAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
        #endregion not implemented

        #endregion IForwardSolver methods

        #region public methods

        private double[] GetDeltaT(double[] t)
        {
            double[] deltaT = new double[t.Length];
            double[] T = new double[t.Length + 1];
            T[0] = t[0];
            for (int i = 1; i < T.Length - 1; i++)
            {
                T[i] = t[i - 1] + (t[i] - t[i - 1]) / 2.0;
            }
            T[T.Length - 1] = 2.0 * t[t.Length - 1] - T[T.Length - 2];
            for (int i = 0; i < deltaT.Length; i++)
            {
                deltaT[i] = T[i + 1] - T[i];
            }
            return deltaT;
        }

        /// <summary>
        /// Returns zero if the input value is smaller then zero of if it is NaN.
        /// Negative value are not possible for the measured reflectance.
        /// The values calculated with the NURBS could be negative when the time
        /// point is very close to the 'physical' beginning of the curve R(t) due
        /// to obscilatoions of the interpolations used to capture the ascent of the curve.
        /// </summary>
        /// <param name="value">double precision number</param>
        /// <returns>zero or the input value</returns>
        public double CheckIfValidOutput(double value)
        {
            if (value < 0.0 || Double.IsNaN(value))
            {
                value = 0.0;
            }
            return value;
        }
        
        /// <summary>
        /// Returns the constant scaling factor for the different reflectance domain.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="power">domain dependent scaling factor power</param>
        /// <returns>scaling factor</returns>
        private double GetScalingFactor(OpticalProperties op, int power)
        {
            return Math.Pow(op.Musp / _opReference.Musp, power);
        }

        /// <summary>
        /// Extrapolates the linear decay of the log of the tail of the curve and integrates
        /// analitically from tMax to infinity to evaluate the steady state signal
        /// </summary>
        /// <param name="generator">NurbsGenerator</param>
        /// <param name="space_ref">spatial coordiante</param>
        /// <param name="op">optical Properties</param>
        /// <returns>Integral value of the curve extrapolated outside the time range</returns>
        private double ExtrapolateIntegralValueOutOfRange(INurbs generator, double space_ref, OpticalProperties op)
        {
            double area;
            double deltaT = 0.01;//ns
            double scalingFactor = GetScalingFactor(op, 3);
            double lR2 = Math.Log10(generator.ComputeSurfacePoint(generator.TimeValues.MaxValue, space_ref));
            double lR1 = Math.Log10(generator.ComputeSurfacePoint(generator.TimeValues.MaxValue - deltaT, space_ref));
            double slope = (lR2 - lR1) / (deltaT);
            double intercept = -slope * generator.TimeValues.MaxValue + lR1;
            area = -Math.Pow(10.0, intercept + slope * generator.TimeValues.MaxValue)
                   * Math.Exp(-op.Mua * v * generator.TimeValues.MaxValue) / (slope - op.Mua);
            return area;
        }
        
        #endregion public methods
    }
}
