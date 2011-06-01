using System;

namespace Vts.MonteCarlo.DataStructuresValidation
{
    public class ValidationResult
    {
        public ValidationResult(bool isValid, string errorMessage, string remarks)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            Remarks = remarks;
        }
        public ValidationResult() : this(true, "", "") {}

        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string Remarks { get; set; }
    }
}
