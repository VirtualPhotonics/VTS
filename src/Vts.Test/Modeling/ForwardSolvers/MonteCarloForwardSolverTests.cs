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
            Assert.That(value1 > 0, Is.True);

            var value2 = fs.ROfFx(op, 0.1);
            Assert.That(value2 > 0, Is.True);
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
            Assert.That(value1 > 0, Is.True);

            var value2 = fs.ROfFx(ops, fxs).First();
            Assert.That(value2 > 0, Is.True);
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
            Assert.That(value1[0] > 0, Is.True);

            var value2 = fs.ROfFx(ops, fxs);
            Assert.That(value2[0] > 0, Is.True);
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
            Assert.That(rOfRhoArray.Length == 6, Is.True);
            Assert.That(rOfRhoArray[0] > rOfRhoArray[1], Is.True);
            Assert.That(rOfRhoArray[2] < rOfRhoArray[0], Is.True);
            Assert.That(rOfRhoArray[3] > rOfRhoArray[4], Is.True);

            var rOfFxArray = fs.ROfFx(ops, fxs);
            Assert.That(rOfFxArray[0] > 0, Is.True);
            Assert.That(rOfFxArray.Length == 6, Is.True);
            Assert.That(rOfFxArray[0] > rOfFxArray[1], Is.True);
            Assert.That(rOfFxArray[2] < rOfFxArray[0], Is.True);
            Assert.That(rOfFxArray[3] > rOfFxArray[4], Is.True);

            var rOfRhoAndTArray = fs.ROfRhoAndTime(ops, fxs, ts);
            Assert.That(rOfRhoAndTArray[0] > 0, Is.True);
            Assert.That(rOfRhoAndTArray.Length == 12, Is.True);
            Assert.That(rOfRhoAndTArray[0] > rOfRhoAndTArray[1], Is.True); // decreasing with time
            Assert.That(rOfRhoAndTArray[1] < rOfRhoAndTArray[2], Is.True); // next fx should be higher again
            Assert.That(rOfRhoAndTArray[0] > rOfRhoAndTArray[2], Is.True); // decreasing with fx
            Assert.That(rOfRhoAndTArray[2] > rOfRhoAndTArray[3], Is.True); // decreasing with time again
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
            Assert.That(rOfRhoArray.Length == 6, Is.True);
            Assert.That(rOfRhoArray[0] > rOfRhoArray[1], Is.True);
            Assert.That(rOfRhoArray[2] < rOfRhoArray[0], Is.True);
            Assert.That(rOfRhoArray[3] > rOfRhoArray[4], Is.True);

            var rOfFxArray = fs.ROfFx(ops, fxs).ToArray();
            Assert.That(rOfFxArray[0] > 0, Is.True);
            Assert.That(rOfFxArray.Length == 6, Is.True);
            Assert.That(rOfFxArray[0] > rOfFxArray[1], Is.True);
            Assert.That(rOfFxArray[2] < rOfFxArray[0], Is.True);
            Assert.That(rOfFxArray[3] > rOfFxArray[4], Is.True);

            var rOfRhoAndTArray = fs.ROfRhoAndTime(ops, fxs, ts).ToArray();
            Assert.That(rOfRhoAndTArray[0] > 0, Is.True);
            Assert.That(rOfRhoAndTArray.Length == 12, Is.True);
            Assert.That(rOfRhoAndTArray[0] > rOfRhoAndTArray[1], Is.True); // decreasing with time
            Assert.That(rOfRhoAndTArray[1] < rOfRhoAndTArray[2], Is.True); // next fx should be higher again
            Assert.That(rOfRhoAndTArray[0] > rOfRhoAndTArray[2], Is.True); // decreasing with fx
            Assert.That(rOfRhoAndTArray[2] > rOfRhoAndTArray[3], Is.True); // decreasing with time again
        }

    }
}
