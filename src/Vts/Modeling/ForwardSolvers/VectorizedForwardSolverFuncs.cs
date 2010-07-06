using System;
using System.Linq;
using Vts.Extensions;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    public class VectorizedForwardSolverFuncs
    {
        private IForwardSolver _fs;

        public VectorizedForwardSolverFuncs(IForwardSolver fs)
        {
            _fs = fs;
        }

        public Func<OpticalProperties, double, double> RofRho
        {
            get
            {
                return (OpticalProperties op, double rho) =>
                    _fs.RofRho(op.AsEnumerable(), rho.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, double> RofRhoAndT
        {
            get
            {
                return
                    (OpticalProperties op, double rho, double t) =>
                        _fs.RofRhoAndT(
                            op.AsEnumerable(),
                            rho.AsEnumerable(),
                            t.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, Complex> RofRhoAndFt
        {
            get
            {
                return 
                    (OpticalProperties op, double rho, double ft) =>
                        _fs.RofRhoAndFt(
                            op.AsEnumerable(),
                            rho.AsEnumerable(),
                            ft.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double> RofFx
        {
            get
            {
                return 
                    (OpticalProperties op, double fx) =>
                        _fs.RofFx(
                            op.AsEnumerable(),
                            fx.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, double> RofFxAndT
        {
            get
            {
                return 
                    (OpticalProperties op, double fx, double t) =>
                        _fs.RofFxAndT(
                            op.AsEnumerable(),
                            fx.AsEnumerable(),
                            t.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, Complex> RofFxAndFt
        {
            get
            {
                return
                    (OpticalProperties op, double fx, double ft) =>
                        _fs.RofFxAndFt(
                            op.AsEnumerable(),
                            fx.AsEnumerable(),
                            ft.AsEnumerable()).FirstOrDefault();
            }
        }
    }
}
