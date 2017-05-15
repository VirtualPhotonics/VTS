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
        private SimulationOutput _outputNA0, _outputNA0p3;
        private double _dosimetryDepth = 1.0;

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
                    new RDiffuseDetectorInput() {FinalTissueRegionIndex=0, NA=0.0},         
                    new ROfAngleDetectorInput() {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndAngleDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 11), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndTimeDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 11), Time = new DoubleRange(0.0, 1.0, 11),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),FinalTissueRegionIndex= 0, NA = 0.0 },
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20),FinalTissueRegionIndex= 0, NA = 0.0}, 
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20)}, // DJC - edited to reflect frequency sampling points (not bins)
                    new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), FinalTissueRegionIndex = 0, NA = 0.0 },
                    new ROfFxAndTimeDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), Time = new DoubleRange(0.0, 1.0, 11), FinalTissueRegionIndex = 0,NA=0.0},        
                    new RSpecularDetectorInput() {FinalTissueRegionIndex=0,NA=0.0},
                    new TDiffuseDetectorInput() {FinalTissueRegionIndex=2, NA=0.0},         
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex= 2, NA = 0.0},
                    new TOfRhoAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10.0, 11), Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex=2,NA = 0.0},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex= 2, NA = 0.0},
                    new TOfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11), FinalTissueRegionIndex = 2, NA=0.0},
                    new RadianceOfRhoAtZDetectorInput() {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex=1, NA=0.0},

                    new ReflectedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA = 0.0
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA = 0.0
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA = 0.0
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
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
                    new RDiffuseDetectorInput() {FinalTissueRegionIndex=0, NA=0.3},         
                    new ROfAngleDetectorInput() {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoAndAngleDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 11), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoAndTimeDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 11), Time = new DoubleRange(0.0, 1.0, 11),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),FinalTissueRegionIndex= 0, NA=0.3 },
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20),FinalTissueRegionIndex= 0, NA=0.3}, 
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20)}, // DJC - edited to reflect frequency sampling points (not bins)
                    new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), FinalTissueRegionIndex = 0, NA=0.3 },
                    new ROfFxAndTimeDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 5), Time = new DoubleRange(0.0, 1.0, 11), FinalTissueRegionIndex = 0,NA=0.3},        
                    new RSpecularDetectorInput() {FinalTissueRegionIndex=0,NA=0.3},
                    new TDiffuseDetectorInput() {FinalTissueRegionIndex=2, NA=0.3},         
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex= 2, NA=0.3},
                    new TOfRhoAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10.0, 11), Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex=2,NA=0.3},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex= 2, NA=0.3},
                    new TOfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11), FinalTissueRegionIndex = 2, NA=0.3},
                  
                    new RadianceOfRhoAtZDetectorInput() {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex=1,NA=0.3},

                    new ReflectedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA=0.3
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA=0.3
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA=0.3
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA=0.3
                    },
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
            Assert.AreEqual(_outputNA0.Rspec, 0.02); // specular reflection of collimated beam is [0,0,-1] so passes NA
            Assert.AreEqual(_outputNA0.Td, 0.0);
            Assert.AreEqual(_outputNA0.T_r[0], 0.0);
            Assert.AreEqual(_outputNA0.T_a[0], 0.0);
            Assert.AreEqual(_outputNA0.T_ra[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.T_xy[0, 0], 0.0);
            Assert.Less(Math.Abs(_outputNA0.Rad_r[0] - 0.006366), 0.000001); // shallow dosimetry depth catches initial flight of [0,0,1] so passes NA
            Assert.AreEqual(_outputNA0.RefMT_rmt[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.RefMT_xymt[0, 0, 0], 0.0);
            Assert.AreEqual(_outputNA0.TransMT_rmt[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.TransMT_xymt[0, 0, 0], 0.0);
            Assert.AreEqual(_outputNA0.RefDynMT_rmt[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.RefDynMT_xymt[0, 0, 0], 0.0);
            Assert.AreEqual(_outputNA0.TransDynMT_rmt[0, 0], 0.0);
            Assert.AreEqual(_outputNA0.TransDynMT_xymt[0, 0, 0], 0.0);
        }
        /// <summary>
        /// test to validate partially open NA validation values taken from prior test run
        /// Note: these results will not align with DAWLayersDetectorsTests because Dynamic detectors included
        /// and they change sequence of RNG
        /// </summary>
        [Test]
        public void validate_detector_tallies_are_zero_when_NA_is_0p3()
        {
            Assert.Less(Math.Abs(_outputNA0p3.Rd - 0.082815), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_r[1] - 0.002943), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_a[0] - 0.011866), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_ra[1, 0] - 0.000421), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_rt[1, 0] - 0.029431), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_rw[1, 0].Real - 0.002942), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_rw[1, 0].Imaginary + 3.36012e-5), 0.00001e-5);
            Assert.Less(Math.Abs(_outputNA0p3.R_xy[0, 7] - 0.001780), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_fx[1].Real - 0.080613), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_fx[1].Imaginary - 0.006354), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_fxt[1, 0].Real - 0.106022), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.R_fxt[1, 0].Imaginary - 0.108368), 0.000001);
            Assert.AreEqual(_outputNA0p3.Rspec, 0.03);
            Assert.Less(Math.Abs(_outputNA0p3.Td - 0.038553), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.T_r[4] - 0.000239), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.T_a[0] - 0.005524), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.T_ra[1, 0] - 0.000216), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.T_xy[0, 3] - 0.000221), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.Rad_r[0] - 0.022439), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.RefMT_rmt[1, 0] - 0.002943), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.RefMT_xymt[0, 7, 0] - 0.001780), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.TransMT_rmt[1, 0] - 0.001512), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.TransMT_xymt[0, 3, 1] - 0.000221), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.RefDynMT_rmt[1, 0] - 0.002943), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.RefDynMT_xymt[0, 7, 0] - 0.001780), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.TransDynMT_rmt[1, 0] - 0.001512), 0.000001);
            Assert.Less(Math.Abs(_outputNA0p3.TransDynMT_xymt[0, 3, 1] - 0.000221), 0.000001);
        }
       
    }
}
