using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests are for the Dynamic detectors.  They were separated out from
    /// the other DAW Layer tests because the "Dynamic" detectors use a random number and
    /// change the sequence of random numbers during a simulation causing the other detector
    /// results to not match linux anymore.
    /// These tests execute a discrete absorption weighting (DAW) MC simulation with 
    /// 100 photons and verify that the tally results match those of prior tests. 
    /// The tests then run a simulation
    /// through a homogeneous two layer tissue (both layers have the same optical properties)
    /// and verify that the detector tallies are the same.  This tests whether the pseudo-
    /// collision pausing at the layer interface does not change the results.
    /// </summary>
    [TestFixture]
    public class DAWLayersDynamicFxDetectorsTests
    {
        private SimulationOutput _outputOneLayerTissue;
        private SimulationOutput _outputTwoLayerTissue;
        private SimulationInput _inputOneLayerTissue;
        private SimulationInput _inputTwoLayerTissue;
        private double _layerThickness = 0.1; // tissue is homogeneous (both layer opt. props same)

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTesGeneratedtFiles = new List<string>()
        {
            "file.txt", // file that captures screen output of MC simulation
        };

        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTesGeneratedtFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a homogeneous
        /// two layer tissue (both layers have same optical properties), execute simulations
        /// and verify results agree with prior results.
        /// NOTE: currently two region executes same photon biography except for pauses
        /// at layer interface.  Variance for DAW results not degraded.
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>() { }, // databases to be written
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            var source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1); // start inside tissue
            var oneLayerDetectors =  new List<IDetectorInput>
                {
                    new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 11), TallySecondMoment = true},
                    new TOfFxDetectorInput() {Fx=new DoubleRange(0.0, 0.5, 11)}, 
                    new ReflectedDynamicMTOfFxAndSubregionHistDetectorInput() 
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11), // fx bins MAKE SURE AGREES with ROfFx fx specification for unit test below
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0}
                    },
                    new TransmittedDynamicMTOfFxAndSubregionHistDetectorInput() 
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11), // fx bins MAKE SURE AGREES with TOfFx fx specification for unit test below
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 }
                    },
                };

            _inputOneLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                oneLayerDetectors);             
            _outputOneLayerTissue = new MonteCarloSimulation(_inputOneLayerTissue).Run();

            var twoLayerDetectors = new List<IDetectorInput>
                {
                    new ReflectedDynamicMTOfFxAndSubregionHistDetectorInput() 
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}
                    },
                    new TransmittedDynamicMTOfFxAndSubregionHistDetectorInput() 
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),                        
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}
                    }
                };
            _inputTwoLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, _layerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(_layerThickness, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                twoLayerDetectors);
            _outputTwoLayerTissue = new MonteCarloSimulation(_inputTwoLayerTissue).Run();
        }
       
        // Reflected Dynamic Momentum Transfer of Fx and SubRegion
        [Test]
        public void validate_DAW_ReflectedDynamicMTOfFxAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_fxmt[0, 0].Magnitude - 0.239141), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_fxmt[0, 1].Magnitude - 0.101220), 0.000001);
            // verify mean integral over MT equals R(Fx) results
            var mtbins = ((ReflectedDynamicMTOfFxAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefDynMT_fxmt[0, i].Magnitude;
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx[0].Magnitude - integral), 0.000001);
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(fx,mt)
            var fxs = ((ReflectedDynamicMTOfFxAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").First()).Fx;
            var fracMTbins =
                ((ReflectedDynamicMTOfFxAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                    Where(d => d.TallyType == "ReflectedDynamicMTOfFxAndSubregionHist").First()).FractionalMTBins;
            for (int i = 0; i < fxs.Count; i++)
            {
                for (int j = 0; j < mtbins.Count - 1; j++)
                {
                    integral = 0.0;
                    for (int m = 0; m < fracMTbins.Count + 1; m++)
                    {
                        integral += _outputOneLayerTissue.RefDynMT_fxmt_frac[i, j, m].Magnitude;
                    }
                    Assert.Less(Math.Abs(integral - _outputOneLayerTissue.RefDynMT_fxmt[i, j].Magnitude), 0.001);
                }
            }
            // validate a few fractional values - indices Fx, mtbins, fraction
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_fxmt_frac[0, 0, 1].Magnitude - 0.0098), 0.0001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_fxmt_frac[0, 0, 2].Magnitude - 0.0094), 0.0001);
            // validate 2 layer tissue results 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_fxmt_frac[0, 0, 1].Magnitude - 0.0098), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_fxmt_frac[0, 0, 2].Magnitude - 0.0094), 0.0001);
            // validate dynamic results
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_fxmt_dynofz[0, 1].Magnitude - 0.9033), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_fxmt_dynofz[0, 1].Magnitude - 0.9033), 0.0001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_fxmt_dynofz[2, 8].Magnitude - 0.3497), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_fxmt_dynofz[2, 8].Magnitude - 0.3497), 0.0001);
            // validate SubregionCollision static, dynamic count for one and two layer tissue
            Assert.AreEqual(_outputOneLayerTissue.RefDynMT_fxmt_subrcols[1, 0], 16883);
            Assert.AreEqual(_outputOneLayerTissue.RefDynMT_fxmt_subrcols[1, 1], 16572);
            Assert.AreEqual(_outputTwoLayerTissue.RefDynMT_fxmt_subrcols[1, 0], 274);
            Assert.AreEqual(_outputTwoLayerTissue.RefDynMT_fxmt_subrcols[1, 1], 62);
            Assert.AreEqual(_outputTwoLayerTissue.RefDynMT_fxmt_subrcols[2, 0], 16707);
            Assert.AreEqual(_outputTwoLayerTissue.RefDynMT_fxmt_subrcols[2, 1], 16412);
            // verify one layer totals equal two layer totals
            // note: the two layer static (or dynamic) sum will not equal the one layer static (or dynamic)
            // because of the random number call to determine which collisions are static vs dynamic
            Assert.AreEqual(_outputOneLayerTissue.RefDynMT_fxmt_subrcols[1,0]+
                            _outputOneLayerTissue.RefDynMT_fxmt_subrcols[1,1],
                            _outputTwoLayerTissue.RefDynMT_fxmt_subrcols[1,0]+
                            _outputTwoLayerTissue.RefDynMT_fxmt_subrcols[1,1]+
                            _outputTwoLayerTissue.RefDynMT_fxmt_subrcols[2,0]+
                            _outputTwoLayerTissue.RefDynMT_fxmt_subrcols[2,1]);
        }

        // Transmitted Momentum Transfer of Fx and SubRegion
        [Test]
        public void validate_DAW_TransmittedDynamicMTOfFxAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransDynMT_fxmt[0, 9].Magnitude - 0.003957), 0.000001);
            // make sure mean integral over MT equals T(Fx) results
            var mtbins = ((TransmittedDynamicMTOfFxAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TransmittedDynamicMTOfFxAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransDynMT_fxmt[0, i].Magnitude;
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_fx[0].Magnitude - integral), 0.000001);
        }

    }
}
