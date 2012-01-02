using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.DataStructuresValidation
{
    [TestFixture]
    public class SimulationInputValidationTests
    {
        [Test]
        public void validate_null_detector_input_is_invalid()
        {
            // generate input without any detector inputs
            var input = new SimulationInput()
            {
                DetectorInputs = new List<IDetectorInput> {}
            };
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        //[Test]
        //public void validate_detector_input_not_implemented_is_invalid()
        //{
        //    // generate input with detector input not implemented yet
        //    var input = new SimulationInput()
        //    {
        //        DetectorInputs = new List<IDetectorInput> {}
        //    };
        //    var result = SimulationInputValidation.ValidateInput(input);
        //    Assert.IsFalse(result.IsValid);
        //}
    }
}
