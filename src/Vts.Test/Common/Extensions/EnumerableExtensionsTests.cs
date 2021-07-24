using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Common
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        private Mock<ForwardSolverBase> _forwardSolverBaseMock;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _forwardSolverBaseMock = new Mock<ForwardSolverBase>()
            {
                CallBase = true
            };
        }

        [Test]
        public void Test_LoopOverVariables_with_two_values()
        {
            var doubleList =
                _forwardSolverBaseMock.Object.ROfRho(new List<OpticalProperties>
                {
                    new OpticalProperties(0.1, 1, 0.8, 1.4),
                    new OpticalProperties(0.01, 1, 0.8, 1.4)
                }, new List<double>
                {
                    0.1,
                    0.2,
                    0.3
                });
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
            Assert.Throws<NotImplementedException>(() =>
            {
                var arrayList = doubleList.ToArray();
                Assert.IsNull(arrayList);
            });
        }

        [Test]
        public void Test_LoopOverVariables_with_three_values()
        {
            var doubleList =
                _forwardSolverBaseMock.Object.ROfRhoAndTime(new List<OpticalProperties>
                {
                    new OpticalProperties(0.1, 1, 0.8, 1.4),
                    new OpticalProperties(0.01, 1, 0.8, 1.4)
                }, new List<double>
                {
                    0.1,
                    0.2,
                    0.3
                }, new List<double>
                {
                    0.1,
                    0.2
                });
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
            Assert.Throws<NotImplementedException>(() =>
            {
                var arrayList = doubleList.ToArray();
                Assert.IsNull(arrayList);
            });
        }
    }
}
