using System;
using System.Numerics;
using NUnit.Framework;
using Vts.Common;
using Vts.Factories;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.Factories
{
    [TestFixture]
    public class ComputationFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeReflectance routine using enum
        /// forward solver specification
        /// </summary>
        [Test]
        public void validate_ComputeReflectance_can_be_called_using_enum_forward_solver()
        {
            var reflectance = ComputationFactory.ComputeReflectance(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.ROfRho,
                ForwardAnalysisType.R,
                new object[]
                {
                    // could have array of OPs, one set for each tissue region
                    new[] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                    new double[] { 1, 2, 3 }
                });
            Assert.IsTrue(Math.Abs(reflectance[0] - 0.021093) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeReflectance routine using
        /// IForwardSolver input
        /// </summary>
        [Test]
        public void validate_ComputeReflectance_can_be_called_using_IForwardSolver()
        {
            var reflectance = ComputationFactory.ComputeReflectance(
                new NurbsForwardSolver(),
                SolutionDomainType.ROfFx,
                ForwardAnalysisType.dRdMua,
                new object[]
                {
                    new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                    new double[] { 1, 2, 3 }
                });
            Assert.IsTrue(Math.Abs(reflectance[0] + 0.005571) < 0.000001);
        }
        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using enum
        /// forward solver and array of optical properties
        /// </summary>
        [Test]
        public void validate_ComputeFluence_can_be_called_using_enum_forward_solver_and_optical_property_array()
        {
            double[] xAxis = new double[] {1, 2, 3};
            double[] zAxis = new double[] {1, 2, 3, 4};
            double[][] independentValues = new double[][] {xAxis, zAxis};
            var fluence = ComputationFactory.ComputeFluence(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues, 
                // could have array of OPs, one set for each tissue region
                new OpticalProperties[] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 0 }
                );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using enum
        /// forward solver and single set of OPs
        /// </summary>
        [Test]
        public void validate_ComputeFluence_can_be_called_using_enum_forward_solver_and_single_optical_properties()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), // single OPs
                new double[] { 0 }
            );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }
        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using IForwardSolver and
        /// array of OPs
        /// </summary>
        [Test]
        public void validate_ComputeFluence_can_be_called_using_IForwardSolver_and_optical_property_array()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                // could have array of OPs, one set for each tissue region
                new OpticalProperties[] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 0 }
                );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using IForwardSolver and
        /// single set of OPs
        /// </summary>
        [Test]
        public void validate_ComputeFluence_can_be_called_using_IForwardSolver_and_single_optical_properties()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), // single OPs
                new double[] { 0 }
            );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }
        /// <summary>
        /// Test against the ComputationFactory class ComputeFluenceComplex routine using enum
        /// forward solver and IOpticalProperty array
        /// </summary>
        [Test]
        public void validate_ComputeFluenceComplex_can_be_called_using_enum_forward_solver_and_IOpticalPropertyRegion_array()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            Complex[] fluence = ComputationFactory.ComputeFluenceComplex(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new IOpticalPropertyRegion[] {
                    new LayerTissueRegion(
                        new DoubleRange(0, 10, 10),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                        )
                },
                new double[] { 0 }
                );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluenceComplex routine using enum
        /// forward solver and single set of OPs
        /// </summary>
        [Test]
        public void validate_ComputeFluenceComplex_can_be_called_using_enum_forward_solver_and_single_optical_properties()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), // single OPs
                new double[] { 0 }
            );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }
        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using IForwardSolver and
        /// array of OPs
        /// </summary>
        [Test]
        public void validate_ComputeFluenceComplex_can_be_called_using_IForwardSolver_and_IOpticalPropertyRegion_array()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new IOpticalPropertyRegion[] {
                    new LayerTissueRegion(
                        new DoubleRange(0, 10, 10),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                    )
                },
                new double[] { 0 }
            );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluenceComplex routine using IForwardSolver and
        /// single set of OPs
        /// </summary>
        [Test]
        public void validate_ComputeFluenceComplex_can_be_called_using_IForwardSolver_and_single_optical_properties()
        {
            double[] xAxis = new double[] { 1, 2, 3 };
            double[] zAxis = new double[] { 1, 2, 3, 4 };
            double[][] independentValues = new double[][] { xAxis, zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), // single OPs
                new double[] { 0 }
            );
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using enum
        /// forward solver and array of optical properties
        /// </summary>
        [Test]
        public void validate_SolveInverse_can_be_called_using_enum_forward_solver()
        {
            object[] initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] {1, 2, 3 }
            };
            double[] measuredData = new double[] { 4, 5, 6 };
            double[] solution = ComputationFactory.SolveInverse(
                ForwardSolverType.PointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis
                );
            // solution is a double array with converged solution OPs
             Assert.IsTrue(Math.Abs(solution[1] - 3.75515) < 0.00001);
        }

        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using enum
        /// forward solver and single set of OPs
        /// </summary>
        [Test]
        public void validate_SolveInverse_can_be_called_using_IForwardSolver_and_IOptimizer()
        {
            object[] initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
                };
            double[] measuredData = new double[] { 4, 5, 6 };
            double[] solution = ComputationFactory.SolveInverse(
                new PointSourceSDAForwardSolver(),
                new MPFitLevenbergMarquardtOptimizer(),
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis
            );
            // solution is a double array with converged solution OPs
            Assert.IsTrue(Math.Abs(solution[1] - 3.75515) < 0.00001);
        }
        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using IForwardSolver and
        /// array of OPs
        /// </summary>
        [Test]
        public void validate_SolveInverse_can_be_called_using_enum_forward_solver_and_bounds()
        {
            object[] initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };
            double[] measuredData = new double[] { 4, 5, 6 };
            double[] lowerBounds = new double[] { 0, 0, 0, 0 }; // one for each OP even if not optimized
            double[] upperBounds = new double[]
            {
                double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity
            };
            var solution = ComputationFactory.SolveInverse(
                ForwardSolverType.PointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis, 
                lowerBounds,
                upperBounds);
            // solution is a double array with converged solution OPs
            Assert.IsTrue(Math.Abs(solution[1] - 3.75530) < 0.00001);
        }

        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using IForwardSolver and
        /// single set of OPs
        /// </summary>
        [Test]
        public void validate_SolveInverse_can_be_called_using_IForwardSolver_IOptimizer_and_bounds()
        {
            object[] initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };
            double[] measuredData = new double[] { 4, 5, 6 };
            double[] lowerBounds = new double[] { 0, 0, 0, 0 }; // one for each OP even if not optimized
            double[] upperBounds = new double[]
            {
                double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity
            };
            var solution = ComputationFactory.SolveInverse(
                new PointSourceSDAForwardSolver(),
                new MPFitLevenbergMarquardtOptimizer(),
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis,
                lowerBounds,
                upperBounds
            );
            // solution is a double array with converged solution OPs
            Assert.IsTrue(Math.Abs(solution[1] - 3.75530) < 0.00001);
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
