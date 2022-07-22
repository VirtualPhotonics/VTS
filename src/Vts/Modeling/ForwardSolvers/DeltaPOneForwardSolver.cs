using System;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// delta-P1 forward solver class
    /// </summary>
    public class DeltaPOneForwardSolver : DiffusionForwardSolverBase
    {
        /// <summary>
        /// delta-P1 forward solver solution
        /// </summary>
        /// <param name="sourceConfiguration">flat or Gaussian</param>
        /// <param name="beamRadius">radius of source</param>
        public DeltaPOneForwardSolver(SourceConfiguration sourceConfiguration, double beamRadius) 
            : base(sourceConfiguration, beamRadius)
        {
            this.ForwardModel = ForwardModel.DeltaPOne;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public DeltaPOneForwardSolver() : this(SourceConfiguration.Distributed, 0.0) { }

        #region override methods to be refactored soon...
        /// <summary>
        /// stationary (time-independent) reflectance
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="fr1">First Fresnel Reflection Moment</param>
        /// <param name="fr2">Second Fresnel Reflection Moment</param>
        /// <returns>reflectance at rho</returns>
        public override double StationaryReflectance(DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            return 0;
        }
        /// <summary>
        /// stationary (time-independent) fluence
        /// </summary>
        /// <param name="rho">s-d separation</param>
        /// <param name="z">depth</param>
        /// <param name="dp">diffusion parameters</param>
        /// <returns>fluence value at (rho,z)</returns>
        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            return 0;
        }
        /// <summary>
        /// temporally-resolved reflectance
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="t">time</param>
        /// <param name="fr1">First Fresnel Reflection Moment</param>
        /// <param name="fr2">Second Fresnel Reflection Moment</param>
        /// <returns></returns>
        public override double TemporalReflectance(DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            return 0;
        }
        /// <summary>
        /// temporally-resolved fluence
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="z">depth</param>
        /// <param name="t">time</param>
        /// <returns>fluence at (rho,z,t)</returns>
        public override double TemporalFluence(DiffusionParameters dp, double rho, double z, double t)
        {
            return 0;
        }
        #endregion
        /// <summary>
        /// reflectance as a function of s-d separation and temporal-frequency
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>R(rho,ft)</returns>
        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// reflectance as a function of spatial-frequency
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <returns>R(fx)</returns>
        public override double ROfFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// reflectance as a function of spatial-frequency and time
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="time">time</param>
        /// <returns>R(fx,t)</returns>
        public override double ROfFxAndTime(OpticalProperties op, double fx, double time)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// reflectance as a function of spatial-frequency and temporal-frequency
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>R(fx,ft)</returns>
        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// fluence as a function of s-d separation and z and temporal-frequency
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">s-d separations</param>
        /// <param name="zs">depths</param>
        /// <param name="fts">temporal-frequencies</param>
        /// <returns>Fluence(rho,z,ft)</returns>

        public override IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// reflectance as a function of s-d separation
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="k">square root of (mua c+i omega)/(Dc)</param>
        /// <param name="fr1">First Fresnel Reflection Moment</param>
        /// <param name="fr2">Second Fresnel Reflection Moment</param>
        /// <returns>Fluence as a function of rho,z and ft</returns>
        public override Complex TemporalFrequencyReflectance(DiffusionParameters dp, double rho, Complex k, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// fluence as a function of temporal-frequency
        /// </summary>
        /// <param name="dp">diffuse parameters</param>
        /// <param name="rho">s-d separation</param>
        /// <param name="z">depth</param>
        /// <param name="k">square root of (mua c+i omega)/(Dc)</param>
        /// <returns></returns>
        public override Complex TemporalFrequencyFluence(DiffusionParameters dp, double rho, double z, Complex k)
        {
            throw new NotImplementedException();
        }
    }
}
