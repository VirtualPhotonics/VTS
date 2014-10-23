using System;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    public class DeltaPOneForwardSolver : DiffusionForwardSolverBase
    {
        public DeltaPOneForwardSolver(SourceConfiguration sourceConfiguration, double beamRadius) 
            : base(sourceConfiguration, beamRadius)
        {
            this.ForwardModel = ForwardModel.DeltaPOne;
        }

        public DeltaPOneForwardSolver() : this(SourceConfiguration.Distributed, 0.0) { }

        #region override methods to be refactored soon...
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

        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public override double ROfFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }

        public override double ROfFxAndTime(OpticalProperties op, double fx, double t)
        {
            throw new NotImplementedException();
        }

        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }


        public override IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public override Complex TemporalFrequencyReflectance(DiffusionParameters dp, double rho, Complex k, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }

        public override Complex TemporalFrequencyFluence(DiffusionParameters dp, double rho, double z, Complex k)
        {
            throw new NotImplementedException();
        }
    }
}
