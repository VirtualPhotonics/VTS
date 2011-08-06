using System;

namespace Vts.MonteCarlo.DataStructuresValidation
{
    /// <summary>
    /// Class that holds the result of validating the Monte Carlo inputs.
    /// </summary>
    public class ValidationResult
    {
        public ValidationResult(bool isValid, string validationRule, string remarks)
        {
            IsValid = isValid;
            ValidationRule = validationRule;
            Remarks = remarks;
        }
        public ValidationResult(bool isValid, string validationRule) 
            : this(isValid, validationRule, "")
        {
        }
        public ValidationResult() 
            : this(false, "", "")
        {
        }
        /// <summary>
        /// Boolean indicating whether result is valid or not
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Rule that is being validated
        /// </summary>
        public string ValidationRule { get; set; }
        /// <summary>
        /// Helpful remarks to correct violation
        /// </summary>
        public string Remarks { get; set; }
    }
}
