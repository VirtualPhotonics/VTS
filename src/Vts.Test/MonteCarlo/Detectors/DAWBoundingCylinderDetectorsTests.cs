using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests execute a discrete absorption weighting (DAW) MC simulation with 
    /// one layer tissue bounded by a cylinder and validate against prior run results
    /// </summary>
    [TestFixture]
    public class DAWBoundingCylinderDetectorsTests
    {
        private SimulationOutput _outputBoundedTissue;
        private SimulationInput _inputBoundedTissue;

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
        /// Setup input to the MC for a homogeneous one layer tissue and a bounding
        /// cylinder tissue (both regions have same optical properties), execute simulations
        /// and verify results agree.
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

            var cylinderRadius = 5.0;
            var tissueThickness = 2.0;

            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0, // reproducible
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                new List<DatabaseType>() { }, // databases to be written
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            var source = new CustomCircularSourceInput(
                cylinderRadius - 0.01, // outer radius
                0.0, // inner radius
                new FlatSourceProfile(),
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emission range
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
            // debug with point source
            //var source = new DirectionalPointSourceInput(
            //    new Position(0.0, 0.0, 0.0),
            //    new Direction(0.0, 0.0, 1.0),
            //    //new Direction(1/Math.Sqrt(2), 0.0, 1/Math.Sqrt(2)),// debug with 45 degree direction and g=1.0
            //    1); // start inside tissue
            var ti = new BoundingCylinderTissueInput(
                    new CaplessCylinderTissueRegion(
                        new Position(0, 0, tissueThickness / 2),
                        cylinderRadius,
                        tissueThickness,
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.4),
                        "HenyeyGreensteinKey5"
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey1"),
                        new LayerTissueRegion( // note two layer and one layer give same results Yay!
                            new DoubleRange(0.0, 1.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            "HenyeyGreensteinKey2"), // debug g=1.0
                        new LayerTissueRegion(
                            new DoubleRange(1.0, tissueThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            "HenyeyGreensteinKey3"), // debug g=1.0
                        new LayerTissueRegion(
                            new DoubleRange(tissueThickness, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey4")
                    }
                    );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey5", new HenyeyGreensteinPhaseFunctionInput());

            var detectors = 
                new List<IDetectorInput>  
                {
                    new RSpecularDetectorInput(),
                    new RDiffuseDetectorInput(), new ROfRhoDetectorInput() {Rho=new DoubleRange(0.0, cylinderRadius, 11)}, 
                    new TDiffuseDetectorInput(),
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, cylinderRadius, 11)},
                    new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, cylinderRadius, 11),
                        Z=new DoubleRange(0, tissueThickness, 11)},
                    new ATotalDetectorInput(),
                    new ATotalBoundingVolumeDetectorInput() { TallySecondMoment = true }
                };

            _inputBoundedTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                ti,
                detectors);
            _outputBoundedTissue = new MonteCarloSimulation(_inputBoundedTissue).Run();
        }

        // Diffuse Reflectance
        [Test]
        public void validate_DAW_boundingcylinder_RDiffuse()
        {
              Assert.That(Math.Abs(_outputBoundedTissue.Rd - 0.238231), Is.LessThan(0.000001));
        }
        // Reflection R(rho)
        [Test]
        public void validate_DAW_boundingcylinder_ROfRho()
        {
             Assert.That(Math.Abs(_outputBoundedTissue.R_r[0] - 0.0), Is.LessThan(0.12381000001));
        }
        // Diffuse Transmittance
        [Test]
        public void validate_DAW_boundingcylinder_TDiffuse()
        {
             Assert.That(Math.Abs(_outputBoundedTissue.Td - 0.256878), Is.LessThan(0.000001));
        }
        // Transmittance T(rho)
        [Test]
        public void validate_DAW_boundingcylinder_TOfRho()
        {
            Assert.That(Math.Abs(_outputBoundedTissue.T_r[1] - 0.003941), Is.LessThan(0.000001));
        }
        // Total Absorption
        [Test]
        public void validate_DAW_boundingcylinder_ATotal()
        {
            Assert.That(Math.Abs(_outputBoundedTissue.Atot - 0.047790), Is.LessThan(0.000001));
        }
        // Total Absorption in Bounding Volume
        [Test]
        public void validate_DAW_boundingcylinder_ATotalBoundingCylinder()
        {
            Assert.That(Math.Abs(_outputBoundedTissue.AtotBV - 0.427099), Is.LessThan(0.000001));
            Assert.That(Math.Abs(_outputBoundedTissue.AtotBV2 - 0.405981), Is.LessThan(0.000001));
        }
        // Absorption(x,y,z)
        [Test]
        public void validate_DAW_boundingcylinder_AOfRhoAndZ()
        {
            Assert.That(Math.Abs(_outputBoundedTissue.A_rz[0, 0] - 0.000746), Is.LessThan(0.000001));
        }
        // sanity checks
        [Test]
        public void validate_DAW_boundingcylinder_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // add specular because photons started outside tissue
            Assert.That(Math.Abs(_outputBoundedTissue.Rd + _outputBoundedTissue.Atot + _outputBoundedTissue.Rspec +
                                    _outputBoundedTissue.AtotBV + _outputBoundedTissue.Td - 1), Is.LessThan(0.000001));
        }

    }
}
