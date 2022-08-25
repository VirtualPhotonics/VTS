using System.Runtime.CompilerServices;

namespace Vts.MonteCarlo.DataStructuresValidation
{
    /// <summary>
    /// The <see cref="DataStructuresValidation"/> namespace contains the Monte Carlo validation classes for the Monte Carlo inputs
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Class that holds the result of validating the Monte Carlo inputs.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Class constructor to capture results of validating the MC Simulation inputs
        /// </summary>
        /// <param name="isValid">Boolean indicating whether input is valid</param>
        /// <param name="validationRule">rule governing whether valid or not</param>
        /// <param name="remarks">possible way to correct input to become valid</param>
        public ValidationResult(bool isValid, string validationRule, string remarks)
        {
            IsValid = isValid;
            ValidationRule = validationRule;
            Remarks = remarks;
        }
        /// <summary>
        /// Overload of constructor omitting remarks
        /// </summary>
        /// <param name="isValid">Boolean indicating whether input is valid</param>
        /// <param name="validationRule">Rule governing whether valid or not</param>
        public ValidationResult(bool isValid, string validationRule) 
            : this(isValid, validationRule, "")
        {
        }
        /// <summary>
        /// Default constructor
        /// </summary>
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
