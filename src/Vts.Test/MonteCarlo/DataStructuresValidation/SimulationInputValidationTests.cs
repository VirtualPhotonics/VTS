using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation
{
    [TestFixture]
    public class SimulationInputValidationTests
    {
        [Test]
        public void validate_null_detector_input_is_invalid_when_no_database_specified()
        {
            // generate input without any detector inputs and no database specified
            var input = new SimulationInput()  // default constructor has empty list of databases
            {
                DetectorInputs = new List<IDetectorInput> {}
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_null_detector_input_is_valid_when_database_specified()
        {
            // generate input without any detector inputs but with database specified
            var input = new SimulationInput()  
            {
                DetectorInputs = new List<IDetectorInput> {}
            };
            input.Options.Databases = new List<DatabaseType> {DatabaseType.DiffuseReflectance};
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid);
        }
        [Test]
        public void validate_detector_input_not_implemented_is_invalid()
        {
            // generate input with detector input not implemented yet
            var input = new SimulationInput()
            {
                Options = new SimulationOptions() { AbsorptionWeightingType = AbsorptionWeightingType.Continuous},
                DetectorInputs = new List<IDetectorInput> { new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput()}
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_duplicate_detector_name_is_invalid()
        {
            // generate input with detector input with duplicate names
            var input = new SimulationInput()
            {
                 DetectorInputs = new List<IDetectorInput> { 
                     new ROfRhoDetectorInput() {Name = "ROfRho1"},
                     new ROfRhoDetectorInput() {Name = "ROfRho1"}
                 }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_ellipsoid_tissue_with_off_zaxis_center_and_cylindrical_detectors_are_not_defined_together()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput()
            {
                TissueInput = new SingleEllipsoidTissueInput()
                {
                    EllipsoidRegion = new EllipsoidTissueRegion() {Center = new Position(1, 1, 0)}
                },
                DetectorInputs = new List<IDetectorInput> {new ROfRhoDetectorInput()}
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_ellipsoid_tissue_without_cylindrical_symmetry_and_cylindrical_detectors_are_not_defined_together()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput()
            {
                TissueInput = new SingleEllipsoidTissueInput()
                {
                    EllipsoidRegion = new EllipsoidTissueRegion() { Dx = 1.0, Dy = 2.0 }
                },
                DetectorInputs = new List<IDetectorInput> { new ROfRhoDetectorInput() }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_angled_source_and_cylindrical_detectors_are_not_defined_together()
        {
            // generate input with angled source and cylindrical detector
            var input = new SimulationInput()
            {
                SourceInput = new DirectionalPointSourceInput(
                    new Position(0,0,0), new Direction(1.0/Math.Sqrt(2), 0, 1.0/Math.Sqrt(2)),1),
                DetectorInputs = new List<IDetectorInput> { new ROfRhoDetectorInput() }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_ellipsoid_tissue_and_ROfFx_detectors_are_not_defined_together()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput()
            {
                TissueInput = new SingleEllipsoidTissueInput()
                {
                    EllipsoidRegion = new EllipsoidTissueRegion() { Dx = 1.0, Dy = 2.0 }
                },
                DetectorInputs = new List<IDetectorInput> { new ROfFxDetectorInput() }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_voxel_tissue_and_ROfFx_detectors_are_not_defined_together()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput()
            {
                TissueInput = new SingleVoxelTissueInput()
                {
                    VoxelRegion = new VoxelTissueRegion()
                },
                DetectorInputs = new List<IDetectorInput> { new ROfFxDetectorInput() }
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        [Test]
        public void validate_tissue_optical_properties_are_non_negative()
        {
            // generate input embedded ellipsoid tissue and cylindrical detector
            var input = new SimulationInput()
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
