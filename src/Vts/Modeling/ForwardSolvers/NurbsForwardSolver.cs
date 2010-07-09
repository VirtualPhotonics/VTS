using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Vts.Common;
using Vts.Extensions;
using Vts.IO;

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
        private readonly double _v;
        private readonly OpticalProperties _opReference;
        private INurbs _rdGenerator;
        private INurbs _sfdGenerator;

        /// <summary>
        /// Initializes a new instance of the NurbsForwardSolver class, setting
        /// the NurbsGenerators to the values passed as Input.
        /// </summary>
        /// <param name="rdGenerator">real domain NurbsGenerator</param>
        /// <param name="sfdGenerator">spatial frequancy domain generator</param>
        public NurbsForwardSolver(INurbs rdGenerator, INurbs sfdGenerator)                                                               
        {
            _rdGenerator = rdGenerator;
            _sfdGenerator = sfdGenerator;
            _v = GlobalConstants.C / 1.4;
            _opReference = new OpticalProperties(0.0, 1, 0.8, 1.4);
        }

        /// <summary>
        /// Default class constructor called by solver factory.
        /// </summary>
        public NurbsForwardSolver()
            : this(
                new NurbsGenerator(NurbsGeneratorType.RealDomain),
                new NurbsGenerator(NurbsGeneratorType.SpatialFrequencyDomain))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the NurbsForwardSolver class, 
        /// with the same stub NurbsGenerator for all the NurbsGenerators.
        /// Used for Unit Tests of the class.
        /// </summary>
        /// <param name="generator">stub NurbsGenerator</param>
        public NurbsForwardSolver(INurbs generator)
            : this(generator, generator)
        {
        }

        #region IForwardSolver methods

        #region Real Domain

        /// <summary>
        /// Calls its vectorized version to evaluate the steady state reflectance at 
        /// a source detector separation rho, for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">source detector separation</param>
        /// <returns>space resolved reflectance</returns>
        public override double RofRho(OpticalProperties op, double rho)
        {
            return RofRho(op.AsEnumerable(), rho.AsEnumerable()).FirstOrDefault();
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
        public override IEnumerable<double> RofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos)
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
                    double exponentialTerm = op.Mua * _v * _opReference.Musp / op.Musp;

                    if (rho_ref <= _rdGenerator.SpaceValues.MaxValue)
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
        public override double RofRhoAndT(OpticalProperties op, double rho, double t)
        {   
            return RofRhoAndT(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).FirstOrDefault();
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
        public override IEnumerable<double> RofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> ts)
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
                            //TODO: vectorial
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
                        
                        yield return scalingFactor * scaledValue * Math.Exp(-op.Mua * _v * t);
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
        public override Complex RofRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return RofRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).FirstOrDefault();
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
        public override IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            double scalingFactor;
            double rho_ref;
            Complex transformedValue;

            foreach (var op in ops)
            {
                scalingFactor = GetScalingFactor(op, 2);

                foreach (var rho in rhos)
                {
                    rho_ref = rho * op.Musp / _opReference.Musp;
                    double exponentialTerm = op.Mua * _v * _opReference.Musp / op.Musp;

                    if (rho_ref <= _rdGenerator.SpaceValues.MaxValue)
                    {
                        foreach (var ft in fts)//lui va scalato?..direi di no perche si tratta di trasformare a posizione scalata.
                        {
                            transformedValue = _rdGenerator.EvaluateNurbsCurveFourierTransform(rho_ref, exponentialTerm, ft * _opReference.Musp / op.Musp);
                            yield return transformedValue * scalingFactor;
                        }
                    }
                    else
                    {
                        foreach (var ft in fts)
                        {
                            yield return new Complex(0.0,0.0); 
                        }
                    }
                }
            }
            //foreach (var op in ops)
            //{
            //    double maxT = _rdGenerator.TimeValues.MaxValue * _opReference.Musp / op.Musp;
            //    double maxFt = fts.Last();
            //    double nPoints = maxT * 2.0 * maxFt;
            //    var time =  new DoubleRange(0.0, maxT, (Int16)nPoints).AsEnumerable();
            //    var dTime = time.ElementAt(1) - time.ElementAt(0);
            //    foreach (var rho in rhos)
            //    {
            //        double[] rOfTime = RofRhoAndT(op.AsEnumerable(), rho.AsEnumerable(), time).ToArray();
            //        foreach (var ft in fts)
            //        {
            //            yield return LinearDiscreteFourierTransform.GetFourierTransform(time.ToArray(), rOfTime, dTime, ft);
            //        }
            //    }
            //}
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
        public override double RofFx(OpticalProperties op, double fx)
        {
            return RofFx(op.AsEnumerable(), fx.AsEnumerable()).FirstOrDefault();
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
        public override IEnumerable<double> RofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {   
            double fx_ref;
            double integralValue;

            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    fx_ref = fx * _opReference.Musp / op.Musp;
                    double exponentialterm = op.Mua * _v * _opReference.Musp / op.Musp;

                    if (fx_ref <= _sfdGenerator.SpaceValues.MaxValue)
                    {
                        integralValue = _sfdGenerator.EvaluateNurbsCurveIntegral(fx_ref,exponentialterm);
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
        /// for a spatial frequancy, fx, and at time, t, for the specified optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="t">time</param>
        /// <returns>spatial frequency and time resolved reflectance</returns>
        public override double RofFxAndT(OpticalProperties op, double fx, double t)
        {
            return RofFxAndT(op.AsEnumerable(), fx.AsEnumerable(), t.AsEnumerable()).FirstOrDefault();
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
        public override IEnumerable<double> RofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts)
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

                            yield return scalingFactor * scaledValue * Math.Exp(-op.Mua * _v * t);
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
        public override Complex RofFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return RofFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }
        /// <summary>
        /// Evaluates the spatial frequency and temporal frequency resolved reflectance
        /// calculating the analitycal Fourier transform of the NURBS curve R(t) at the
        /// required spatial frequency for the specified optical properties. 
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="fxs">spatial frequency</param>
        /// <param name="fts">temporal frequancy</param>
        /// <returns>spatial frequency and temporal frequancy resolved reflectance</returns>
        public override IEnumerable<Complex> RofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            double fx_ref;
            Complex transformedValue;

            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    fx_ref = fx * _opReference.Musp / op.Musp;
                    double exponentialterm = op.Mua * _v * _opReference.Musp / op.Musp;

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
            //foreach (var op in ops)
            //{
            //    double maxT = _sfdGenerator.TimeValues.MaxValue * _opReference.Musp / op.Musp;
            //    double maxFt = fts.Last();
            //    double nPoints = maxT * 2.0 * maxFt;
            //    var time = new DoubleRange(0.0, maxT, (Int16)nPoints).AsEnumerable();
            //    var dTime = time.ElementAt(1) - time.ElementAt(0);
            //    foreach (var fx in fxs)
            //    {
            //        double[] rOfTime = RofFxAndT(op.AsEnumerable(), fx.AsEnumerable(), time).ToArray();
            //        foreach (var ft in fts)
            //        {
            //            yield return LinearDiscreteFourierTransform.GetFourierTransform(time.ToArray(), rOfTime, dTime, ft);
            //        }
            //    }
            //}
        }

        #endregion Spatial Frequency Domain

        #region not implemented
        /// <summary>
        /// Evaluates the radial resolved fluence.
        /// <remarks>Not implemented.</remarks>
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="rhos"></param>
        /// <param name="zs"></param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the temporal and radial resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops"></param>
        /// <param name="rhos"></param>
        /// <param name="zs"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the temporal frequency and radial resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops"></param>
        /// <param name="rhos"></param>
        /// <param name="zs"></param>
        /// <param name="fts"></param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the spatial frequancy resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops"></param>
        /// <param name="fxs"></param>
        /// <param name="zs"></param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Evaluates the spatial frequancy and time resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops"></param>
        /// <param name="fxs"></param>
        /// <param name="zs"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the spatial frequancy and temporal frequency resolved fluence.
        /// </summary>
        /// <remarks>Not implemented.</remarks>
        /// <param name="ops"></param>
        /// <param name="fxs"></param>
        /// <param name="zs"></param>
        /// <param name="fts"></param>
        /// <returns></returns>
        public override IEnumerable<double> FluenceofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
        #endregion not implemented

        #endregion IForwardSolver methods

        /// <summary>
        /// Returns zero if the input value is smaller then zero of if it is NaN.
        /// Negative value are not possible for the measured reflectance.
        /// The values calculated with the NURBS could be negative when the time
        /// point is very close to the 'physical' beginning of the curve R(t) due
        /// to obscilatoions of the interpolations used to capture the ascent of the curve.
        /// </summary>
        /// <param name="value">double precision number</param>
        /// <returns>zero or the input value</returns>
        private double CheckIfValidOutput(double value)
        {
            //TODO throw exception if it happens.
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
        /// Extrapolates the linear fit of the log of the tail of the curve and integrates
        /// analitically from tMax to infinity.
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
                   * Math.Exp(-op.Mua * _v * generator.TimeValues.MaxValue) / (slope - op.Mua);
            return area;
        }
    }
}
