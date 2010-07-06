using System;
using System.Collections.Generic;

namespace Vts.Modeling.ForwardSolvers
{
    public class TestNewFunctionality : ISource, IBoundaryCondition
    {

        #region IBoundaryCondition Members

        public Func<IEnumerable<double>, IEnumerable<double>> SetBoundaryCondition(BoundaryConditionType bct)
        {
            //if (bct == BoundaryConditionType.Extrapolated)
            //{
            //    Func<IEnumerable<double>,IEnumerable<double>> analyticFunc => (

            //    return new Func<IEnumerable<double>,IEnumerable<double>> 
            //}

            throw new NotImplementedException();

        }
        #endregion IBoundaryCondition Members

        #region ISource Members

        public Func<DiffusionParameters, IList<double>, double> SetSourceFunction(SourceType st,
            Func<DiffusionParameters, IList<double>, double> PointDistributionFunction)
        {
            return (DiffusionParameters dp, IList<double> paramList) =>
                //double rho, double z) =>
                PointDistributionFunction(dp, paramList) *
                Math.Exp(-dp.mutTilde * dp.zp);

        }

        public Func<DiffusionParameters, double, double, double> SetSourceFunction(SourceType st,
           Func<DiffusionParameters, double, double, double> PointDistributionFunction)
        {


            {
                return (DiffusionParameters dp, double rho, double z) =>
                    PointDistributionFunction(dp, rho, z) *
                    Math.Exp(-dp.mutTilde * dp.zp);
            }

            //Func<double, double> distributionFunction = zPrime =>
            //    {
            //        switch (st)
            //        {
            //            default:                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            //            case SourceType.Point:
            //                return 1.0;
            //            case SourceType.DistributedLine:
            //                return Math.Exp(-dps.mutTilde * zPrime);
            //            case SourceType.DistributedGaussian:
            //                return 1.0;
            //            case SourceType.Gaussian:
            //                return 1.0;
            //        }
            //    };
            //throw new NotImplementedException();
        }
        #endregion ISource Members




        #region ISource Members

        public Func<double, double> SetSourceFunction(SourceType st, DiffusionParameters dps)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
