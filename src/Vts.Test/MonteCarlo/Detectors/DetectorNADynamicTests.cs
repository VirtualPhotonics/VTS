using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector NA processes the exiting photon correctly
    /// for *dynamic* detectors since they call random number and skew RNG sequence
    /// </summary>
    [TestFixture]
    public class DetectorNADynamicTests
    {
        private SimulationOutput _outputNA0, _outputNA0p3, _outputNoNASpecified;

        /// <summary>
        /// Setup input to the MC for a homogeneous one layer tissue and specify reflectance
        /// and transmittance detectors
        /// </summary>
        [OneTimeSetUp]
        public void Execute_Monte_Carlo()
        {
            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>() { }, 
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            var source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     0); // start in air
            var tissue = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion( 
                        new DoubleRange(0.0, 10.0), // make tissue layer thin so transmittance results improved
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
            var detectorsNA0 =  new List<IDetectorInput>
                {
                    new ReflectedDynamicMTOfFxAndSubregionHistDetectorInput()
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        FinalTissueRegionIndex= 0,
                        NA = 0.0
                    },
                    new TransmittedDynamicMTOfFxAndSubregionHistDetectorInput()
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11),
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 },
                        FinalTissueRegionIndex= 2,
                        NA = 0.0
                    },
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        FinalTissueRegionIndex= 0, 
                        NA = 0.0
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 },
                        FinalTissueRegionIndex= 2, 
                        NA = 0.0
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 5), 
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        FinalTissueRegionIndex= 0, 
                        NA = 0.0
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0} ,
                        FinalTissueRegionIndex= 2, 
                        NA = 0.0
                    }
                };
            var input = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissue,
                detectorsNA0);             
            _outputNA0 = new MonteCarloSimulation(input).Run();

            var detectorsNA0p3 = new List<IDetectorInput>
                { 
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        FinalTissueRegionIndex= 0, 
                        NA=0.3
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5), 
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 },
                        FinalTissueRegionIndex= 2, 
                        NA=0.3
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 5), 
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                        FinalTissueRegionIndex= 0, 
                        NA=0.3
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0} ,
                        FinalTissueRegionIndex= 2, 
                        NA = 0.3
                    }
                };
            input = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissue,
                detectorsNA0p3);
            _outputNA0p3 = new MonteCarloSimulation(input).Run();

            var detectorsNoNASpecified = new List<IDetectorInput>
                {
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                    },
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5), 
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0 },
                    },
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 5), 
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0},
                    },
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11), 
                        Z = new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 5),
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0} ,
                    }
                };
            input = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissue,
                detectorsNoNASpecified);
            _outputNoNASpecified = new MonteCarloSimulation(input).Run();
        }

        /// <summary>
        /// test to validate NA=0.  Note that not all validation values are 0 due to vertical detection
        /// </summary>
        [Test]
        public void Validate_dynamic_detector_tallies_are_zero_when_NA_is_zero()
        {
            Assert.AreEqual(0.0, _outputNA0.RefDynMT_fxmt[0, 0].Magnitude);
            Assert.AreEqual(0.0, _outputNA0.RefDynMT_rmt[0, 0]);
            Assert.AreEqual(0.0, _outputNA0.RefDynMT_xymt[0, 0, 0]);
            Assert.AreEqual(0.0, _outputNA0.TransDynMT_fxmt[0, 0].Magnitude);
            Assert.AreEqual(0.0, _outputNA0.TransDynMT_rmt[0, 0]);
            Assert.AreEqual(0.0, _outputNA0.TransDynMT_xymt[0, 0, 0]);
        }
        /// <summary>
        /// test to validate partially open NA validation values taken from prior test run
        /// Note: since dynamic (RNG called in detector), the validation values will change if
        /// change to test
        /// </summary>
        [Test]
        public void Validate_detector_tallies_when_NA_is_0p3()
        {
            Assert.Less(Math.Abs(_outputNA0p3.RefDynMT_rmt[0, 0] - 0.006296), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.RefDynMT_xymt[4, 4, 0] - 0.002490), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.TransDynMT_rmt[2, 0] - 0.000469), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.TransDynMT_xymt[1, 9, 1] - 0.000678), 0.000001);
        }

        /// <summary>
        /// test for backwards compatibility to make sure if the infile defined detectors that
        /// did not specify NA or FinalTissueRegion, then the default settings of these (NA=double.Infinity,
        /// FinalTissueRegion=1) occur and give non-zero results.
        /// /// </summary>
        [Test]
        public void Validate_dynamic_detector_tallies_are_not_zero_when_NA_is_not_specified()
        {
            Assert.AreNotEqual(0.0, _outputNoNASpecified.RefDynMT_rmt[1, 0]);
            Assert.AreNotEqual(0.0, _outputNoNASpecified.RefDynMT_xymt[0, 9, 1]);
            Assert.AreNotEqual(0.0, _outputNoNASpecified.TransDynMT_rmt[1, 0]);
            Assert.AreNotEqual(0.0, _outputNoNASpecified.TransDynMT_xymt[0, 0, 0]);
        }

    }
}
