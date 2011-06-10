using System;

namespace Vts.MonteCarlo.DataStructuresValidation
{
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

        public bool IsValid { get; set; }
        public string ValidationRule { get; set; }
        public string Remarks { get; set; }
    }
}
