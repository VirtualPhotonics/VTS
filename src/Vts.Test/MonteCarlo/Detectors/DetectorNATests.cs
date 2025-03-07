using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests verify that the specification of a detector NA processes the exiting photon correctly
    /// </summary>
    [TestFixture]
    public class DetectorNATests
    {
        private SimulationInput _inputForPmc;
        private SimulationOutput _outputNa0, _outputNa0P3, _outputNoNaNoFinalTissueRegionSpecified, _outputNa0p3FinalTissueRegion1;
        private const double _dosimetryDepth = 1.0;
        private pMCDatabase _pMcDatabase;

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
                new List<DatabaseType> { DatabaseType.pMCDiffuseReflectance }, // write database for pMC tests
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
            var detectorsNa0 =  new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput {FinalTissueRegionIndex=0, NA=0.0},         
                    new ROfAngleDetectorInput {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndAngleDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndTimeDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Time = new DoubleRange(0.0, 1.0, 11),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfRhoAndOmegaDetectorInput { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20),FinalTissueRegionIndex= 0, NA = 0.0},
                    new ROfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),FinalTissueRegionIndex= 0, NA = 0.0 },
                    new ROfXAndYAndTimeDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),Time=new DoubleRange(0, 1, 11), FinalTissueRegionIndex= 0, NA = 0.0 },
                    new ROfXAndYAndMaxDepthDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),MaxDepth=new DoubleRange(0,10.0,11),FinalTissueRegionIndex= 0, NA = 0.0 },
                    new ROfFxDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), FinalTissueRegionIndex = 0, NA = 0.0 },
                    new ROfFxAndTimeDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), Time = new DoubleRange(0.0, 1.0, 11), FinalTissueRegionIndex = 0,NA=0.0},
                    new ROfFxAndAngleDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2), FinalTissueRegionIndex = 0,NA=0.0},
                    new RSpecularDetectorInput {FinalTissueRegionIndex=0,NA=0.0},
                    new TDiffuseDetectorInput {FinalTissueRegionIndex=2, NA=0.0},         
                    new TOfAngleDetectorInput {Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex= 2, NA = 0.0},
                    new TOfRhoAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11), Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex=2,NA = 0.0},
                    new TOfRhoDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex= 2, NA = 0.0},
                    new TOfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11), FinalTissueRegionIndex = 2, NA=0.0},
                    new RadianceOfRhoAtZDetectorInput {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex=1, NA=0.0},
                    new ROfXAndYAndThetaAndPhiDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11),
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Theta=new DoubleRange(Math.PI / 2, Math.PI, 2),
                        Phi = new DoubleRange(0, 2* Math.PI, 2),
                        FinalTissueRegionIndex= 0, NA=0.0
                    },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA = 0.0
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA = 0.0
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA = 0.0
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA = 0.0
                    },
                };
            var inputNa0 = new SimulationInput(
                100,
                "",
                new SimulationOptions { Seed = 0 }, // no database gen 
                source,
                tissue,
                detectorsNa0);             
            _outputNa0 = new MonteCarloSimulation(inputNa0).Run();

            var detectorsNa0P3 = new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput {FinalTissueRegionIndex=0, NA=0.3},         
                    new ROfAngleDetectorInput {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoAndAngleDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoAndTimeDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Time = new DoubleRange(0.0, 1.0, 11),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfRhoAndOmegaDetectorInput { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20),FinalTissueRegionIndex= 0, NA=0.3},
                    new ROfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),FinalTissueRegionIndex= 0, NA=0.3 },
                    new ROfXAndYAndTimeDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),Time=new DoubleRange(0, 1, 11), FinalTissueRegionIndex= 0, NA = 0.3 },
                    new ROfXAndYAndMaxDepthDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),MaxDepth=new DoubleRange(0,10.0,11),FinalTissueRegionIndex= 0, NA = 0.3 },
                    new ROfFxDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), FinalTissueRegionIndex = 0, NA=0.3 },
                    new ROfFxAndTimeDetectorInput {Fx = new DoubleRange(0.0, 0.5, 5), Time = new DoubleRange(0.0, 1.0, 11), FinalTissueRegionIndex = 0,NA=0.3},
                    new ROfFxAndAngleDetectorInput {Fx = new DoubleRange(0.0, 0.5, 5), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2), FinalTissueRegionIndex = 0,NA=0.3},
                    new RSpecularDetectorInput {FinalTissueRegionIndex=0,NA=0.3},
                    new TDiffuseDetectorInput {FinalTissueRegionIndex=2, NA=0.3},         
                    new TOfAngleDetectorInput {Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex= 2, NA=0.3},
                    new TOfRhoAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11), Angle=new DoubleRange(0.0, Math.PI / 2, 2),FinalTissueRegionIndex=2,NA=0.3},
                    new TOfRhoDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex= 2, NA=0.3},
                    new TOfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11), FinalTissueRegionIndex = 2, NA=0.3},
                  
                    new RadianceOfRhoAtZDetectorInput {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 11),FinalTissueRegionIndex=1,NA=0.3},
                    new ROfXAndYAndThetaAndPhiDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11),
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Theta=new DoubleRange(Math.PI / 2, Math.PI, 2),
                        Phi = new DoubleRange(0, 2* Math.PI, 2),
                        FinalTissueRegionIndex= 0,
                        NA=0.3
                    },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA=0.3
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 0, 
                            NA=0.3
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA=0.3
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            FinalTissueRegionIndex= 2, 
                            NA=0.3
                    },
                };
            var inputNa0P3 = new SimulationInput(
                100,
                "",
                new SimulationOptions { Seed = 0 }, // don't turn on database gen
                source,
                tissue,
                detectorsNa0P3);
            _outputNa0P3 = new MonteCarloSimulation(inputNa0P3).Run();

            var detectorsNoNaNoFinalTissueRegionSpecified = new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput {},         
                    new ROfAngleDetectorInput {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                    new ROfRhoDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11)},
                    new ROfRhoAndAngleDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2)},
                    new ROfRhoAndTimeDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Time = new DoubleRange(0.0, 1.0, 11)},
                    new ROfRhoAndOmegaDetectorInput { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20)},
                    new ROfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11) },
                    new ROfXAndYAndTimeDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),Time=new DoubleRange(0, 1, 11), FinalTissueRegionIndex= 0 },
                    new ROfXAndYAndMaxDepthDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),MaxDepth=new DoubleRange(0,10.0,11),FinalTissueRegionIndex= 0 },
                    new ROfFxDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51) },
                    new ROfFxAndTimeDetectorInput {Fx = new DoubleRange(0.0, 0.5, 5), Time = new DoubleRange(0.0, 1.0, 11)},
                    new ROfFxAndAngleDetectorInput {Fx = new DoubleRange(0.0, 0.5, 5), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2), FinalTissueRegionIndex = 0},
                    new RSpecularDetectorInput {},
                    new TDiffuseDetectorInput {},         
                    new TOfAngleDetectorInput {Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfRhoAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11), Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfRhoDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11)},
                    new TOfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11)},
                    new ROfXAndYAndThetaAndPhiDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Theta=new DoubleRange(Math.PI / 2, Math.PI, 2),
                        Phi = new DoubleRange(0, 2* Math.PI, 2),
                        FinalTissueRegionIndex= 0
                    },

                    new RadianceOfRhoAtZDetectorInput {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 11)},

                    new ReflectedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11), 
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11), 
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5), 
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                    },
                };
            var inputNoNaNoFinalTissueRegionSpecified = new SimulationInput(
                100,
                "",
                simulationOptions, // turn on database gen when NA fully open
                source,
                tissue,
                detectorsNoNaNoFinalTissueRegionSpecified);
            _outputNoNaNoFinalTissueRegionSpecified = new MonteCarloSimulation(inputNoNaNoFinalTissueRegionSpecified).Run();

            _inputForPmc = inputNoNaNoFinalTissueRegionSpecified;  // set pMC input to one that specified database generation
            _pMcDatabase = pMCDatabase.FromFile("DiffuseReflectanceDatabase", "CollisionInfoDatabase"); // grab database 

            var detectorsNa0P3FinalTissueRegion1 = new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput { NA=0.3,FinalTissueRegionIndex = 1},
                    new ROfAngleDetectorInput {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),NA=0.3, FinalTissueRegionIndex = 1},
                    new ROfRhoDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11),NA=0.3, FinalTissueRegionIndex = 1},
                    new ROfRhoAndAngleDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),NA=0.3, FinalTissueRegionIndex = 1},
                    new ROfRhoAndTimeDetectorInput {Rho = new DoubleRange(0.0, 10.0, 11), Time = new DoubleRange(0.0, 1.0, 11),NA=0.3, FinalTissueRegionIndex = 1},
                    new ROfRhoAndOmegaDetectorInput { Rho = new DoubleRange(0.0, 10.0, 11), Omega = new DoubleRange(0.05, 1.0, 20),NA=0.3, FinalTissueRegionIndex = 1},
                    new ROfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),NA=0.3, FinalTissueRegionIndex = 1 },
                    new ROfXAndYAndTimeDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),Time=new DoubleRange(0, 1, 11),NA=0.3, FinalTissueRegionIndex= 1 },
                    new ROfXAndYAndMaxDepthDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),MaxDepth=new DoubleRange(0,10.0,11),NA=0.3,FinalTissueRegionIndex= 1 },
                    new ROfFxDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51),NA=0.3, FinalTissueRegionIndex = 1},
                    new ROfFxAndTimeDetectorInput {Fx = new DoubleRange(0.0, 0.5, 5), Time = new DoubleRange(0.0, 1.0, 11),NA=0.3, FinalTissueRegionIndex=1},
                    new ROfFxAndAngleDetectorInput {Fx = new DoubleRange(0.0, 0.5, 5), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),NA=0.3, FinalTissueRegionIndex = 1}, 
                    new TDiffuseDetectorInput {NA=0.3, FinalTissueRegionIndex=1},
                    new TOfAngleDetectorInput {Angle=new DoubleRange(0.0, Math.PI / 2, 2),NA=0.3,FinalTissueRegionIndex=1},
                    new TOfRhoAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11), Angle=new DoubleRange(0.0, Math.PI / 2, 2),NA=0.3,FinalTissueRegionIndex=1},
                    new TOfRhoDetectorInput {Rho=new DoubleRange(0.0, 10.0, 11),NA=0.3,FinalTissueRegionIndex=1},
                    new TOfXAndYDetectorInput { X = new DoubleRange(-10.0, 10.0, 11), Y = new DoubleRange(-10.0, 10.0, 11),NA=0.3,FinalTissueRegionIndex=1},
                    new ROfXAndYAndThetaAndPhiDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11),
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Theta=new DoubleRange(Math.PI / 2, Math.PI, 2),
                        Phi = new DoubleRange(0, 2* Math.PI, 2),
                        NA=0.3,FinalTissueRegionIndex= 1
                    },

                    new RadianceOfRhoAtZDetectorInput {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 11),NA=0.3,FinalTissueRegionIndex=1},

                    new ReflectedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5),
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            NA=0.3,FinalTissueRegionIndex=1
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11),
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5),
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            NA=0.3,FinalTissueRegionIndex=1
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput
                    {
                            Rho=new DoubleRange(0.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5),
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            NA=0.3,FinalTissueRegionIndex=1
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput
                    {
                            X=new DoubleRange(-10.0, 10.0, 11),
                            Y=new DoubleRange(-10.0, 10.0, 11),
                            MTBins=new DoubleRange(0.0, 500.0, 5),
                            FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                            NA=0.3,FinalTissueRegionIndex=1
                    },
                };
            var inputNa0P3FinalTissueRegion1 = new SimulationInput(
            100,
            "",
            new SimulationOptions { Seed = 0 }, // don't turn on database gen
            source,
            tissue,
            detectorsNa0P3FinalTissueRegion1);
            _outputNa0p3FinalTissueRegion1 = new MonteCarloSimulation(inputNa0P3FinalTissueRegion1).Run();

        }

        /// <summary>
        /// test to validate NA=0.  Note that not all validation values are 0 due to vertical detection
        /// </summary>
        [Test]
        public void Validate_detector_tallies_are_zero_when_NA_is_zero()
        {
            Assert.That(_outputNa0.Rd, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_r[0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_a[0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_ra[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_rt[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_rw[0, 0].Real, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_rw[0, 0].Imaginary, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_xy[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_xyt[0, 0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_xymd[0, 0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_fx[0].Real, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_fx[0].Imaginary, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_fxt[0, 0].Real, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_fxt[0, 0].Imaginary, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_fxa[0, 0].Real, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_fxa[0, 0].Imaginary, Is.EqualTo(0.0));
            Assert.That(_outputNa0.R_xytp[0, 0, 0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.Rspec, Is.EqualTo(0.01)); // specular reflection of collimated beam is [0,0,-1] so passes NA
            Assert.That(_outputNa0.Td, Is.EqualTo(0.0));
            Assert.That(_outputNa0.T_r[0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.T_a[0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.T_ra[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.T_xy[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.Rad_r[0], Is.EqualTo(0.0)); 
            Assert.That(_outputNa0.RefMT_rmt[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.RefMT_xymt[0, 0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.TransMT_rmt[0, 0], Is.EqualTo(0.0));
            Assert.That(_outputNa0.TransMT_xymt[0, 0, 0], Is.EqualTo(0.0));
        }

        /// <summary>
        /// test to validate partially open NA validation values taken from prior test run
        /// </summary>
        [Test]
        public void Validate_detector_tallies_when_NA_is_0p3()
        {
            Assert.That(Math.Abs(_outputNa0P3.Rd - 0.045615), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_r[0] - 0.003170), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_a[0] - 0.006536), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_ra[0, 0] - 0.000454), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_rt[0, 0] - 0.031704), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_rw[0, 0].Real - 0.003170), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_rw[0, 0].Imaginary + 1.474517e-6), Is.LessThan(0.00001e-6));
            Assert.That(Math.Abs(_outputNa0P3.R_xy[3, 6] - 0.001351), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_xyt[3, 6, 3] - 0.013510), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_xymd[3, 6, 5] - 0.001351), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_fx[1].Real - 0.044879), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_fx[1].Imaginary - 0.000765), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_fxt[1, 0].Real - 0.252024), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_fxt[1, 0].Imaginary - 0.089027), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_fxa[1, 0].Real - 0.002755), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_fxa[1, 0].Imaginary - 0.001786), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.R_xytp[3, 6, 0, 0] - 0.000193), Is.LessThan(0.000001));
            Assert.That(_outputNa0P3.Rspec, Is.EqualTo(0.01));
            Assert.That(Math.Abs(_outputNa0P3.Td - 0.023415), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.T_r[1] - 0.001520), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.T_a[0] - 0.003355), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.T_ra[1, 0] - 0.000217), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.T_xy[4, 7] - 0.000997), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.Rad_r[0] - 0.015929), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.RefMT_rmt[2, 0] - 0.000599), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.RefMT_xymt[3, 6, 0] - 0.001351), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.TransMT_rmt[1, 0] - 0.001520), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputNa0P3.TransMT_xymt[4, 7, 0] - 0.000997), Is.LessThan(0.000001));
        }

        /// <summary>
        /// test for backwards compatibility to make sure if the infile defined detectors that
        /// did not specify NA or FinalTissueRegion, then the default settings of these (NA=double.Infinity,
        /// FinalTissueRegion=1) occur and give non-zero results.
        /// /// </summary>
        [Test]
        public void Validate_detector_tallies_are_not_zero_when_NA_is_not_specified()
        {
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.Rd, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_r[1], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_a[0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_ra[1, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_rt[1, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_rw[1, 0].Real, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_rw[1, 0].Imaginary, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_xy[0, 1], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_xyt[0, 1, 3], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_xymd[0, 1, 7], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_fx[1].Real, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_fx[1].Imaginary, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_fxt[1, 0].Real, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_fxt[1, 0].Imaginary, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_fxa[1, 0].Real, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_fxa[1, 0].Imaginary, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.R_xytp[0, 1, 0, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.Rspec, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.Td, Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.T_r[1], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.T_a[0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.T_ra[1, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.T_xy[0, 2], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.Rad_r[0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.RefMT_rmt[1, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.RefMT_xymt[0, 1, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.TransMT_rmt[1, 0], Is.Not.EqualTo(0.0));
            Assert.That(_outputNoNaNoFinalTissueRegionSpecified.TransMT_xymt[0, 2, 0], Is.Not.EqualTo(0.0));
        }

        /// <summary>
        /// comparison test for NA=0.3 that specifying FinalTissueRegion=0 or 1
        /// need to pick array indices with non-zero tallies, however comparison could be
        /// equal if photons are within NA in both cases and AreEqual is used
        /// /// </summary>
        [Test]
        public void Validate_detector_tallies_for_final_tissue_region_0_or_1_comparison()
        {
            Assert.That(_outputNa0p3FinalTissueRegion1.Rd, Is.Not.EqualTo(_outputNa0P3.Rd));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_r[0], Is.Not.EqualTo(_outputNa0P3.R_r[0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_a[0], Is.Not.EqualTo(_outputNa0P3.R_a[0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_ra[0, 0], Is.Not.EqualTo(_outputNa0P3.R_ra[0, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_rt[0, 0], Is.Not.EqualTo(_outputNa0P3.R_rt[0, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_rw[0, 0].Real, Is.Not.EqualTo(_outputNa0P3.R_rw[0, 0].Real));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_rw[0, 0].Imaginary, Is.Not.EqualTo(_outputNa0P3.R_rw[0, 0].Imaginary));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_xy[3, 6], Is.EqualTo(_outputNa0P3.R_xy[3, 6]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_xyt[3, 6, 3], Is.EqualTo(_outputNa0P3.R_xyt[3, 6, 3]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_xymd[3, 6, 5], Is.EqualTo(_outputNa0P3.R_xymd[3, 6, 5]));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_fx[1].Real, Is.Not.EqualTo(_outputNa0P3.R_fx[1].Real));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_fx[1].Imaginary, Is.Not.EqualTo(_outputNa0P3.R_fx[1].Imaginary));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_fxt[1, 0].Real, Is.Not.EqualTo(_outputNa0P3.R_fxt[1, 0].Real));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_fxt[1, 0].Imaginary, Is.Not.EqualTo(_outputNa0P3.R_fxt[1, 0].Imaginary));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_fxa[1, 0].Real, Is.Not.EqualTo(_outputNa0P3.R_fxa[1, 0].Real));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_fxa[1, 0].Imaginary, Is.Not.EqualTo(_outputNa0P3.R_fxa[1, 0].Imaginary));
            Assert.That(_outputNa0p3FinalTissueRegion1.R_xytp[3, 6, 0, 0], Is.EqualTo(_outputNa0P3.R_xytp[3, 6, 0, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.Td, Is.EqualTo(_outputNa0P3.Td));
            Assert.That(_outputNa0p3FinalTissueRegion1.T_r[1], Is.EqualTo(_outputNa0P3.T_r[1]));
            Assert.That(_outputNa0p3FinalTissueRegion1.T_a[0], Is.EqualTo(_outputNa0P3.T_a[0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.T_ra[1, 0], Is.EqualTo(_outputNa0P3.T_ra[1, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.T_xy[4, 7], Is.EqualTo(_outputNa0P3.T_xy[4, 7]));
            Assert.That(_outputNa0p3FinalTissueRegion1.Rad_r[0], Is.Not.EqualTo(_outputNa0P3.Rad_r[0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.RefMT_rmt[2, 0], Is.Not.EqualTo(_outputNa0P3.RefMT_rmt[2, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.RefMT_xymt[3, 6, 0], Is.EqualTo(_outputNa0P3.RefMT_xymt[3, 6, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.TransMT_rmt[1, 0], Is.EqualTo(_outputNa0P3.TransMT_rmt[1, 0]));
            Assert.That(_outputNa0p3FinalTissueRegion1.TransMT_xymt[4, 7, 0], Is.EqualTo(_outputNa0P3.TransMT_xymt[4, 7, 0]));
        }

        /// <summary>
        /// Test to validate that pMC/dMC detectors tallies are 0 when NA=0
        /// </summary>
        [Test]
        public void Validate_pMC_dMC_detector_NA_tallies_are_zero_when_NA_is_0()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>
                {
                    new pMCROfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.0,
                    },
                    new pMCROfRhoAndTimeDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        Time=new DoubleRange(0.0, 1.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.0,
                    },  
                    new pMCROfFxDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 5),
                        PerturbedOps=new OpticalProperties[] { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new int[] { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.0,
                    },
                    new pMCROfFxAndTimeDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 5),
                        Time=new DoubleRange(0.0, 1.0, 11),
                        PerturbedOps=new OpticalProperties[] { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new int[] { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.0,
                    },                    
                    new dMCdROfRhodMuaDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.0,
                    },
                    new dMCdROfRhodMusDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.0,
                    }, 
                },
                _pMcDatabase,
                _inputForPmc);
            var postProcessedOutput = postProcessor.Run();

            Assert.That(postProcessedOutput.pMC_R_r[0], Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.pMC_R_rt[0, 0], Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.pMC_R_fx[0].Real, Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.pMC_R_fx[0].Imaginary, Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.pMC_R_fxt[0, 0].Real, Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.pMC_R_fxt[0, 0].Imaginary, Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.dMCdMua_R_r[0], Is.EqualTo(0.0));
            Assert.That(postProcessedOutput.dMCdMus_R_r[0], Is.EqualTo(0.0));
        }

        /// <summary>
        /// Test to validate that pMC/dMC detectors with partially open NA results match prior run
        /// </summary>
        [Test]
        public void Validate_pMC_dMC_detector_NA_tallies_when_NA_is_0p3()
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>
                {
                    new pMCROfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.3,
                    },
                    new pMCROfRhoAndTimeDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        Time=new DoubleRange(0.0, 1.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.3,
                    },  
                    new pMCROfFxDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 5),
                        PerturbedOps=new OpticalProperties[] { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new int[] { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.3,
                    },
                    new pMCROfFxAndTimeDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 5),
                        Time=new DoubleRange(0.0, 1.0, 11),
                        PerturbedOps=new OpticalProperties[] { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new int[] { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.3,
                    },                    
                    new dMCdROfRhodMuaDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.3,
                    },
                    new dMCdROfRhodMusDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 11),
                        PerturbedOps=new List<OpticalProperties>
                        { // perturbed ops
                            _inputForPmc.TissueInput.Regions[0].RegionOP,
                            _inputForPmc.TissueInput.Regions[1].RegionOP,
                            _inputForPmc.TissueInput.Regions[2].RegionOP},
                        PerturbedRegionsIndices=new List<int> { 1 },
                        FinalTissueRegionIndex = 0,
                        NA=0.3,
                    }, 
                },
                _pMcDatabase,
                _inputForPmc);
            var postProcessedOutput = postProcessor.Run();

            Assert.That(Math.Abs(postProcessedOutput.pMC_R_r[0] - 0.003170), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_rt[0, 0] - 0.031704), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx[1].Real - 0.019227), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fx[1].Imaginary - 0.012466), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fxt[1, 0].Real - 0.252024), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.pMC_R_fxt[1, 0].Imaginary - 0.089027), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.dMCdMua_R_r[0] + 0.001005), Is.LessThan(0.000001));
            Assert.That(Math.Abs(postProcessedOutput.dMCdMus_R_r[0] - 0.000263), Is.LessThan(0.000001));
        }
    }
}
