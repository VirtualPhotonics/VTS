using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class MonteCarloForwardSolverTests
    {
        /// <summary>
        /// Setup for the SolverFactory tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test solver construction 
        /// </summary>
        [Test]
        public void constructor_returns_valid_solver()
        {
            var fs = new MonteCarloForwardSolver();

            Assert.IsNotNull(fs);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void simple_forward_calls_return_nonzero_result()
        {
            var fs = new MonteCarloForwardSolver();
            var op = new OpticalProperties();

            var value1 = fs.ROfRho(op, 10);
            Assert.IsTrue(value1 > 0);

            var value2 = fs.ROfFx(op, 0.1);
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void enumerable_forward_calls_return_nonzero_result()
        {
            var fs = new MonteCarloForwardSolver();
            var ops = new OpticalProperties().AsEnumerable();
            var rhos = 10D.AsEnumerable();
            var fxs = 0.1.AsEnumerable();

            var value1 = fs.ROfRho(ops, rhos).First();
            Assert.IsTrue(value1 > 0);

            var value2 = fs.ROfFx(ops, fxs).First();
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void array_forward_calls_return_nonzero_result()
        {
            var fs = new MonteCarloForwardSolver();
            var ops = new[] { new OpticalProperties() };
            var rhos = new[] { 10.0 };
            var fxs = new[] { 0.1 };

            var value1 = fs.ROfRho(ops, rhos);
            Assert.IsTrue(value1[0] > 0);

            var value2 = fs.ROfFx(ops, fxs);
            Assert.IsTrue(value2[0] > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void array_forward_calls_return_correct_number_of_results_in_the_right_order()
        {
            var fs = new MonteCarloForwardSolver();
            var ops = new[] { new OpticalProperties(), new OpticalProperties() };
            var rhos = new[] { 10.0, 11.0, 12.0 };
            var fxs = new[] { 0.1, 0.2, 0.3 };
            var ts = new[] { 0.02, 0.04 };

            var rOfRhoArray = fs.ROfRho(ops, rhos);
            Assert.IsTrue(rOfRhoArray.Length == 6);
            Assert.IsTrue(rOfRhoArray[0] > rOfRhoArray[1]);
            Assert.IsTrue(rOfRhoArray[2] < rOfRhoArray[0]);
            Assert.IsTrue(rOfRhoArray[3] > rOfRhoArray[4]);

            var rOfFxArray = fs.ROfFx(ops, fxs);
            Assert.IsTrue(rOfFxArray[0] > 0);
            Assert.IsTrue(rOfFxArray.Length == 6);
            Assert.IsTrue(rOfFxArray[0] > rOfFxArray[1]);
            Assert.IsTrue(rOfFxArray[2] < rOfFxArray[0]);
            Assert.IsTrue(rOfFxArray[3] > rOfFxArray[4]);

            var rOfRhoAndTArray = fs.ROfRhoAndTime(ops, fxs, ts);
            Assert.IsTrue(rOfRhoAndTArray[0] > 0);
            Assert.IsTrue(rOfRhoAndTArray.Length == 12);
            Assert.IsTrue(rOfRhoAndTArray[0] > rOfRhoAndTArray[1]); // decreasing with time
            Assert.IsTrue(rOfRhoAndTArray[1] < rOfRhoAndTArray[2]); // next fx should be higher again
            Assert.IsTrue(rOfRhoAndTArray[0] > rOfRhoAndTArray[2]); // decreasing with fx
            Assert.IsTrue(rOfRhoAndTArray[2] > rOfRhoAndTArray[3]); // decreasing with time again
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void enumerable_forward_calls_return_correct_number_of_results_in_the_right_order()
        {
            var fs = new MonteCarloForwardSolver();
            var ops = new List<OpticalProperties> { new OpticalProperties(), new OpticalProperties() };
            var rhos = new List<double> { 10.0, 11.0, 12.0 };
            var fxs = new List<double> { 0.1, 0.2, 0.3 };
            var ts = new List<double> { 0.02, 0.04 };

            var rOfRhoArray = fs.ROfRho(ops, rhos).ToArray();
            Assert.IsTrue(rOfRhoArray.Length == 6);
            Assert.IsTrue(rOfRhoArray[0] > rOfRhoArray[1]);
            Assert.IsTrue(rOfRhoArray[2] < rOfRhoArray[0]);
            Assert.IsTrue(rOfRhoArray[3] > rOfRhoArray[4]);

            var rOfFxArray = fs.ROfFx(ops, fxs).ToArray();
            Assert.IsTrue(rOfFxArray[0] > 0);
            Assert.IsTrue(rOfFxArray.Length == 6);
            Assert.IsTrue(rOfFxArray[0] > rOfFxArray[1]);
            Assert.IsTrue(rOfFxArray[2] < rOfFxArray[0]);
            Assert.IsTrue(rOfFxArray[3] > rOfFxArray[4]);

            var rOfRhoAndTArray = fs.ROfRhoAndTime(ops, fxs, ts).ToArray();
            Assert.IsTrue(rOfRhoAndTArray[0] > 0);
            Assert.IsTrue(rOfRhoAndTArray.Length == 12);
            Assert.IsTrue(rOfRhoAndTArray[0] > rOfRhoAndTArray[1]); // decreasing with time
            Assert.IsTrue(rOfRhoAndTArray[1] < rOfRhoAndTArray[2]); // next fx should be higher again
            Assert.IsTrue(rOfRhoAndTArray[0] > rOfRhoAndTArray[2]); // decreasing with fx
            Assert.IsTrue(rOfRhoAndTArray[2] > rOfRhoAndTArray[3]); // decreasing with time again
        }

        /// <summary>
        /// Tear down for the NurbsGenerator tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
        }
    }
}
