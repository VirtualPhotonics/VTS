using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
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
            return new List<SimulationInput>()
            {
                PointSourceOneLayerTissueAllDetectors(),
                PointSourceOneLayerTissueROfRhoAndFluenceOfRhoAndZDetectors(),
                PointSourceOneLayerTissueRadianceOfRhoAndZAndAngleDetector(),
                PointSourceTwoLayerTissueROfRhoDetector(),
                PointSourceTwoLayerTissueROfRhoDetectorWithPhotonDatabase(),
                PointSourceSingleEllipsoidTissueFluenceOfRhoAndZDetector(),
                pMCPointSourceOneLayerTissueROfRhoDAW(),
                GaussianSourceOneLayerTissueROfRhoDetector(),
                PointSourceMultiLayerReflectedMTOfRhoAndSubregionHistDetector(),
                PointSourceThreeLayerReflectedTimeOfRhoAndSubregionHistDetector()
            };
        }

        #region point source one layer tissue all detectors
        /// <summary>
        /// Point source, single tissue layer definition, all detectors included
        /// </summary>
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
                    new List<DatabaseType>() { }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    // units space[mm], time[ns], temporal-freq[GHz], abs./scat. coeff[/mm]
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput() {Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},
                    new ROfRhoDetectorInput() {Rho =new DoubleRange(0.0, 10, 101)},
                    new ROfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},
                    new ROfRhoAndTimeDetectorInput() {Rho= new DoubleRange(0.0, 10, 101),Time=new DoubleRange(0.0, 10, 101)},
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                    new ROfRhoAndOmegaDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)}, // GHz
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10, 101)},
                    new TOfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                    new ATotalDetectorInput(),
                    new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                    new AOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                    new FluenceOfRhoAndZAndTimeDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Time= new DoubleRange(0.0, 10, 101)},
                    new FluenceOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 3)},
                    new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput(){
                        X=new DoubleRange(-10.0, 10.0, 101),
                        Y= new DoubleRange(-10.0, 10.0, 101),
                        Z= new DoubleRange(0.0, 10.0, 101), 
                        Theta=new DoubleRange(0.0, Math.PI, 5), // theta (polar angle)
                        Phi=new DoubleRange(-Math.PI, Math.PI, 5)}, // phi (azimuthal angle)
                    new RSpecularDetectorInput()
                }
                );
        }
        #endregion

        #region point source one layer R(rho) and Fluence(rho) (for lab exercises)
        /// <summary>
        /// Point source, single tissue layer definition, only ROfRho detector included
        /// </summary>
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
                    new List<DatabaseType>() { }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput() { Rho =new DoubleRange(0.0, 10, 101)},
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)}
                }
             );
        }
        #endregion

        #region point source one layer Fluence(rho, z) and Radiance(rho, z, angle) 
        // THIS IS USED FOR THE VP LABS, PLEASE DO NOT DELETE
        /// <summary>
        /// Point source, single tissue layer definition, Radiance included
        /// </summary>
        public static SimulationInput PointSourceOneLayerTissueRadianceOfRhoAndZAndAngleDetector()
        {
            return new SimulationInput(
                10000,
                "one_layer_FluenceOfRhoAndZ_RadianceOfRhoAndZAndAngle",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z= new DoubleRange(0.0, 10, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput()
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
                    new List<DatabaseType>() { }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 1.5),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(1.5, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region point source two layer R(rho) with photon database
        /// <summary>
        /// Point source, two-layer tissue definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput PointSourceTwoLayerTissueROfRhoDetectorWithPhotonDatabase()
        {
            return new SimulationInput(
                100,
                "two_layer_ROfRho_with_db",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new [] { DatabaseType.DiffuseReflectance }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 1.5),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(1.5, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region point source single ellipsoid Fluence(rho)
        /// <summary>
        /// Point source, single ellipsoid tissue definition, only ROfRho detector included
        /// </summary>
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
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new SingleEllipsoidTissueInput(
                    new EllipsoidRegion(
                        new Position(0, 0, 1),
                        0.5,
                        0.5,
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new FluenceOfRhoAndZDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z= new DoubleRange(0.0, 10, 101)}
                }
            );
        }
        #endregion

        #region pMC point source one layer tissue R(rho) DAW
        /// <summary>
        /// Perturbation MC point source, single tissue layer definition, R(rho) included
        /// </summary>
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
                    new List<DatabaseType>() { DatabaseType.pMCDiffuseReflectance }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region Gaussian source one layer R(rho)
        /// <summary>
        /// Gaussian source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput GaussianSourceOneLayerTissueROfRhoDetector()
        {
            return new SimulationInput(
                100,
                "Gaussian_source_one_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new CustomCircularSourceInput(
                    3.0, // outer radius
                    0.0, // inner radius
                    new GaussianSourceProfile(1.0), // fwhm
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange (0.0, 0.0), // azimuthal angle emmision range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0,0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
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
        /// Point source, multi-layer tissue definition, only ReflectedMCOfRhoAndSubRegionHistDetector detector included
        /// </summary>
        public static SimulationInput PointSourceMultiLayerReflectedMTOfRhoAndSubregionHistDetector()
        {
            return new SimulationInput(
                100,
                "two_layer_ReflectedMTOfRhoAndSubregionHist",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.0001, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue, start in tissue so no MT tally at tissue crossing in air
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 1.0), // upper layer 1mm
                            new OpticalProperties(0.01, 1.0, 0.7, 1.33)), // Tyler's data
                        new LayerRegion(
                            new DoubleRange(1.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.7, 1.33)), 
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput(){
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)} // fractional MT bins
                }
            );
        }
        #endregion

        #region point source three layer SubRegion Time
        /// <summary>
        /// Point source, three-layer tissue definition, with R(rho,time) and
        /// ReflectedTimeOfRhoAndSubregionHistDetector detector included
        /// </summary>
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
                    new List<DatabaseType>() { }, // databases to be written
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
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 5.0),
                            new OpticalProperties(0.01, 1.0, 0.9, 1.4)),  
                        new LayerRegion(
                            new DoubleRange(5.0, 10.0),
                            new OpticalProperties(0.01, 1.0, 0.9, 1.4)), 
                        new LayerRegion(
                            new DoubleRange(10.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.9, 1.4)), 
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new ROfRhoAndTimeDetectorInput(){
                          Rho=new DoubleRange(0.0, 10.0, 21), // rho bins
                          Time=new DoubleRange(0.0, 1.0, 11)},  // time bins
                    new ReflectedTimeOfRhoAndSubregionHistDetectorInput(){
                          Rho=new DoubleRange(0.0, 10.0, 21), // rho bins
                          Time=new DoubleRange(0.0, 1.0, 11)} // time bins
                }
            );
        }
        #endregion
    }
}
