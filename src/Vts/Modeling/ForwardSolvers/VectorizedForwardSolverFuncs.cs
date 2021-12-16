using System;
using System.Linq;
using System.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// vectorized forward solver functions
    /// </summary>
    public class VectorizedForwardSolverFuncs
    {
        private IForwardSolver _fs;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="fs"></param>
        public VectorizedForwardSolverFuncs(IForwardSolver fs)
        {
            _fs = fs;
        }
        /// <summary>
        /// reflectance as a function of s-d separation
        /// </summary>
        public Func<OpticalProperties, double, double> ROfRho
        {
            get
            {
                return (OpticalProperties op, double rho) =>
                    _fs.ROfRho(op.AsEnumerable(), rho.AsEnumerable()).FirstOrDefault();
            }
        }
        /// <summary>
        /// reflectance as a function of s-d separation and time
        /// </summary>
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
        /// <summary>
        /// reflectance as a function of s-d separation and temporal-frequency
        /// </summary>
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
        /// <summary>
        /// reflectance as a function of spatial-frequency
        /// </summary>
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
        /// <summary>
        /// reflectance as a function of spatial-frequency and time
        /// </summary>
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
        /// <summary>
        /// reflectance as a function of spatial-frequency and temporal-frequency
        /// </summary>
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
