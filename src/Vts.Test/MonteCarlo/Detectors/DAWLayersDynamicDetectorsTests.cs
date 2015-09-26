using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
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
        private double _layerThickness = 0.1; // tissue is homogeneous (both layer opt. props same)

        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a homogeneous
        /// two layer tissue (both layers have same optical properties), execute simulations
        /// and verify results agree with prior results.
        /// NOTE: currently two region executes same photon biography except for pauses
        /// at layer interface.  Variance for DAW results not degraded.
        /// </summary>
        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
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
                    new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 21), TallySecondMoment = true},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 21)}, 
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 21), Y = new DoubleRange(-10.0, 10.0, 21) },
                    new TOfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 21), Y = new DoubleRange(-10.0, 10.0, 21) },
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0}
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with TOfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 }
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0}
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0}   
                    }
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
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 21), // rho bins MAKE SURE AGREES with TOfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),                        
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0}
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),              
                        BloodVolumeFraction = new List<double>() { 0, 0.2, 0.5, 0},
                        TallySecondMoment = true,
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 21), 
                        Y = new DoubleRange(-10.0, 10.0, 21),
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

       
        // Reflected Dynamic Momentum Transfer of Rho and SubRegion
        [Test]
        public void validate_DAW_ReflectedDynamicMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt[0, 0] - 0.100174), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt[0, 1] - 0.011499), 0.000001);
            // verify mean integral over MT equals R(rho) results
            var mtbins = ((ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefDynMT_rmt[0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r[0] - integral), 0.000001);
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var rhobins = ((ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First()).Rho;
            var fracMTbins =
                ((ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                    Where(d => d.TallyType == "ReflectedDynamicMTOfRhoAndSubregionHist").First()).FractionalMTBins;
            var numsubregions = _inputOneLayerTissue.TissueInput.Regions.Length;
            for (int i = 0; i < rhobins.Count - 1; i++)
            {
                for (int j = 0; j < mtbins.Count - 1; j++)
                {
                    for (int k = 0; k < numsubregions; k++)
                    {
                        integral = 0.0;
                        for (int m = 0; m < fracMTbins.Count + 1; m++)
                        {
                            integral += _outputOneLayerTissue.RefDynMT_rmt_frac[i, j, k, m];
                        }
                        Assert.Less(Math.Abs(integral - _outputOneLayerTissue.RefDynMT_rmt[i, j]), 0.001);
                    }
                }
            }
            // validate a few fractional values - note third index = tissue region and 0,2 is air and should have
            // contributions only to fourth index=0
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_frac[0, 0, 0, 0] - 0.100), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_frac[0, 0, 1, 0] - 0.013), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_rmt_frac[0, 0, 1, 1] - 0.038), 0.001);
            // validate 2 layer tissue results 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_frac[0, 0, 0, 0] - 0.100), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_frac[0, 0, 1, 0] - 0.076), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_rmt_frac[0, 0, 1, 1] - 0.013), 0.001);
        }

        // Transmitted Momentum Transfer of Rho and SubRegion
        [Test]
        public void validate_DAW_TransmittedDynamicMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransDynMT_rmt[3, 19] - 0.000326), 0.000001);
            // make sure mean integral over MT equals T(rho) results
            var mtbins = ((TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TransmittedDynamicMTOfRhoAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransDynMT_rmt[3, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[3] - integral), 0.000001);
        }
        // Reflected Dynamic Momentum Transfer of X, Y and SubRegion
        [Test]
        public void validate_DAW_ReflectedDynamicMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt[0, 0, 23] - 0.000917), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt2[0, 0, 23] - 0.000084), 0.000001);
            // make sure mean integral over MT equals R(rho) results
            var mtbins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefDynMT_xymt[0, 0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[0, 0] - integral), 0.000001);

            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var xbins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput) _inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First()).X;
            var ybins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First()).Y;
            var fracMTbins = ((ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
               Where(d => d.TallyType == "ReflectedDynamicMTOfXAndYAndSubregionHist").First()).FractionalMTBins;
            var numsubregions = _inputOneLayerTissue.TissueInput.Regions.Length;
            for (int l = 0; l < xbins.Count - 1; l++)
            {
                for (int i = 0; i < ybins.Count - 1; i++)
                {
                    for (int j = 0; j < mtbins.Count - 1; j++)
                    {
                        for (int k = 0; k < numsubregions; k++)
                        {
                            integral = 0.0;
                            for (int m = 0; m < fracMTbins.Count + 1; m++)
                            {
                                integral += _outputOneLayerTissue.RefDynMT_xymt_frac[l, i, j, k, m];
                            }
                            Assert.Less(Math.Abs(integral - _outputOneLayerTissue.RefDynMT_xymt[l, i, j]), 0.001);
                        }
                    }
                }
            }
            // validate a few fractional values - note third index = 0,2 is air and should have
            // contributions only to fourth index=0
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 0, 0] - 0.001), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 1, 5] - 0.001), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 2, 0] - 0.001), 0.001);
            // validate 2 layer tissue results - complementary fracs should be the same in
            // the two layers, i.e. if region 1 has (0,0.1] weight, then region 2 should have (0.9,1] same weight 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 1, 0] - 0.001), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 2, 5] - 0.001), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefDynMT_xymt_frac[0, 0, 23, 3, 0] - 0.001), 0.001);
        }
        // Transmitted Momentum Transfer of X, Y and SubRegion
        [Test]
        public void validate_DAW_TransmittedDynamicMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransDynMT_xymt[0, 0, 45] - 0.000155), 0.000001);
            // make sure mean integral over MT equals T(rho) results
            var mtbins = ((TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TransmittedDynamicMTOfXAndYAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransDynMT_xymt[0, 0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xy[0, 0] - integral), 0.000001);
        }
    }
}
