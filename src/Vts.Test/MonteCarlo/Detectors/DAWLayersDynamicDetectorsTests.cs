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
    public class DAWLayersDynamicDetectorsTests
    {
        private SimulationOutput _outputOneLayerTissue;
        private SimulationOutput _outputTwoLayerTissue;
        private SimulationInput _inputOneLayerTissue;
        private SimulationInput _inputTwoLayerTissue;
        private readonly double _layerThickness = 0.1; // tissue is homogeneous (both layer opt. props same)

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTesGeneratedtFiles = new List<string>()
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
                    new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 21), TallySecondMoment = true},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 21)}, 
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 21), Y = new DoubleRange(-10.0, 10.0, 21) },
                    new TOfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 21), Y = new DoubleRange(-10.0, 10.0, 21) },
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        TallySecondMoment = true
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with TOfRho rho specification for unit test below
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 },
                        TallySecondMoment = true
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        TallySecondMoment = true
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        TallySecondMoment = true
                    }
                };

            var ti = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 20.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());

            _inputOneLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                ti,
                oneLayerDetectors);             
            _outputOneLayerTissue = new MonteCarloSimulation(_inputOneLayerTissue).Run();

            var twoLayerDetectors = new List<IDetectorInput>
                {
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with TOfRho rho specification for unit test below
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),                        
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21),
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),              
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0},
                        TallySecondMoment = true,
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21),
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),              
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}   
                    }
                };
            var ti2 = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, _layerThickness),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(_layerThickness, 20.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                }
                );
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
    
            _inputTwoLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                ti2,
                twoLayerDetectors);
            _outputTwoLayerTissue = new MonteCarloSimulation(_inputTwoLayerTissue).Run();
        }
       
        // Reflected Dynamic Momentum Transfer of Rho and SubRegion
        [Test]
        public void validate_DAW_ReflectedDynamicMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt[0, 0] - 0.100174), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt[0, 1] - 0.011499), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt2[0, 0] - 0.125508), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt2[0, 1] - 0.013222), Is.LessThan(0.000001));
            // verify mean integral over MT equals R(rho) results
            var mtbins = ((ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefDynMT_rmt[0, i];
            }
            Assert.That(Math.Abs(_outputOneLayerTissue.R_r[0] - integral), Is.LessThan(0.000001));
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var rhobins = ((ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist")).Rho;
            var fracMTbins =
                ((ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.First(
                    d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist")).FractionalMTBins;
            for (int i = 0; i < rhobins.Count - 1; i++)
            {
                for (int j = 0; j < mtbins.Count - 1; j++)
                {
                    integral = 0.0;
                    for (int m = 0; m < fracMTbins.Count + 1; m++)
                    {
                        integral += _outputOneLayerTissue.RefDynMT_rmt_frac[i, j, m];
                    }
                    Assert.That(Math.Abs(integral - _outputOneLayerTissue.RefDynMT_rmt[i, j]), Is.LessThan(0.001));
                }
            }
            // validate a few fractional values - indices rho, mtbins, fraction
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_frac[0, 0, 0] - 0.0127), Is.LessThan(0.0001));
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_frac[0, 0, 1] - 0.0378), Is.LessThan(0.0001));
            // validate 2 layer tissue results 
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_frac[0, 0, 0] - 0.0127), Is.LessThan(0.0001));
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_frac[0, 0, 1] - 0.0378), Is.LessThan(0.0001));
            // validate dynamic results
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_dynofz[0, 1] - 0.0814), Is.LessThan(0.0001));
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_dynofz[0, 1] - 0.0814), Is.LessThan(0.0001));
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_dynofz[2, 8] - 0.000001243), Is.LessThan(0.000000001));
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_dynofz[2, 8] - 0.000001243), Is.LessThan(0.000000001));
            // validate SubregionCollision static, dynamic count for one and two layer tissue
            Assert.That(_outputOneLayerTissue.RefDynMT_rmt_subrcols[1, 0], Is.EqualTo(12798));
            Assert.That(_outputOneLayerTissue.RefDynMT_rmt_subrcols[1, 1], Is.EqualTo(12631));
            Assert.That(_outputTwoLayerTissue.RefDynMT_rmt_subrcols[1, 0], Is.EqualTo(253));
            Assert.That(_outputTwoLayerTissue.RefDynMT_rmt_subrcols[1, 1], Is.EqualTo(75));
            Assert.That(_outputTwoLayerTissue.RefDynMT_rmt_subrcols[2, 0], Is.EqualTo(12637));
            Assert.That(_outputTwoLayerTissue.RefDynMT_rmt_subrcols[2, 1], Is.EqualTo(12464));
            // verify one layer totals equal two layer totals
            // note: the two layer static (or dynamic) sum will not equal the one layer static (or dynamic)
            // because of the random number call to determine whichcollisions are static vs dynamic
            Assert.That(_outputTwoLayerTissue.RefDynMT_rmt_subrcols[1,0]+
                        _outputTwoLayerTissue.RefDynMT_rmt_subrcols[1,1]+
                        _outputTwoLayerTissue.RefDynMT_rmt_subrcols[2,0]+
                        _outputTwoLayerTissue.RefDynMT_rmt_subrcols[2,1], 
                Is.EqualTo(_outputOneLayerTissue.RefDynMT_rmt_subrcols[1, 0] +
                           _outputOneLayerTissue.RefDynMT_rmt_subrcols[1, 1]));
        }

        // Transmitted Momentum Transfer of Rho and SubRegion
        [Test]
        public void validate_DAW_TransmittedDynamicMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.That(Math.Abs(_outputOneLayerTissue.TransDynMT_rmt[3, 19] - 0.000326), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputOneLayerTissue.TransDynMT_rmt2[3, 19] - 1.06449e-5), Is.LessThan(0.00001e-5));
            // make sure mean integral over MT equals T(rho) results
            var mtbins = ((TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransDynMT_rmt[3, i];
            }
            Assert.That(Math.Abs(_outputOneLayerTissue.T_r[3] - integral), Is.LessThan(0.000001));
        }
        // Reflected Dynamic Momentum Transfer of X, Y and SubRegion
        [Test]
        public void validate_DAW_ReflectedDynamicMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt[0, 0, 23] - 0.000917), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt2[0, 0, 23] - 0.000084), Is.LessThan(0.000001));
            // make sure mean integral over MT equals R(rho) results
            var mtbins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefDynMT_xymt[0, 0, i];
            }
            Assert.That(Math.Abs(_outputOneLayerTissue.R_xy[0, 0] - integral), Is.LessThan(0.000001));

            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var xbins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist")).X;
            var ybins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist")).Y;
            var fracMTbins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist")).FractionalMTBins;
            for (int l = 0; l < xbins.Count - 1; l++)
            {
                for (int i = 0; i < ybins.Count - 1; i++)
                {
                    for (int j = 0; j < mtbins.Count - 1; j++)
                    {
                        integral = 0.0;
                        for (int m = 0; m < fracMTbins.Count + 1; m++)
                        {
                            integral += _outputOneLayerTissue.RefDynMT_xymt_frac[l, i, j, m];
                        }
                        Assert.That(Math.Abs(integral - _outputOneLayerTissue.RefDynMT_xymt[l, i, j]), Is.LessThan(0.001));
                    }
                }
            }
            // validate a few fractional values - indices rho, mtbins, fraction
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 5] - 0.00091), Is.LessThan(0.00001));
            // validate 2 layer tissue results 
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 5] - 0.00091), Is.LessThan(0.00001));
            // validate dynamic results
            Assert.That(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt_dynofz[0, 0, 1] - 0.00221), Is.LessThan(0.00001));
            Assert.That(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt_dynofz[0, 0, 1] - 0.00221), Is.LessThan(0.00001));
            // validate SubregionCollision static, dynamic count for one and two layer tissue
            Assert.That(_outputOneLayerTissue.RefDynMT_xymt_subrcols[1, 0], Is.EqualTo(12803));
            Assert.That(_outputOneLayerTissue.RefDynMT_xymt_subrcols[1, 1], Is.EqualTo(12626));
            Assert.That(_outputTwoLayerTissue.RefDynMT_xymt_subrcols[1, 0], Is.EqualTo(273));
            Assert.That(_outputTwoLayerTissue.RefDynMT_xymt_subrcols[1, 1], Is.EqualTo(55));
            Assert.That(_outputTwoLayerTissue.RefDynMT_xymt_subrcols[2, 0], Is.EqualTo(12630));
            Assert.That(_outputTwoLayerTissue.RefDynMT_xymt_subrcols[2, 1], Is.EqualTo(12471));
            // verify one layer totals equal two layer totals
            // note: the two layer static (or dynamic) sum will not equal the one layer static (or dynamic)
            // because of the random number call to determine whichcollisions are static vs dynamic
            Assert.That(_outputTwoLayerTissue.RefDynMT_xymt_subrcols[1, 0] +
                        _outputTwoLayerTissue.RefDynMT_xymt_subrcols[1, 1] +
                        _outputTwoLayerTissue.RefDynMT_xymt_subrcols[2, 0] +
                        _outputTwoLayerTissue.RefDynMT_xymt_subrcols[2, 1],
                        Is.EqualTo(_outputOneLayerTissue.RefDynMT_xymt_subrcols[1, 0] +
                                   _outputOneLayerTissue.RefDynMT_xymt_subrcols[1, 1]));
        }
        // Transmitted Momentum Transfer of X, Y and SubRegion
        [Test]
        public void validate_DAW_TransmittedDynamicMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.That(Math.Abs(_outputOneLayerTissue.TransDynMT_xymt[0, 0, 45] - 0.000155), Is.LessThan(0.000001));
            // make sure mean integral over MT equals T(rho) results
            var mtbins = ((TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransDynMT_xymt[0, 0, i];
            }
            Assert.That(Math.Abs(_outputOneLayerTissue.T_xy[0, 0] - integral), Is.LessThan(0.000001));
        }
    }
}
