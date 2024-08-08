using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation
{
    [TestFixture]
    public class SimulationInputValidationTests
    {
        /// <summary>
        /// Test to verify input with no detectors nor database specified is invalid
        /// </summary>
        [Test]
        public void Validate_null_detector_input_is_invalid_when_no_database_specified()
        {
            // generate input without any detector inputs and no database specified
            var input = new SimulationInput()  // default constructor has empty list of databases
            {
                DetectorInputs = new List<IDetectorInput>()
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }

        /// <summary>
        /// Test to verify input with database specified and no detectors specified is valid
        /// </summary>
        [Test]
        public void Validate_null_detector_input_is_valid_when_database_specified()
        {
            // generate input without any detector inputs but with database specified
            var input = new SimulationInput
            {
                DetectorInputs = new List<IDetectorInput>(),
                Options =
                {
                    Databases = new List<DatabaseType> {DatabaseType.DiffuseReflectance}
                }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Test to verify input with detector, Radiance(x,y,z,theta,phi) and CAW is invalid
        /// because not implemented
        /// </summary>
        [Test]
        public void Validate_detector_input_not_implemented_is_invalid()
        {
            // generate input with detector input not implemented yet
            var input = new SimulationInput
            {
                Options = new SimulationOptions { AbsorptionWeightingType = AbsorptionWeightingType.Continuous},
                DetectorInputs = new List<IDetectorInput> { new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput()}
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }

        /// <summary>
        /// Test to verify that input with two detectors with the same Name is invalid
        /// </summary>
        [Test]
        public void Validate_duplicate_detector_name_is_invalid()
        {
            // generate input with detector input with duplicate names
            var input = new SimulationInput
            {
                 DetectorInputs = new List<IDetectorInput> { 
                     new ROfRhoDetectorInput {Name = "ROfRho1"},
                     new ROfRhoDetectorInput {Name = "ROfRho1"}
                 }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }

        /// <summary>
        /// Test to verify input with cylindrical detector and off axis ellipsoid in tissue outputs warning
        /// but continues as valid input
        /// </summary>
        [Test]
        public void Validate_ellipsoid_tissue_with_off_zaxis_center_and_cylindrical_detectors_issues_warning()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput
            {
                TissueInput = new SingleEllipsoidTissueInput
                {
                    EllipsoidRegion = new EllipsoidTissueRegion {Center = new Position(1, 1, 5)}
                },
                DetectorInputs = new List<IDetectorInput> {new ROfRhoDetectorInput()}
            };
            // set to catch Console output
            var output = new StringWriter();
            Console.SetOut(output);
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid); // only warning
            Assert.That(output.ToString(), Is.EqualTo("Warning: off center ellipsoid in tissue with cylindrical detector defined: user discretion advised\r\n"));
        }

        /// <summary>
        /// Test to verify input cylindrical detector and ellipsoid in tissue is invalid
        /// </summary>
        [Test]
        public void Validate_ellipsoid_tissue_without_cylindrical_symmetry_and_cylindrical_detectors_issues_warning()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput
            {
                TissueInput = new SingleEllipsoidTissueInput
                {
                    EllipsoidRegion = new EllipsoidTissueRegion { Dx = 1.0, Dy = 2.0 }
                },
                DetectorInputs = new List<IDetectorInput> { new ROfRhoDetectorInput() }
            };
            // set to catch Console output
            var output = new StringWriter();
            Console.SetOut(output);
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid); // only warning
            Assert.That(output.ToString(), Is.EqualTo("Warning: ellipsoid with Dx != Dy in tissue with cylindrical detector defined: user discretion advised\r\n"));
        }

        /// <summary>
        /// Test to verify input with angled source and cylindrical detectors outputs warning
        /// but continues as valid input
        /// </summary>
        [Test]
        public void Validate_angled_source_and_cylindrical_detectors_are_not_defined_together()
        {
            // generate input with angled source and cylindrical detector
            var input = new SimulationInput
            {
                SourceInput = new DirectionalPointSourceInput(
                    new Position(0,0,0), new Direction(1.0/Math.Sqrt(2), 0, 1.0/Math.Sqrt(2)),1),
                DetectorInputs = new List<IDetectorInput> { new ROfRhoDetectorInput() }
            };
            // set to catch Console output
            var output = new StringWriter();
            Console.SetOut(output);
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid); // only warning
            Assert.That(output.ToString(), Is.EqualTo("Warning: Angled source and cylindrical coordinate detector defined: user discretion advised\r\n"));
        }

        /// <summary>
        /// Test to verify input with ellipsoid in tissue and R(fx) detector outputs warning
        /// but continues as valid input
        /// </summary>
        [Test]
        public void Validate_ellipsoid_tissue_and_ROfFx_detectors_defined_together_issues_warning()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput
            {
                TissueInput = new SingleEllipsoidTissueInput
                {
                    EllipsoidRegion = new EllipsoidTissueRegion { Dx = 1.0, Dy = 2.0 }
                },
                DetectorInputs = new List<IDetectorInput> { new ROfFxDetectorInput() }
            };
            // set to catch Console output
            var output = new StringWriter();
            Console.SetOut(output);
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid); // only warning
            Assert.That(output.ToString(), Is.EqualTo("Warning: R(fx) theory assumes a homogeneous or layered tissue geometry: user discretion advised\r\n"));
        }

        /// <summary>
        /// Test to verify input with voxel in tissue and R(fx) detector outputs warning
        /// but continues as valid input
        /// </summary>
        [Test]
        public void Validate_voxel_tissue_and_ROfFx_detectors_defined_together_issues_warning()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput
            {
                TissueInput = new SingleVoxelTissueInput
                {
                    // make sure voxel defined so validation doesn't error on voxel geometry
                    VoxelRegion = new VoxelTissueRegion
                    {
                        X = new DoubleRange(-5,5,2),
                        Y = new DoubleRange(-5,5,2),
                        Z = new DoubleRange(1,6,2)
                    }
                },
                DetectorInputs = new List<IDetectorInput> { new ROfFxDetectorInput() }
            };
            // set to catch Console output
            var output = new StringWriter();
            Console.SetOut(output);
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid); // only warning
            Assert.That(output.ToString(), Is.EqualTo("Warning: R(fx) theory assumes a homogeneous or layered tissue geometry: user discretion advised\r\n"));

        }

        /// <summary>
        /// Test to verify input with negative optical properties is invalid
        /// </summary>
        [Test]
        public void Validate_tissue_optical_properties_are_non_negative()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput
            {
                TissueInput = new MultiLayerTissueInput(
                    new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(-1.0, 1.0, 0.8, 1.4)), // make mua negative
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                })
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
    }
}
