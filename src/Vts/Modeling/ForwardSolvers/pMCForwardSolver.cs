using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PostProcessing;

namespace Vts.Modeling.ForwardSolvers
{
    // this assumes pMC of homogeneous MultiLayer tissue
    public class pMCForwardSolver : ForwardSolverBase
    {
        private static pMCLoader _pMCLoader;
        private Output _postProcessedOutput;

        public pMCForwardSolver()
            : base(SourceConfiguration.Point, 0.0)
        {
            if (_pMCLoader == null)
            {
                // todo: create new databases with these names
                _pMCLoader = new pMCLoader("Vts.Database", "", "DiffuseReflectanceDatabase", 
                    "CollisionInfoDatabase");
            }
        }

        #region IForwardSolver Members

        #region Spatial Domain Solutions
        public override double RofRho(OpticalProperties op, double rho)
        {
            return RofRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofRho(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos)
        {
            // determine rho bins with centers at IEnumerable rhos
            var rhoDelta = (rhos.Last() - rhos.First()) / rhos.Count();
            var rhoBins = new DoubleRange(rhos.First() - rhoDelta / 2, rhos.Last() + rhoDelta / 2, rhos.Count() + 2);
            foreach (var op in ops)
            {
                var detectorInputs = new List<IpMCDetectorInput> 
                {
                    new pMCROfRhoDetectorInput(
                        //new DoubleRange(rhos.First(), rhos.Last(), rhos.Count()),
                        rhoBins,
                        // make list of ops that have requested ops as middle region (of multilayer tissue)
                        new List<OpticalProperties>() { 
                            new OpticalProperties(), op, new OpticalProperties() },
                        new List<int>() { 1 },
                        TallyType.pMCROfRho.ToString())
                };
                var _postProcessedOutput =
                    PhotonDatabasePostProcessor.GenerateOutput(
                        VirtualBoundaryType.pMCDiffuseReflectance,
                        detectorInputs,
                        false,
                        pMCLoader.PMCDatabase,
                        pMCLoader.DatabaseInput);
                for (int r = 0; r < rhos.Count(); r++)
                {
                    yield return _postProcessedOutput.pMC_R_r[r];
                }
                //yield return (IEnumerable<double>)_postProcessedOutput.R_rt.ToEnumerable();
            }
        }

        public override double RofRhoAndT(OpticalProperties op, double rho, double t)
        {
            return RofRhoAndT(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofRhoAndT(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos, IEnumerable<double> times)
        {
            // need to fix:  results may be bad because detector expects continguous rho,time bins but 
            // depending on the rho,time selections, bins might be too big.

            // determine rho,time bins with centers at IEnumerable rhos,times
            var rhoDelta = (rhos.Last() - rhos.First()) / rhos.Count();
            var timeDelta = (times.Last() - times.First()) / times.Count();
            var rhoBins = new DoubleRange(rhos.First() - rhoDelta / 2, rhos.Last() + rhoDelta / 2, rhos.Count() + 2);
            var timeBins = new DoubleRange(times.First() - timeDelta / 2, times.Last() + timeDelta / 2, times.Count() + 2);
            // check if only one rho or time bin
            if (rhos.Count() == 1)
            {
                rhoBins = new DoubleRange(rhos.First() - 0.1, rhos.First() + 0.1, 2);
            }
            if (times.Count() == 1)
            {
                timeBins = new DoubleRange(times.First() - 0.0025, times.First() + 0.0025, 2);
            }
            // check if rhos.First == 0 or times.First == 0, not sure what to do here
            if (rhos.First() == 0.0)
            {

            }
            if (times.First() == 0.0)
            {
            }

            foreach (var op in ops)
            {
                var detectorInputs = new List<IpMCDetectorInput> 
                { 
                    new pMCROfRhoAndTimeDetectorInput(  
                        //new DoubleRange(rhos.First(), rhos.Last(), rhos.Count()),
                        //new DoubleRange(times.First(), times.Last(), times.Count()),
                        rhoBins,
                        timeBins,
                        // make list of ops that have requested ops as middle region (of multilayer tissue)
                        new List<OpticalProperties>() { 
                             new OpticalProperties(), op, new OpticalProperties() },
                        new List<int>() { 1 } // assumes homogeneous tissue
                     )                       
                };
                var _postProcessedOutput =
                    PhotonDatabasePostProcessor.GenerateOutput(
                        VirtualBoundaryType.pMCDiffuseReflectance,
                        detectorInputs, 
                        false,
                        pMCLoader.PMCDatabase,
                        pMCLoader.DatabaseInput);
                for (int r = 0; r < rhos.Count(); r++)
                {
                    for (int t = 0; t < times.Count(); t++) // omit last bin which captures all beyond
                    {
                        yield return _postProcessedOutput.pMC_R_rt[r, t];
                    }

                }
                //yield return (IEnumerable<double>)_postProcessedOutput.R_rt.ToEnumerable();
            }
        }

        /// <summary>
        /// Call a method to compute a discrete Fourier Transform on R(rho,t) MC results.
        /// </summary>
        /// <param name="op">Optical Properties</param>
        /// <param name="rho">rho</param>
        /// <param name="ft">time frequency</param>
        /// <returns></returns>
        public override Complex RofRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return RofRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        }
        public override IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {
                foreach (var rho in rhos)
                {
                    foreach (var ft in fts)
                    {
                        yield return 1;
                    }
                }
            }
        }
        #endregion

        #region Spatial Frequency Domain Solutions
        public override double RofFx(OpticalProperties op, double fx)
        {
            return RofFx(op.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> RofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    yield return 1;
                }
            }
        }

        public override double RofFxAndT(OpticalProperties op, double fx, double t)
        {
            return RofFxAndT(op.AsEnumerable(), fx.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> RofFxAndT(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs, IEnumerable<double> times)
        {
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    foreach (var time in times)
                    {
                        yield return 1;
                    }
                }
            }
        }

        public override Complex RofFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return RofFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }
        public override IEnumerable<Complex> RofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            foreach (var op in ops)
            {
                foreach (var fx in fxs)
                {
                    foreach (var ft in fts)
                    {
                        yield return 1;
                    }
                }
            }
        }

        #endregion

        #endregion


        public override IEnumerable<double> FluenceofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
    }
}
