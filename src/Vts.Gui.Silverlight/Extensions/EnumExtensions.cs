using System;
using Vts.Common;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsGaussianForwardModel(this ForwardSolverType forwardSolverType)
        {
            switch (forwardSolverType)
            {
                case ForwardSolverType.PointSourceSDA:
                case ForwardSolverType.DistributedPointSourceSDA:
                case ForwardSolverType.DeltaPOne:
                case ForwardSolverType.MonteCarlo:
                default:
                    return false;
                case ForwardSolverType.DistributedGaussianSourceSDA:
                    return true;
            }
        }

        public static bool IsMultiRegionForwardModel(this ForwardSolverType forwardSolverType)
        {
            switch (forwardSolverType)
            {
                case ForwardSolverType.PointSourceSDA:
                case ForwardSolverType.DistributedPointSourceSDA:
                case ForwardSolverType.DistributedGaussianSourceSDA:
                case ForwardSolverType.DeltaPOne:
                case ForwardSolverType.MonteCarlo:
                default:
                    return false;
                case ForwardSolverType.TwoLayerSDA:
                    return true;
            }
        }

        public static int GetMaxArgumentLocation(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Rho:
                    return 0;
                case IndependentVariableAxis.Time:
                    return 2;
                case IndependentVariableAxis.Fx:
                    return 0;
                case IndependentVariableAxis.Ft:
                    return 2;
                case IndependentVariableAxis.Z:
                    return 1;
                case IndependentVariableAxis.Wavelength:
                    return 2;
                default:
                    throw new NotImplementedException("Independent axis " + axis + " is not implemented for this software feature.");
            }
        }

        public static bool IsSpatialAxis(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Rho:
                case IndependentVariableAxis.Fx:
                    return true;
                case IndependentVariableAxis.Time:
                case IndependentVariableAxis.Ft:
                case IndependentVariableAxis.Z:
                case IndependentVariableAxis.Wavelength:
                    return false;
                default:
                    throw new NotImplementedException("Independent axis " + axis + " is not implemented for this software feature.");
            }
        }

        public static bool IsTemporalAxis(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Time:
                case IndependentVariableAxis.Ft:
                    return true;
                case IndependentVariableAxis.Rho:
                case IndependentVariableAxis.Fx:
                case IndependentVariableAxis.Z:
                case IndependentVariableAxis.Wavelength:
                    return false;
                default:
                    throw new NotImplementedException("Independent axis " + axis + " is not implemented for this software feature.");
            }
        }

        public static bool IsDepthAxis(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Z:
                    return true;
                case IndependentVariableAxis.Time:
                case IndependentVariableAxis.Ft:
                case IndependentVariableAxis.Rho:
                case IndependentVariableAxis.Fx:
                case IndependentVariableAxis.Wavelength:
                    return false;
                default:
                    throw new NotImplementedException("Independent axis " + axis + " is not implemented for this software feature.");
            }
        }

        public static bool IsWavelengthAxis(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Wavelength:
                    return true;
                case IndependentVariableAxis.Time:
                case IndependentVariableAxis.Ft:
                case IndependentVariableAxis.Rho:
                case IndependentVariableAxis.Fx:
                case IndependentVariableAxis.Z:
                    return false;
                default:
                    throw new NotImplementedException("Independent axis " + axis + " is not implemented for this software feature.");
            }
        }
        
        public static string GetUnits(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return IndependentVariableAxisUnits.MM.GetInternationalizedString();
                case IndependentVariableAxis.Time:
                    return IndependentVariableAxisUnits.NS.GetInternationalizedString();
                case IndependentVariableAxis.Fx:
                    return IndependentVariableAxisUnits.InverseMM.GetInternationalizedString();
                case IndependentVariableAxis.Ft:
                    return IndependentVariableAxisUnits.GHz.GetInternationalizedString();
                case IndependentVariableAxis.Wavelength:
                    return IndependentVariableAxisUnits.NM.GetInternationalizedString();
            }
        }

        public static string GetTitle(this IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return IndependentVariableAxis.Rho.GetLocalizedString();
                case IndependentVariableAxis.Time:
                    return IndependentVariableAxis.Time.GetLocalizedString();
                case IndependentVariableAxis.Fx:
                    return IndependentVariableAxis.Fx.GetLocalizedString();
                case IndependentVariableAxis.Ft:
                    return IndependentVariableAxis.Ft.GetLocalizedString();
                case IndependentVariableAxis.Wavelength:
                    return IndependentVariableAxis.Wavelength.GetLocalizedString();
            }
        }

        public static string GetUnits(this SolutionDomainType sdType)
        {
            switch (sdType)
            {
                case SolutionDomainType.ROfRho:
                default:
                    return DependentVariableAxisUnits.PerMMSquared.GetInternationalizedString();
                case SolutionDomainType.ROfFx:
                    return DependentVariableAxisUnits.Unitless.GetInternationalizedString();
                case SolutionDomainType.ROfRhoAndTime:
                    return DependentVariableAxisUnits.PerMMSquaredPerNS.GetInternationalizedString();
                case SolutionDomainType.ROfFxAndTime:
                    return DependentVariableAxisUnits.PerNS.GetInternationalizedString();
                case SolutionDomainType.ROfRhoAndFt:
                    return DependentVariableAxisUnits.PerMMSquaredPerGHz.GetInternationalizedString();
                case SolutionDomainType.ROfFxAndFt:
                    return DependentVariableAxisUnits.PerGHz.GetInternationalizedString();
            }
        }

        public static string GetUnits(this FluenceSolutionDomainType sdType)
        {
            switch (sdType)
            {
                case FluenceSolutionDomainType.FluenceOfRhoAndZ:
                default:
                    return DependentVariableAxisUnits.PerMMCubed.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceOfFxAndZ:
                    return DependentVariableAxisUnits.PerMM.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndTime:
                    return DependentVariableAxisUnits.PerMMCubedPerNS.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceOfFxAndZAndTime:
                    return DependentVariableAxisUnits.PerMMPerNS.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndFt:
                    return DependentVariableAxisUnits.PerMMCubedPerGHz.GetInternationalizedString();
                case FluenceSolutionDomainType.FluenceOfFxAndZAndFt:
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
                case IndependentVariableAxis.Time:
                    return new DoubleRange(0D, 0.05D, 51);  // units=ns
                case IndependentVariableAxis.Fx:
                    return new DoubleRange(0D, 0.5D, 51);
                case IndependentVariableAxis.Ft:
                    return new DoubleRange(0D, 0.5D, 51); // units=GHz
                case IndependentVariableAxis.Wavelength:
                    return new DoubleRange(650D, 1000D, 36); //TODO: right units?
            }
        }

        public static double GetDefaultConstantAxisValue(this IndependentVariableAxis constantType)
        {
            switch (constantType)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return 1.0;
                case IndependentVariableAxis.Time:
                    return 0.05;
                case IndependentVariableAxis.Fx:
                    return 0.0;
                case IndependentVariableAxis.Ft:
                    return 0.0;
                case IndependentVariableAxis.Wavelength:
                    return 650.0;
            }
        }
        
        public static RangeViewModel GetDefaultIndependentAxisRange(this IndependentVariableAxis independentAxisType)
        {
            return new RangeViewModel(independentAxisType.GetDefaultRange(), independentAxisType.GetUnits(), independentAxisType, independentAxisType.GetTitle());
        }
    }
}
