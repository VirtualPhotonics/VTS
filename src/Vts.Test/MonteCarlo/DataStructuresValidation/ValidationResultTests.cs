using System;
using NUnit.Framework;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Helpers;
    
namespace Vts.Test.MonteCarlo.DataStructuresValidation
{
    [TestFixture]
    public class ValidationResultTests
    {
        /// <summary>
        /// Validate default and fully-defined constructor
        /// </summary>
        [Test]
        public void Validate_constructor_results()
        {
            // default constructor
            var validationResult = new ValidationResult();
            Assert.IsInstanceOf<ValidationResult>(validationResult);
            // fully defined
            validationResult = new ValidationResult()
            {
                IsValid = false,
                ValidationRule = "validationRule",
                Remarks = "remarks"
            };
            Assert.IsInstanceOf<ValidationResult>(validationResult);
        }

    }
}

