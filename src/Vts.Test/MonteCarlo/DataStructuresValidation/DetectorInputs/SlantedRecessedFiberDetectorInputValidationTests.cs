using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.DetectorInputs
{
    [TestFixture]
    public class SlantedRecessedFiberDetectorInputValidationTests
    {
        /// <summary>
        /// Test to check that detector defined on surface of tissue or above
        /// </summary>
        [Test]
        public void Validate_code_checks_that_detector_defined_on_tissue_surface()
        {
            var tissueInput = new MultiLayerTissueInput();
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                new List<IDetectorInput>()
                {
                    new SlantedRecessedFiberDetectorInput()
                    {
                        ZPlane = 10
                    }  
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }

        /// <summary>
        /// Test to check that detector radius is not negative or zero
        /// </summary>
        [Test]
        public void Validate_code_checks_that_detector_radius_is_nonzero()
        {
            var tissueInput = new MultiLayerTissueInput();
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                new List<IDetectorInput>()
                {
                    new SlantedRecessedFiberDetectorInput()
                    {
                        Radius = 0
                    }
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }

        /// <summary>
        /// Test to check that detector angle is not negative
        /// </summary>
        [Test]
        public void Validate_code_checks_that_detector_angle_is_not_negative()
        {
            var tissueInput = new MultiLayerTissueInput();
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                new List<IDetectorInput>()
                {
                    new SlantedRecessedFiberDetectorInput()
                    {
                        Angle = -Math.PI
                    }
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }

        /// <summary>
        /// Test to check that detector angle is not 90 or beyond
        /// </summary>
        [Test]
        public void Validate_code_checks_that_detector_angle_is_not_90_or_beyond()
        {
            var tissueInput = new MultiLayerTissueInput();
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                new List<IDetectorInput>()
                {
                    new SlantedRecessedFiberDetectorInput()
                    {
                        Angle = Math.PI/2.0
                    }
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
    }
}
