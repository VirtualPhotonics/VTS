using System;
using System.Collections.Generic;
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
    /// These tests execute a discrete absorption weighting (DAW) MC simulation with 
    /// two concentric infinite cylinders
    /// </summary>
    [TestFixture]
    public class DAWMultiInfiniteCylinderDetectorsTests
    {
        private SimulationOutput _outputOneRegionTissue;
        private SimulationOutput _outputThreeRegionTissue;
        private SimulationInput _inputOneRegionTissue;
        private SimulationInput _inputThreeRegionTissue;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt", // file that captures screen output of MC simulation
        };

        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a two concentric
        /// infinite cylinder tissue (both regions have same optical properties), execute simulations
        /// and verify results agree.
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0, // reproducible
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
            var detectors = 
                new List<IDetectorInput>  // no cylindrical cylinders inconsistent with tissue geometry
                {
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput() {Angle=new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-20.0, 20.0, 41),
                        Y =new DoubleRange(-20.0, 20.0, 2)}, 
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new ATotalDetectorInput(),
                    new AOfXAndYAndZDetectorInput() {X=new DoubleRange(-20.0, 20.0, 41),
                        Y=new DoubleRange(-20.0, 20.0, 2),
                        Z=new DoubleRange(0, 20, 21)}
                };
            var oneRegionTissue = new MultiLayerTissueInput(
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
            oneRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            oneRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            oneRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());

            _inputOneRegionTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                oneRegionTissue,
                detectors);             
            _outputOneRegionTissue = new MonteCarloSimulation(_inputOneRegionTissue).Run();

            var threeRegionTissue = new MultiConcentricInfiniteCylinderTissueInput(
                new ITissueRegion[]
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.75,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4), //debug with g=1
                        "HenyeyGreensteinKey4"
                    ),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.5,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4), //debug with g=1
                        "HenyeyGreensteinKey5"
                    ),
                },
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 20.0), // debug with thin slab d=2
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4), // debug with g=1
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                }
            );
            threeRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            threeRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            threeRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            threeRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            threeRegionTissue.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey5", new HenyeyGreensteinPhaseFunctionInput());

            _inputThreeRegionTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                threeRegionTissue, 
                detectors);
            _outputThreeRegionTissue = new MonteCarloSimulation(_inputThreeRegionTissue).Run();
        }

        // Diffuse Reflectance
        [Test]
        public void validate_DAW_multiinfinitecylinder_RDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Rd - _outputThreeRegionTissue.Rd), 0.000001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_DAW_multiinfinitecylinder_ROfAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_a[0] - _outputThreeRegionTissue.R_a[0]), 0.000001);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_DAW_multiinfinitecylinder_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Td - _outputThreeRegionTissue.Td), 0.000001);
        }
        // Transmittance Time(angle)
        [Test]
        public void validate_DAW_multiinfinitecylinder_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.T_a[0] - _outputThreeRegionTissue.T_a[0]), 0.000001);
        }
        // Reflectance R(x,y)
        [Test]
        public void validate_DAW_multiinfinitecylinder_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_xy[10, 0] -
                                 _outputThreeRegionTissue.R_xy[10, 0]), 0.000001);
        }
        // Total Absorption
        [Test]
        public void validate_DAW_multiinfinitecylinder_ATotal()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Atot -_outputThreeRegionTissue.Atot), 0.000001);
        }
        // Absorption(x,y,z)
        [Test]
        public void validate_DAW_multiinfinitecylinder_AOfXAndYAndZ()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.A_xyz[20,0,0] - 
                                 _outputThreeRegionTissue.A_xyz[20,0,0]), 0.000001);
        }
        // sanity checks
        [Test]
        public void validate_DAW_multiinfinitecylinder_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.Less(Math.Abs(_outputThreeRegionTissue.Rd + _outputThreeRegionTissue.Atot + 
                                 _outputThreeRegionTissue.Td - 1), 0.00000000001);
        }
    }
}
