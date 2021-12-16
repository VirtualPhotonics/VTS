using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests execute an MC simulation with 100 photons and verify
    /// that all photons tallie to specular
    /// </summary>
    [TestFixture]
    public class SpecularLayerDetectorTests
    {
        private SimulationOutput _output;
        private double _specularReflectance;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "Output",
        };

        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt",
        };

        /// <summary>
        /// clear all previously generated folders and files
        /// </summary>
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }

        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                     new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                 );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());

            var input = new SimulationInput(
                 100,
                 "Output",
                 new SimulationOptions(
                     0,
                     RandomNumberGeneratorType.MersenneTwister,
                     AbsorptionWeightingType.Analog,
                     new List<DatabaseType>() { }, // databases to be written
                     false, // track statistics
                     0.0, // RR threshold -> 0 = no RR performed
                     0),
                 new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     0 // start in air
                 ),
                 ti,
                new List<IDetectorInput>
                {
                    new RSpecularDetectorInput() {TallySecondMoment = true}, 
                }
            );

            _specularReflectance = Optics.Specular(input.TissueInput.Regions[0].RegionOP.N,
               input.TissueInput.Regions[1].RegionOP.N);
            _output = new MonteCarloSimulation(input).Run();
        }

        // Specular Reflectance
        [Test]
        public void validate_RSpecular()
        {
            Assert.Less(Math.Abs(_output.Rspec - _specularReflectance), 0.003);
            Assert.Less(Math.Abs(_output.Rspec2 - 0.03), 0.01);
            Assert.AreEqual(3, _output.Rspec_TallyCount);
        }
    }
}
