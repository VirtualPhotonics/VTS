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
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReadSimulationInputFromFile_throws_FileNotFoundException_returns_null()
        {
            var result = MonteCarloSetup.ReadSimulationInputFromFile("dummy.txt");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateParameterSweep_ParameterSweepType_List()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "1", "1" }, ParameterSweepType.List);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateParameterSweep_ParameterSweepType_List_invalid_parameters()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "", "1", "string", "string" }, ParameterSweepType.Delta);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateParameterSweep_invalid_parameter_count_Count()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "" }, ParameterSweepType.Count);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateParameterSweep_invalid_ParameterSweepType()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "mus1", "0.5", "1.5", "0.1" }, (ParameterSweepType)Enum.GetValues(typeof(ParameterSweepType)).Length + 1
                );
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateParameterSweep_Delta()
        {
            var parameterSweep =
                MonteCarloSetup.CreateParameterSweep(new[] { "mus1", "0.5", "1.5", "0.1" }, ParameterSweepType.Delta);
            Assert.That( parameterSweep.Name, Is.EqualTo("mus1"));
            Assert.That( parameterSweep.Range.Start, Is.EqualTo(0.5));
            Assert.That( parameterSweep.Range.Stop, Is.EqualTo(1.5));
            Assert.That( parameterSweep.Range.Delta, Is.EqualTo(0.1));
        }

        [Test]
        public void CreateParameterSweep_invalid_parameter_count_Delta()
        {
            var result =
                MonteCarloSetup.CreateParameterSweep(new[] { "" }, ParameterSweepType.Delta);
            Assert.That(result, Is.Null);
        }
    }
}
