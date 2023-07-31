using NUnit.Framework;
using System;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    internal class MonteCarloSetupTests
    {
        [Test]
        public void ReadSimulationInputFromFile_returns_null()
        {
            var result = MonteCarloSetup.ReadSimulationInputFromFile("");
            Assert.IsNull(result);
        }

        [Test]
        public void ReadSimulationInputFromFile_throws_FileNotFoundException_returns_null()
        {
            var result = MonteCarloSetup.ReadSimulationInputFromFile("dummy.txt");
            Assert.IsNull(result);
        }

        [Test]
        public void CreateParameterSweep_ParameterSweepType_List()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "1", "1" }, ParameterSweepType.List);
            Assert.IsNull(result);
        }

        [Test]
        public void CreateParameterSweep_ParameterSweepType_List_invalid_parameters()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "", "1", "string", "string" }, ParameterSweepType.Delta);
            Assert.IsNull(result);
        }

        [Test]
        public void CreateParameterSweep_invalid_parameter_count_Count()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "" }, ParameterSweepType.Count);
            Assert.IsNull(result);
        }

        [Test]
        public void CreateParameterSweep_invalid_ParameterSweepType()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "mus1", "0.5", "1.5", "0.1" }, (ParameterSweepType)Enum.GetValues(typeof(ParameterSweepType)).Length + 1
                );
            Assert.IsNull(result);
        }

        [Test]
        public void CreateParameterSweep_Delta()
        {
            var parameterSweep =
                MonteCarloSetup.CreateParameterSweep(new[] { "mus1", "0.5", "1.5", "0.1" }, ParameterSweepType.Delta);
            Assert.AreEqual("mus1", parameterSweep.Name);
            Assert.AreEqual(0.5, parameterSweep.Range.Start);
            Assert.AreEqual(1.5, parameterSweep.Range.Stop);
            Assert.AreEqual(0.1, parameterSweep.Range.Delta);
        }

        [Test]
        public void CreateParameterSweep_invalid_parameter_count_Delta()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "" }, ParameterSweepType.Delta);
            Assert.IsNull(result);
        }
    }
}
