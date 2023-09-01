using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used SimulationInput classes for various tissue types.
    /// </summary>
    public class SimulationInputProvider : SimulationInput
    {
        /// <summary>
        /// Method that provides instances of all inputs in this class.
        /// </summary>
        /// <returns>a list of the SimulationInputs generated</returns>
        public static IList<SimulationInput> GenerateAllSimulationInputs()
        {
            // additions to this list need to be added to MCCL Program tests for clean up
            return new List<SimulationInput>
            {
                PointSourceOneLayerTissueAllDetectors(),
                PointSourceOneLayerTissueROfRhoAndFluenceOfRhoAndZDetectors(),
                PointSourceOneLayerTissueRadianceOfRhoAndZAndAngleDetector(),
                PointSourceTwoLayerTissueROfRhoDetector(),
                PointSourceTwoLayerTissueROfRhoTOfRhoDetectorWithPhotonDatabase(),
                PointSourceSingleEllipsoidTissueFluenceOfRhoAndZDetector(),
                PointSourceSingleInfiniteCylinderTissueAOfXAndYAndZDetector(),
                PointSourceMultiInfiniteCylinderTissueAOfXAndYAndZDetector(),
                pMCPointSourceOneLayerTissueROfRhoDAW(), // don't change this it is part of documentation
                Gaussian2DSourceOneLayerTissueROfRhoDetector(),
                Flat2DSourceOneLayerTissueROfRhoDetector(),
                Flat2DSourceTwoLayerBoundedTissueAOfRhoAndZDetector(),
                GaussianLineSourceOneLayerTissueROfRhoDetector(),
                PointSourceMultiLayerMomentumTransferDetectors(),
                PointSourceSingleVoxelTissueROfXAndYAndFluenceOfXAndYAndZDetector(),
                PointSourceThreeLayerReflectedTimeOfRhoAndSubregionHistDetector(),
                EmbeddedDirectionalCircularSourceEllipTissueFluenceOfXAndYAndZ(),
                PointSourceSurfaceFiberTissueAndDetector(),
                FluorescenceEmissionAOfXAndYAndZSourceInfiniteCylinder(),
                PointSourceBoundedTissue(),
                ImageSourceOneLayerTissueROfXAndYDetector()
            };
        }

        #region point source one layer tissue all detectors
        /// <summary>
        /// Point source, single tissue layer definition, all detectors included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceOneLayerTissueAllDetectors()
        {
            return new SimulationInput(
                100,
                "one_layer_all_detectors",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    // units space[mm], time[ns], temporal-freq[GHz], abs./scat. coeff[/mm]    
                    new AOfRhoAndZDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                    new AOfXAndYAndZDetectorInput {X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},                
                    new ATotalDetectorInput(),      
                    new FluenceOfRhoAndZAndTimeDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Time= new DoubleRange(0.0, 10, 101)},              
                    new FluenceOfRhoAndZDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                    new FluenceOfXAndYAndZDetectorInput {X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},
                    new FluenceOfXAndYAndZAndOmegaDetectorInput {X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)},
                    new FluenceOfXAndYAndZAndTimeDetectorInput {X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101),Time=new DoubleRange(0.0, 1, 21)},
                    new FluenceOfXAndYAndZAndStartingXAndYDetectorInput
                    {
                        X =new DoubleRange(-10, 10, 5),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 11),
                        StartingX=new DoubleRange(-1, 1, 2),StartingY=new DoubleRange(-10,10,2)},
                    new FluenceOfRhoAndZAndOmegaDetectorInput {Rho=new DoubleRange(0, 10, 101),Z=new DoubleRange(0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)},
                    new FluenceOfFxAndZDetectorInput {Fx=new DoubleRange(0, 0.5, 51),Z=new DoubleRange(0, 10, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 5)},
                    new RadianceOfFxAndZAndAngleDetectorInput {Fx=new DoubleRange(0.0, 0.5, 51),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 5)},
                    new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput
                    {
                        X=new DoubleRange(-10.0, 10.0, 101),
                        Y= new DoubleRange(-10.0, 10.0, 101),
                        Z= new DoubleRange(0.0, 10.0, 101), 
                        Theta=new DoubleRange(0.0, Math.PI, 5), // theta (polar angle)
                        Phi=new DoubleRange(-Math.PI, Math.PI, 5)}, // phi (azimuthal angle)
                    new RadianceOfRhoAtZDetectorInput {Rho = new DoubleRange(0.0, 10, 101), ZDepth = 3},
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput {Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},
                    new ROfFxAndTimeDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), Time= new DoubleRange(0.0, 10, 11)},
                    new ROfFxDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51)},
                    new ROfFxAndAngleDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), Angle= new DoubleRange(Math.PI / 2, Math.PI, 5)},
                    new ROfFxAndMaxDepthDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51), MaxDepth= new DoubleRange(0.0, 10, 11)},
                    new ROfRhoAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},             
                    new ROfRhoAndOmegaDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)}, // GHz
                    new ROfRhoAndTimeDetectorInput {Rho= new DoubleRange(0.0, 10, 101),Time=new DoubleRange(0.0, 10, 11)},
                    new ROfRhoAndMaxDepthDetectorInput {Rho= new DoubleRange(0.0, 10, 101),MaxDepth=new DoubleRange(0.0, 10, 11)},
                    new ROfRhoAndMaxDepthRecessedDetectorInput {Rho= new DoubleRange(0.0, 10, 101),MaxDepth=new DoubleRange(0.0, 10, 11),ZPlane=-1.0},
                    new ROfRhoDetectorInput {Rho =new DoubleRange(0.0, 10, 101)},
                    new ROfRhoRecessedDetectorInput {Rho =new DoubleRange(0.0, 10, 101),ZPlane=-1.0},
                    new ROfXAndYDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)},
                    new ROfXAndYRecessedDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),ZPlane=-1.0},
                    new ROfXAndYAndTimeDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),Time=new DoubleRange(0.0, 1, 11)},
                    new ROfXAndYAndTimeRecessedDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),Time=new DoubleRange(0.0, 1, 11),ZPlane=-1.0},
                    new ROfXAndYAndTimeAndSubregionDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),Time=new DoubleRange(0.0, 1, 11)},
                    new ROfXAndYAndTimeAndSubregionRecessedDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),Time=new DoubleRange(0.0, 1, 11),ZPlane=-1.0},
                    new ROfXAndYAndThetaAndPhiDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Theta = new DoubleRange(Math.PI / 2, Math.PI, 3),
                        Phi = new DoubleRange(-Math.PI, Math.PI, 5)
                    },
                    new ROfXAndYAndMaxDepthDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),MaxDepth=new DoubleRange(0.0, 10, 11)},
                    new ROfXAndYAndMaxDepthRecessedDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),MaxDepth=new DoubleRange(0.0, 10, 11),ZPlane=-1.0},
                    new RSpecularDetectorInput(),
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput {Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                    new TOfRhoAndAngleDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                    new TOfRhoDetectorInput {Rho=new DoubleRange(0.0, 10, 101)},
                    new TOfXAndYDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)},
                    new TOfXAndYAndTimeAndSubregionDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21),Time=new DoubleRange(0.0, 1, 11)},
                    new TOfFxDetectorInput {Fx = new DoubleRange(0.0, 0.5, 51)},
                }
            );
        }
        #endregion

        #region point source one layer R(rho) and Fluence(rho) (for lab exercises)
        /// <summary>
        /// Point source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceOneLayerTissueROfRhoAndFluenceOfRhoAndZDetectors()
        {
            return new SimulationInput(
                100,
                "one_layer_ROfRho_FluenceOfRhoAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput { Rho =new DoubleRange(0.0, 10, 101), FinalTissueRegionIndex=0, NA=1.0},
                    new FluenceOfRhoAndZDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)}
                }
             );
        }
        #endregion

        #region point source one layer Fluence(rho, z) and Radiance(rho, z, angle) (for lab exercises)
        // THIS IS USED FOR THE VP LABS, PLEASE DO NOT DELETE
        /// <summary>
        /// Point source, single tissue layer definition, Radiance included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceOneLayerTissueRadianceOfRhoAndZAndAngleDetector()
        {
            return new SimulationInput(
                100,  // set to 10000 for lab exercises
                "one_layer_FluenceOfRhoAndZ_RadianceOfRhoAndZAndAngle",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new FluenceOfRhoAndZDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z= new DoubleRange(0.0, 10, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Z=new DoubleRange(0.0, 10, 101),
                        Angle= new DoubleRange(0, Math.PI, 3)
                    }
                }
             );
        }
        #endregion

        #region point source two layer R(rho) (for lab exercises)
        /// <summary>
        /// Point source, two-layer tissue definition, only ROfRho detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceTwoLayerTissueROfRhoDetector()
        {
            return new SimulationInput(
                100,
                "two_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 1.5),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(1.5, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region point source two layer R(rho) and T(rho) with photon database
        /// <summary>
        /// Point source, two-layer tissue definition, only ROfRho detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceTwoLayerTissueROfRhoTOfRhoDetectorWithPhotonDatabase()
        {
            return new SimulationInput(
                100,
                "two_layer_ROfRho_TOfRho_with_databases",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new [] { DatabaseType.DiffuseReflectance, DatabaseType.DiffuseTransmittance }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 1.5),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(1.5, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                    new TOfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101),
                        FinalTissueRegionIndex = 3
                    },
                }
            );
        }
        #endregion

        #region point source single ellipsoid Fluence(rho,z)
        /// <summary>
        /// Point source, single ellipsoid tissue definition, only FluenceOfRhoAndZ detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceSingleEllipsoidTissueFluenceOfRhoAndZDetector()
        {
            return new SimulationInput(
                100,
                "ellip_FluenceOfRhoAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 1),
                        0.5,
                        0.5,
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new FluenceOfRhoAndZDetectorInput {Rho=new DoubleRange(0.0, 10, 101),Z= new DoubleRange(0.0, 10, 101)}
                }
            );
        }
        #endregion

        #region point source single infinite cylinder A(x,y,z)
        /// <summary>
        /// Point source, single infinite cylinder tissue definition, only AOfXAndYAndZ detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceSingleInfiniteCylinderTissueAOfXAndYAndZDetector()
        {
            return new SimulationInput(
                100,
                "infinite_cylinder_AOfXAndYAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new SingleInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 2),
                        1.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.0)
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {// need to specify at least 2 bins in each dimension if generating fluorescence source
                    new AOfXAndYAndZDetectorInput 
                    {
                        X =new DoubleRange(-10, 10, 201),
                        Y =new DoubleRange(-10, 10, 5), 
                        Z =new DoubleRange(0, 10, 101)}
                }
            );
        }
        #endregion

        #region point source multi infinite cylinder A(x,y,z)
        /// <summary>
        /// Point source, multi infinite cylinder tissue definition, only AOfXAndYAndZ detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceMultiInfiniteCylinderTissueAOfXAndYAndZDetector()
        {
            return new SimulationInput(
                100,
                "multi_infinite_cylinder_AOfXAndYAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiConcentricInfiniteCylinderTissueInput(
new ITissueRegion[]
                    {
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1),
                            0.75,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                    },
        new ITissueRegion[]
                    {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput
                    {
                        X =new DoubleRange(-10, 10, 201),
                        Y =new DoubleRange(-10, 10, 2),
                        Z =new DoubleRange(0, 10, 101)},
                }
            );
        }
        #endregion

        #region pMC point source one layer tissue R(rho) DAW part of website documentation so don't modify
        /// <summary>
        /// Perturbation MC point source, single tissue layer definition, R(rho) included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput pMCPointSourceOneLayerTissueROfRhoDAW()
        {
            return new SimulationInput(
                100,
                "pMC_one_layer_ROfRho_DAW",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { DatabaseType.pMCDiffuseReflectance }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region Gaussian 2D source one layer R(rho)
        /// <summary>
        /// Gaussian 2D source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput Gaussian2DSourceOneLayerTissueROfRhoDetector()
        {
            return new SimulationInput(
                100,
                "Gaussian_2D_source_one_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new CustomCircularSourceInput(
                    3.0, // outer radius
                    0.0, // inner radius
                    new GaussianSourceProfile(1.0), // fwhm
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange (0.0, 0.0), // azimuthal angle emission range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0,0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
             );
        }
        #endregion

        #region Flat 2D source two layer bounded tissue A(rho,z)
        /// <summary>
        /// Flat 2D source, two layer, bounded tissue, AOfRhoAndZ detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput Flat2DSourceTwoLayerBoundedTissueAOfRhoAndZDetector()
        {
            return new SimulationInput(
                100,
                "Flat_2D_source_two_layer_bounded_AOfRhoAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0), 
                new CustomCircularSourceInput(
                    0.1, // outer radius
                    0.0, // inner radius
                    new FlatSourceProfile(),
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emission range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                new BoundingCylinderTissueInput(
                    new CaplessCylinderTissueRegion(
                        new Position(0, 0, 50.0),
                        1.0,
                        100.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new AOfRhoAndZDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101),
                        Z = new DoubleRange(0, 100, 101)
                    },
                    new ATotalBoundingVolumeDetectorInput(),
                }
             );
        }
        #endregion

        #region Flat 2D source one layer R(rho)
        /// <summary>
        /// Flat 2D source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput Flat2DSourceOneLayerTissueROfRhoDetector()
        {
            return new SimulationInput(
                100,
                "Flat_2D_source_one_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new CustomCircularSourceInput(
                    3.0, // outer radius
                    0.0, // inner radius
                    new FlatSourceProfile(),
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emission range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
             );
        }
        #endregion

        #region Gaussian line source one layer R(rho)
        /// <summary>
        /// Gaussian line source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput GaussianLineSourceOneLayerTissueROfRhoDetector()
        {
            return new SimulationInput(
                100,
                "Gaussian_line_source_one_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new CustomLineSourceInput(
                    3.0, // line length
                    new GaussianSourceProfile(1.0), // fwhm
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emission range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
             );
        }
        #endregion

        #region point source multilayer momentum transfer
        /// <summary>
        /// Point source, multi-layer tissue definition, all momentum detectors detectors included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceMultiLayerMomentumTransferDetectors()
        {
            return new SimulationInput(
                100,
                "two_layer_momentum_transfer_detectors",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    true, // track statistics
                    0.00, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue, start in tissue so no MT tally at tissue crossing in air
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 1.0), // upper layer 1mm
                            new OpticalProperties(0.01, 1.0, 0.7, 1.33)), // Tyler's data
                        new LayerTissueRegion(
                            new DoubleRange(1.0, 10.0), // modify tissue thickness to 10mm to test transmitted MT detector
                            new OpticalProperties(0.01, 1.0, 0.7, 1.33)), 
                        new LayerTissueRegion(
                            new DoubleRange(10.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    // detectors with cylindrical symmetry
                    new ROfRhoDetectorInput{Rho =new DoubleRange(0.0, 10, 101) },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    new TOfRhoDetectorInput {Rho =new DoubleRange(0.0, 10, 101)},
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    // detectors with Cartesian coordinates                      
                    new ROfXAndYDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)},
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput
                    {
                        X=new DoubleRange(-100.0, 100.0, 21), 
                        Y= new DoubleRange(-100.0, 100.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    new TOfXAndYDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput
                    {
                        X=new DoubleRange(-100.0, 100.0, 21), 
                        Y= new DoubleRange(-100.0, 100.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    // DYNAMIC MT detectors
                    // detectors with cylindrical symmetry
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins                
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins                        
                        BloodVolumeFraction = new List<double> { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins                
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins
                        BloodVolumeFraction = new List<double> { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    // detectors with Cartesian coordinates                      
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput
                    {
                        X=new DoubleRange(-10.0, 10.0, 21), 
                        Y= new DoubleRange(-10.0, 10.0, 21),                 
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins
                        BloodVolumeFraction = new List<double> { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput
                    {
                        X=new DoubleRange(-10.0, 10.0, 21), 
                        Y= new DoubleRange(-10.0, 10.0, 21),             
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins
                        BloodVolumeFraction = new List<double> { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    // SFD detectors
                    new ReflectedDynamicMTOfFxAndSubregionHistDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11), // fx bins                
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins                        
                        BloodVolumeFraction = new List<double> { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    new TransmittedDynamicMTOfFxAndSubregionHistDetectorInput
                    {
                        Fx=new DoubleRange(0.0, 0.5, 11), // fx bins                
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins                        
                        BloodVolumeFraction = new List<double> { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                }
            );
        }
        #endregion

        #region point source single voxel Fluence(x,y,z)
        /// <summary>
        /// Point source, single voxel tissue definition, only FluenceOfXAndYAndZ detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceSingleVoxelTissueROfXAndYAndFluenceOfXAndYAndZDetector()
        {
            return new SimulationInput(
                100,
                "voxel_ROfXAndY_FluenceOfXAndYAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new SingleVoxelTissueInput(
                    new VoxelTissueRegion(
                        new DoubleRange(-1, 1),
                        new DoubleRange(-1, 1), 
                        new DoubleRange(1, 2), 
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                { 
                    new ROfXAndYDetectorInput {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                    new FluenceOfXAndYAndZDetectorInput
                    {X=new DoubleRange(-10, 10, 101),
                        Y=new DoubleRange(-10, 10, 101), Z= new DoubleRange(0.0, 10, 101)}
                }
            );
        }
        #endregion

        #region point source three layer SubRegion Time
        /// <summary>
        /// Point source, three-layer tissue definition, with R(rho,time) and
        /// ReflectedTimeOfRhoAndSubregionHistDetector detector included
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceThreeLayerReflectedTimeOfRhoAndSubregionHistDetector()
        {
            return new SimulationInput(
                100,
                "three_layer_ReflectedTimeOfRhoAndSubregionHist",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue, start in tissue so no MT tally at tissue crossing in air
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 5.0),
                            new OpticalProperties(0.01, 1.0, 0.9, 1.4)),  
                        new LayerTissueRegion(
                            new DoubleRange(5.0, 10.0),
                            new OpticalProperties(0.01, 1.0, 0.9, 1.4)), 
                        new LayerTissueRegion(
                            new DoubleRange(10.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.9, 1.4)), 
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfRhoAndTimeDetectorInput
                    {
                          Rho=new DoubleRange(0.0, 10.0, 21), // rho bins
                          Time=new DoubleRange(0.0, 1.0, 11)},  // time bins
                    new ReflectedTimeOfRhoAndSubregionHistDetectorInput
                    {
                          Rho=new DoubleRange(0.0, 10.0, 21), // rho bins
                          Time=new DoubleRange(0.0, 1.0, 11)} // time bins
                }
            );
        }
        #endregion

        #region directional circular source embedded in tissue pointed downward
        /// <summary>
        /// Directional circular source, converging on tissue with embedded ellipse
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput EmbeddedDirectionalCircularSourceEllipTissueFluenceOfXAndYAndZ()
        {
            return new SimulationInput(
                100,
                "embedded_directional_circular_source_ellip_tissue",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalCircularSourceInput(
                    0.7071, // < 0: Converging beam, > 0: Diverging beam, = 0: Collimated beam
                    0.2, // outer radius in mm
                    0, // inner radius
                    new FlatSourceProfile(), // flat beam
                    new Direction(0, 0, 1), // direction of principal source axis 
                    new Position(0.0, 0.0, 5.0), // translation from origin
                    new PolarAzimuthalAngles(0.0, 0.0), // beam rotation from inward normal
                    1), // 0=start in air, 1=start in tissue
                new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 7),
                        5,
                        0.5,
                        0.5,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1e-5, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    // units space[mm], time[ns], temporal-freq[GHz], abs./scat. coeff[/mm]    
                    new FluenceOfXAndYAndZDetectorInput {X=new DoubleRange(-5, 5, 100),Y=new DoubleRange(-5, 5, 100),Z=new DoubleRange(0, 10, 101)},
                }
            );
        }
        #endregion

        #region point source one layer Surface Fiber Detector 

        /// <summary>
        /// Point source on multilayer with surface fiber tissue and surface
        /// fiber detector
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceSurfaceFiberTissueAndDetector()
        {
            return new SimulationInput(
                100,
                "surface_fiber_detector",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1), // 0=start in air, 1=start in tissue
                new MultiLayerWithSurfaceFiberTissueInput(
                    new SurfaceFiberTissueRegion(
                        new Position(0, 0, 0),
                        0.3, // needs to match SurfaceFiberDetectorInput
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                { 
                    new SurfaceFiberDetectorInput
                    {
                        Center = new Position(0, 0, 0),
                        Radius = 0.3,
                        N = 1.4,
                        NA = double.PositiveInfinity,
                        Name = "SurfaceFiber_Open",
                        FinalTissueRegionIndex = 3,
                        TallySecondMoment = false
                    },
                    new SurfaceFiberDetectorInput
                    {
                        Center = new Position(0, 0, 0),
                        Radius = 0.3,
                        N = 1.4,
                        NA = 0.22,
                        Name = "SurfaceFiber_NA0p22",
                        FinalTissueRegionIndex = 3,
                        TallySecondMoment = true
                    },
                    new SurfaceFiberDetectorInput
                    {
                        Center = new Position(0, 0, 0),
                        Radius = 0.3,
                        N = 1.4,
                        NA = 0.39,
                        Name = "SurfaceFiber_NA0p39",
                        FinalTissueRegionIndex = 3,
                        TallySecondMoment = true
                    }
                }
            );
        }
        #endregion

        #region fluorescence emission source based on AOfXAndYAndZ of prior simulation this pairs 
        /// <summary>
        /// Fluorescence emission source using A(x,y,z) from prior simulation
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput FluorescenceEmissionAOfXAndYAndZSourceInfiniteCylinder()
        {
            return new SimulationInput(
                100,
                "fluorescence_emission_AOfXAndYAndZ_source_infinite_cylinder",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType> { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                // pairs w above infinite_cylinder_ROfRho_FluenceOfRhoAndZ
                new FluorescenceEmissionAOfXAndYAndZSourceInput(
                    "infinite_cylinder_AOfXAndYAndZ",
                    "infinite_cylinder_AOfXAndYAndZ.txt",
                    3,
                    SourcePositionSamplingType.CDF
                    ),
                new SingleInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        1.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 10, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new ROfXAndYDetectorInput
                    {X=new DoubleRange(-10, 10, 101),
                        Y = new DoubleRange(-100.0, 100.0, 2)},
                }
            );
        }
        #endregion
        
        #region tissue bounded by voxel region  
        /// <summary>
        /// Bounded Cylinder Tissue
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput PointSourceBoundedTissue()
        {
            return new SimulationInput(
                100,
                "point_source_bounded_tissue",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new BoundingVoxelTissueInput(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-1, 1, 2),
                        new DoubleRange(-1, 1, 2),
                        new DoubleRange(0, 100.0),
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 10, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ATotalBoundingVolumeDetectorInput() 
                }
            );
        }
        #endregion

        #region image source one layer 
        /// <summary>
        /// Bounded Cylinder Tissue
        /// </summary>
        /// <returns>An instance of the SimulationInput class</returns>
        public static SimulationInput ImageSourceOneLayerTissueROfXAndYDetector()
        {
            return new SimulationInput(
                100,
                "image_source_one_layer_tissue_ROfXAndY",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalImageSourceInput(
                    "folder",             // image folder
                    "image.png",               // image name
                    1280,        // x-axis number of pixels
                    1024,        // y-axis number of pixels
                    0.1,            // pixel size x-axis
                    0.1,           // pixel size y-axis
                    0.0,          // conv or diverge
                    new Direction(0,0,1),  
                    new Position(0.0, 0.0, 0.0),
                    new PolarAzimuthalAngles(),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfXAndYDetectorInput()
                }
            );
        }
        #endregion
    }
}
