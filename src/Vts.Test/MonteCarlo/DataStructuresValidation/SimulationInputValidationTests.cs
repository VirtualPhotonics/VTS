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
        /// <summary>
        /// Test to check that both DiffuseReflectance and DiffuseTransmittance VBs
        /// are defined within SimulationInput.  And if not, that the validation software
        /// passes back correct failure.
        /// </summary>
        //[Test]
        //public void validate_reflectance_and_transmittance_virtual_boundaries_defined()
        //{
        //    // generate input without a DiffuseTransmittance VB
        //    var input = new SimulationInput()
        //    {
        //        VirtualBoundaryInputs = new List<IVirtualBoundaryInput>
        //        {
        //            new SurfaceVirtualBoundaryInput(
        //                VirtualBoundaryType.DiffuseReflectance,
        //                new List<IDetectorInput>
        //                {
        //                    new RDiffuseDetectorInput(),
        //                },
        //                false,
        //                VirtualBoundaryType.DiffuseReflectance.ToString()
        //            ),
        //        }
        //    };
        //    var result = SimulationInputValidation.ValidateInput(input);
        //    Assert.IsFalse(result.IsValid);
        //}
    }
}
