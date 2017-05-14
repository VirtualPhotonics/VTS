using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector NA processes the
    /// exiting photon correctly
    /// </summary>
    [TestFixture]
    public class DetectorNATests
    {
        private SimulationOutput _outputNA0, _outputNA0p2;
        private double _dosimetryDepth = 2.0;

        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and specify reflectance
        /// and transmittance detectors
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
            var tissue = new MultiLayerTissueInput(
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
                });
            var detectorsNA0 =  new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput() {FinalTissueRegionIndex=0, NA=0.0},         
                    new ROfAngleDetectorInput() {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndAngleDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndTimeDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), Time = new DoubleRange(0.0, 1.0, 101),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 101), Y = new DoubleRange(-10.0, 10.0, 101),FinalTissueRegionIndex= 0, NA = 0.0 },
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 101), Omega = new DoubleRange(0.05, 1.0, 20),FinalTissueRegionIndex= 0, NA = 0.0}, 
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 101), Omega = new DoubleRange(0.05, 1.0, 20)}, // DJC - edited to reflect frequency sampling points (not bins)
                    new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), FinalTissueRegionIndex = 0, NA = 0.0 },
                    new ROfFxAndTimeDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), Time = new DoubleRange(0.0, 1.0, 101), FinalTissueRegionIndex = 0,NA=0.0},        
                    
                    new TDiffuseDetectorInput() {FinalTissueRegionIndex=2, NA=0.0},         
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex= 2, NA = 0.0},
                    new TOfRhoAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10.0, 101), Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex=2,NA = 0.0},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),FinalTissueRegionIndex= 2, NA = 0.0},
                    new TOfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 101), Y = new DoubleRange(-10.0, 10.0, 101), FinalTissueRegionIndex = 2, NA=0.0},
                  
                    //new RadianceOfRhoAtZDetectorInput() {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 101)},

                    new ReflectedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                            Rho=new DoubleRange(0.0, 10.0, 101), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                            MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
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

            // set up detectors with NA = 0.7 -> sin(theta)=0.5
            var detectorsNA0p2 = new List<IDetectorInput>
            {
                //new ROfAngleDetectorInput() {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), TallySecondMoment = true, FinalTissueRegionIndex = 1, NA = 0.4},
            };
            input = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                tissue,
                detectorsNA0p2);
            _outputNA0p2 = new MonteCarloSimulation(input).Run();
        }


        [Test]
        public void validate_detector_tallies_are_zero_when_NA_is_zero()
        {
            Assert.AreEqual(_outputNA0.Rd, 0.0);
            Assert.AreEqual(_outputNA0.R_r[0], 0.0);
            Assert.AreEqual(_outputNA0.R_a[0], 0.0);
            Assert.AreEqual(_outputNA0.R_ra[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.R_rt[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.R_rw[0, 0].Real, 0.0);
            Assert.AreEqual(_outputNA0.R_rw[0, 0].Imaginary, 0.0);
            Assert.AreEqual(_outputNA0.R_xy[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.R_fx[0].Real, 0.0);
            Assert.AreEqual(_outputNA0.R_fx[0].Imaginary, 0.0);
            Assert.AreEqual(_outputNA0.R_fxt[0, 0].Real, 0.0);
            Assert.AreEqual(_outputNA0.R_fxt[0, 0].Imaginary, 0.0);
            Assert.AreEqual(_outputNA0.Td, 0.0);
            Assert.AreEqual(_outputNA0.T_r[0], 0.0);
            Assert.AreEqual(_outputNA0.T_a[0], 0.0);
            Assert.AreEqual(_outputNA0.T_ra[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.T_xy[0, 0], 0.0);
            //Assert.AreEqual(_outputNA0.Rad_r[0], 0.0);
        }
        /// <summary>
        /// test to validate partially open NA
        /// validation values taken from prior test run
        /// </summary>
        [Test]
        public void validate_detector_tallies_are_zero_when_NA_is_0p3()
        {
            Assert.Less(Math.Abs(_outputNA0p2.R_r[0] - 0.3170404), 0.0000001);
            //Assert.AreEqual(_output.R_r2[0], 0.0);
            //Assert.AreEqual(_output.R_a[0], 0.0);
            //Assert.AreEqual(_output.R_ra[0, 0], 0.0);
            //Assert.AreEqual(_output.R_rt[0, 0], 0.0);
            //Assert.AreEqual(_output.R_rw[0, 0], 0.0);
            //Assert.AreEqual(_output.T_r[0], 0.0);
            //Assert.AreEqual(_output.T_a[0], 0.0);
            //Assert.AreEqual(_output.T_ra[0, 0], 0.0);
            //Assert.AreEqual(_output.R_xy[0, 0], 0.0);
            //Assert.AreEqual(_output.Rad_r[0], 0.0);
        }
        //// Reflected Momentum Transfer of Rho and SubRegion
        //[Test]
        //public void validate_DAW_ReflectedMTOfRhoAndSubregionHist()
        //{
        //    // use initial results to verify any new changes to the code
        //    Assert.Less(Math.Abs(_output.RefMT_rs_hist[0, 0] - 0.632816), 0.000001);
        //    // make sure mean integral over MT equals R(rho) results
        //    var mtbins = ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
        //        Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First()).MTBins;
        //    var integral = 0.0;
        //    for (int i = 0; i < mtbins.Count - 1; i++)
        //    {
        //        integral += _output.RefMT_rs_hist[0, i];
        //    }
        //    Assert.Less(Math.Abs(_output.R_r[0] - integral), 0.000001);
        //}
    }
}
