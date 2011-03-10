using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Interfaces;
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
                _pMCLoader = new pMCLoader("Vts.Database", "", "photonBiographies1e6");
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
            var detectorInput = new pMCROfRhoDetectorInput();
            var detectorInputs = new List<IpMCDetectorInput> { detectorInput };

            // todo: revisit
            detectorInput.Rho = new DoubleRange(rhos.First(), rhos.Last(), rhos.Count());
            List<int> perturbedRegionsIndices = new List<int>() { 1 }; // assumes homogeneous tissue 
            foreach (var op in ops)
            {
                // make list of ops that have requested ops as middle region (of multilayer tissue)
                List<OpticalProperties> regionOps = new List<OpticalProperties>() { 
                    new OpticalProperties(), op, new OpticalProperties() };
                var _postProcessedOutput =
                    PhotonTerminationDatabasePostProcessor.GenerateOutput(
                        detectorInputs, 
                        pMCLoader.PhotonTerminationDatabase,
                        pMCLoader.databaseOutput, 
                        regionOps, 
                        perturbedRegionsIndices);
                // yield return method won't work here because want to process all rhos and times during one pass of db
                for (int r = 0; r < rhos.Count() - 1; r++)
                {
                    yield return _postProcessedOutput.R_r[r];
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
            var detectorInput = new pMCROfRhoDetectorInput();
            var detectorInputs = new List<IpMCDetectorInput> { detectorInput };

            // todo: revisit!!
            //detectorInput.AWT = AbsorptionWeightingType.Continuous;
            //detectorInput.TallyTypeList = new List<TallyType>() { TallyType.pMuaMusInROfRhoAndTime };
            //// the next two should come from parameter list
            //detectorInput.Rho = new DoubleRange(rhos.First(), rhos.Last(), rhos.Count());
            //detectorInput.Time = new DoubleRange(times.First(), times.Last(), times.Count());


            List<int> perturbedRegionsIndices = new List<int>() { 1 }; // assumes homogeneous tissue 
            foreach (var op in ops)
            {
                // make list of ops that have requested ops as middle region (of multilayer tissue)
                List<OpticalProperties> regionOps = new List<OpticalProperties>() { 
                    new OpticalProperties(), op, new OpticalProperties() };
                var _postProcessedOutput =
                    PhotonTerminationDatabasePostProcessor.GenerateOutput(
                    detectorInputs, pMCLoader.PhotonTerminationDatabase,
                    pMCLoader.databaseOutput, regionOps, perturbedRegionsIndices);
                // yield return method won't work here because want to process all rhos and times during one pass of db
                for (int r = 0; r < rhos.Count(); r++)
                {
                    for (int t = 0; t < times.Count() - 1; t++) // omit last bin which captures all beyond
                    {
                        yield return _postProcessedOutput.R_rt[r, t];
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
