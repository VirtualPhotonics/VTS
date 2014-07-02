using System;
using System.Linq;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    public class VectorizedForwardSolverFuncs
    {
        private IForwardSolver _fs;

        public VectorizedForwardSolverFuncs(IForwardSolver fs)
        {
            _fs = fs;
        }

        public Func<OpticalProperties, double, double> ROfRho
        {
            get
            {
                return (OpticalProperties op, double rho) =>
                    _fs.ROfRho(op.AsEnumerable(), rho.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, double> ROfRhoAndT
        {
            get
            {
                return
                    (OpticalProperties op, double rho, double t) =>
                        _fs.ROfRhoAndTime(
                            op.AsEnumerable(),
                            rho.AsEnumerable(),
                            t.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, Complex> ROfRhoAndFt
        {
            get
            {
                return 
                    (OpticalProperties op, double rho, double ft) =>
                        _fs.ROfRhoAndFt(
                            op.AsEnumerable(),
                            rho.AsEnumerable(),
                            ft.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double> ROfFx
        {
            get
            {
                return 
                    (OpticalProperties op, double fx) =>
                        _fs.ROfFx(
                            op.AsEnumerable(),
                            fx.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, double> ROfFxAndT
        {
            get
            {
                return 
                    (OpticalProperties op, double fx, double t) =>
                        _fs.ROfFxAndTime(
                            op.AsEnumerable(),
                            fx.AsEnumerable(),
                            t.AsEnumerable()).FirstOrDefault();
            }
        }

        public Func<OpticalProperties, double, double, Complex> ROfFxAndFt
        {
            get
            {
                return
                    (OpticalProperties op, double fx, double ft) =>
                        _fs.ROfFxAndFt(
                            op.AsEnumerable(),
                            fx.AsEnumerable(),
                            ft.AsEnumerable()).FirstOrDefault();
            }
        }
    }
}
