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
                case SolutionDomainType.RofRho:
                default:
                    return new[] { IndependentVariableAxis.Rho };
                case SolutionDomainType.RofRhoAndT:
                    return new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.T };
                case SolutionDomainType.RofRhoAndFt:
                    return new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft };
                case SolutionDomainType.RofFx:
                    return new[] { IndependentVariableAxis.Fx };
                case SolutionDomainType.RofFxAndT:
                    return new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.T };
                case SolutionDomainType.RofFxAndFt:
                    return new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft };
            }
        }
    }
}
