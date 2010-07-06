using Vts.Common;

namespace Vts.SiteVisit.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsGaussianForwardModel(this ForwardSolverType forwardSolverType)
        {
            switch (forwardSolverType)
            {
                case ForwardSolverType.PointSDA:
                case ForwardSolverType.DistributedPointSDA:
                case ForwardSolverType.DeltaPOne:
                case ForwardSolverType.MonteCarlo:
                default:
                    return false;
                case ForwardSolverType.DistributedGaussianSDA:
                    return true;
            }
        }

        public static int GetMaxArgumentLocation(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return 0;
                case IndependentVariableAxis.T:
                    return 2;
                case IndependentVariableAxis.Fx:
                    return 0;
                case IndependentVariableAxis.Ft:
                    return 2;
                case IndependentVariableAxis.Z:
                    return 1;
            }
        }

        public static string GetUnits(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return IndependentVariableAxisUnits.MM.GetInternationalizedString();
                case IndependentVariableAxis.T:
                    return IndependentVariableAxisUnits.NS.GetInternationalizedString();
                case IndependentVariableAxis.Fx:
                    return IndependentVariableAxisUnits.InverseMM.GetInternationalizedString();
                case IndependentVariableAxis.Ft:
                    return IndependentVariableAxisUnits.GHz.GetInternationalizedString();
            }
        }

        public static string GetUnits(this SolutionDomainType sdType)
        {
            switch (sdType)
            {
                case SolutionDomainType.RofRho:
                default:
                    return DependentVariableAxisUnits.PerMMSquared.GetInternationalizedString();
                case SolutionDomainType.RofFx:
                    return DependentVariableAxisUnits.Unitless.GetInternationalizedString();
                case SolutionDomainType.RofRhoAndT:
                    return DependentVariableAxisUnits.PerMMSquaredPerNS.GetInternationalizedString();
                case SolutionDomainType.RofFxAndT:
                    return DependentVariableAxisUnits.PerNS.GetInternationalizedString();
                case SolutionDomainType.RofRhoAndFt:
                    return DependentVariableAxisUnits.PerMMSquaredPerGHz.GetInternationalizedString();
                case SolutionDomainType.RofFxAndFt:
                    return DependentVariableAxisUnits.PerGHz.GetInternationalizedString();
            }
        }

        public static string GetUnits(this FluenceSolutionDomainType sdType)
        {
            switch (sdType)
            {
                case FluenceSolutionDomainType.FluenceofRho:
                default:
                    return DependentVariableAxisUnits.PerMMCubed.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceofFx:
                    return DependentVariableAxisUnits.PerMM.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceofRhoAndT:
                    return DependentVariableAxisUnits.PerMMCubedPerNS.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceofFxAndT:
                    return DependentVariableAxisUnits.PerMMPerNS.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceofRhoAndFt:
                    return DependentVariableAxisUnits.PerMMCubedPerGHz.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceofFxAndFt:
                    return DependentVariableAxisUnits.PerMMPerGHz.GetInternationalizedString();
            }
        }
        public static DoubleRange GetDefaultRange(this IndependentVariableAxis independentAxisType)
        {
            switch (independentAxisType)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return new DoubleRange(0.5D, 9.5D, 19); // units=mm
                case IndependentVariableAxis.T:
                    return new DoubleRange(0D, 0.05D, 51);  // units=ns
                case IndependentVariableAxis.Fx:
                    return new DoubleRange(0D, 0.5D, 51);
                case IndependentVariableAxis.Ft:
                    return new DoubleRange(0D, 0.5D, 51); // units=GHz
                case IndependentVariableAxis.Wavelength:
                    return new DoubleRange(650D, 1000D, 176); //TODO: right units?
            }
        }

        public static double GetDefaultConstantAxisValue(this IndependentVariableAxis constantType)
        {
            switch (constantType)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return 1.0;
                case IndependentVariableAxis.T:
                    return 0.05;
                case IndependentVariableAxis.Fx:
                    return 0.0;
                case IndependentVariableAxis.Ft:
                    return 0.0;
                case IndependentVariableAxis.Wavelength:
                    return 650.0;
            }
        }


    }
}
