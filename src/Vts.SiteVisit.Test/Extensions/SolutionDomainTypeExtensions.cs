using System.Collections.Generic;

namespace Vts.SiteVisit.Test
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
                case SolutionDomainType.ROfRhoAndT:
                    return new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.T };
                case SolutionDomainType.ROfRhoAndFt:
                    return new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft };
                case SolutionDomainType.ROfFx:
                    return new[] { IndependentVariableAxis.Fx };
                case SolutionDomainType.ROfFxAndT:
                    return new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.T };
                case SolutionDomainType.ROfFxAndFt:
                    return new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft };
            }
        }
    }
}
