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
            Assert.That(validationResult, Is.InstanceOf<ValidationResult>());
            // fully defined
            validationResult = new ValidationResult()
            {
                IsValid = false,
                ValidationRule = "validationRule",
                Remarks = "remarks"
            };
            Assert.That(validationResult, Is.InstanceOf<ValidationResult>());
        }

    }
}

