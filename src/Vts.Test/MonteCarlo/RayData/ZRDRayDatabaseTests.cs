﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.RayData;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class ZRDRayDatabaseTests
    {
        private static SimulationInput _input;
        private static SimulationOutput _output;
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testzrdraydatabase",
            "ZRDDiffuseReflectanceDatabase", // name has no "test" prefix, it is generated by the code so name fixed
            "ZRDDiffuseReflectanceDatabase.txt",
            "file.txt", // file that captures output of MC simulation that usually goes to screen
        };

        /// <summary>
        /// Set up simulation specifying ZRDRayDatabase to be written
        /// </summary>
        [OneTimeSetUp]
        public void setup_simulation_input_components()
        {
            // delete previously generated files
            clear_folders_and_files();

            _input = new SimulationInput(
                100,
                "",
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { DatabaseType.ZRDDiffuseReflectance }, // SPECIFY DATABASE
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    1),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1),
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                            new LayerTissueRegion(
                                new DoubleRange(double.NegativeInfinity, 0.0),
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                            new LayerTissueRegion(
                                new DoubleRange(0.0, 20.0),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                            new LayerTissueRegion(
                                new DoubleRange(20.0, double.PositiveInfinity),
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }),
                new List<IDetectorInput>()
                {
                    new ROfRhoAndTimeDetectorInput()
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1, 101)
                    }
                }              
            );
            _output = new MonteCarloSimulation(_input).Run();
        }

        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in listOfTestGeneratedFiles)
            {
                GC.Collect();
                FileIO.FileDelete(file);
            }
        }
        ///// <summary>
        ///// test to verify RayDatabase.FromFile is working correctly using actual Zemax ZRD file
        ///// </summary>
        //[Test]
        //public void validate_ZRDRayDatabase_deserialized_class_is_correct_when_using_FromFile()
        //{
        //    var databaseFilename = @"C:\Users\hayakawa\Desktop\RP\Zemax\uncompressed";
        //    // read the database from file, and verify the correct number of photons were written
        //    var rayDatabase = ZRDRayDatabase.FromFile(databaseFilename);

        //    // manually enumerate through the first two elements (same as foreach)
        //    // PhotonDatabase is designed so you don't have to have the whole thing
        //    // in memory, so .ToArray() loses the benefits of the lazy-load data point

        //    Assert.AreEqual(1000, rayDatabase.NumberOfElements);
        //    var enumerator = rayDatabase.DataPoints.GetEnumerator();
        //    // advance to the first point and test that the point is valid
        //    enumerator.MoveNext();
        //    var dp1 = enumerator.Current;
        //    Assert.AreEqual(0.0, dp1.Z);
        //    Assert.IsTrue(Math.Abs(dp1.Weight - 0.001) < 0.0001);
        //}

        /// <summary>
        /// test to verify RayDatabase.ToFile is working correctly.
        /// </summary>
        [Test]
        public void validate_ZRDRayDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            string databaseFileName = "testzrdraydatabase";
            var firstRayDP = new ZRDRayDataPoint(new RayDataPoint(
                    new Position(1, 1, 0),
                    new Direction(0, 1 / Math.Sqrt(2), -1 / Math.Sqrt(2)),
                    1.0));
            var secondRayDP = new ZRDRayDataPoint(new RayDataPoint(
                    new Position(2, 2, 0),
                    new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2)),
                    2.0));
            using (var dbWriter = new ZRDRayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName))
            {       
                dbWriter.Write(firstRayDP);
                dbWriter.Write(secondRayDP);
            }
            // read back file written
            var rayDatabase = ZRDRayDatabase.FromFile(databaseFileName);
            Assert.AreEqual(rayDatabase.NumberOfElements, 2);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsTrue(dp1.X == firstRayDP.X);
            Assert.IsTrue(dp1.Y == firstRayDP.Y);
            Assert.IsTrue(dp1.Z == firstRayDP.Z);
            Assert.IsTrue(dp1.Ux == firstRayDP.Ux);
            Assert.IsTrue(dp1.Uy == firstRayDP.Uy);
            Assert.IsTrue(dp1.Uz == firstRayDP.Uz);
            Assert.IsTrue(dp1.Weight == firstRayDP.Weight);
            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsTrue(dp2.X == secondRayDP.X);
            Assert.IsTrue(dp2.Y == secondRayDP.Y);
            Assert.IsTrue(dp2.Z == secondRayDP.Z);
            Assert.IsTrue(dp2.Ux == secondRayDP.Ux);
            Assert.IsTrue(dp2.Uy == secondRayDP.Uy);
            Assert.IsTrue(dp2.Uz == secondRayDP.Uz);
            Assert.IsTrue(dp2.Weight == secondRayDP.Weight);
        }

        [Test]
        public void validate_ZRDDatabase_file_gets_written_and_is_correct()
        {
            Assert.IsTrue(FileIO.FileExists("ZRDDiffuseReflectanceDatabase"));
            // read the database from file, and verify the correct number of photons were written
            var rayDatabase = ZRDRayDatabase.FromFile("ZRDDiffuseReflectanceDatabase");

            Assert.AreEqual(88, rayDatabase.NumberOfElements);
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.AreEqual(0.0, dp1.Z);
            Assert.IsTrue(Math.Abs(dp1.Weight - 0.021116) < 0.000001);
        }
    }
}