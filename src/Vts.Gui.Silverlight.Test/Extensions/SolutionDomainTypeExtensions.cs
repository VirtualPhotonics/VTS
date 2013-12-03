using System.Collections.Generic;

namespace Vts.Gui.Silverlight.Test
{
    public static class SolutionDomainTypeExtensions
    {
        /// <summary>
        /// Returns an enumerable list of possible IndependentVariableAxis values for the given solution domain
        /// </summary>
        /// <param name="solutionDomainType"></param>
        /// <returns></returns>
        public static IEnumerable<IndependentVariableAxis> GetIndependentVariableAxes(this SolutionDomainType solutionDomainType)
        {
            switch (solutionDomainType)
            {
                case SolutionDomainType.ROfRho:
                default:
                    return new[] { IndependentVariableAxis.Rho };
                case SolutionDomainType.ROfRhoAndTime:
                    return new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time };
                case SolutionDomainType.ROfRhoAndFt:
                    return new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft };
                case SolutionDomainType.ROfFx:
                    return new[] { IndependentVariableAxis.Fx };
                case SolutionDomainType.ROfFxAndTime:
                    return new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time };
                case SolutionDomainType.ROfFxAndFt:
                    return new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft };
            }
        }
    }
}
