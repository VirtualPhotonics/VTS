using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.SourceInputs
{
    [TestFixture]
    public class FluorescenceEmissionAOfXAndYAndZInputValidationTests
    {
        private AOfXAndYAndZDetector _aOfXAndYAndZDetector;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFolders = new List<string>()
        {
            "sourcetest",
        };

        readonly List<string> _listOfTestGeneratedFiles = new List<string>()
        {
            "inputAOfXAndYAndZ.txt"
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in _listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }
        /// <summary>
        /// Set up output folder representing results from a prior MC simulation
        /// that sets up A(x,y,z) detector array to verify validation code correctly.
        /// Note about this unit test: a) could not just run MC simulation and read 
        /// results because MC sim puts results in Vts.MCCL/bin/Debug an not accessible 
        /// by this unit test, b) goal was to test if source reads data correctly, 
        /// to do this needed to have correct files in resources then
        /// write them locally so that they could be read by this unit test.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            // AOfXAndYAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=4
            // detector rho=[0 4] 4 bins, z=[0 2] 2 bins
            _aOfXAndYAndZDetector = (dynamic)DetectorIO.ReadDetectorFromFileInResources(
                "AOfXAndYAndZ", "Resources/sourcetest/", assemblyName);

            DetectorIO.WriteDetectorToFile(_aOfXAndYAndZDetector, "sourcetest");
            // write prior simulation infile to folder
            FileIO.CopyFileFromResources(
                "Resources/sourcetest/inputAOfXAndYAndZ.txt",
                "sourcetest/inputAOfXAndYAndZ.txt", assemblyName);

        }
        /// <summary>
        /// Test to check that validation result is correct when specifying
        /// Uniform sampling.  The infile inputAOfXAndYAndZ.txt used to specify that absorbed
        /// energy to serve as a source specifies AOfXAndYAndZ detector with dx=1, dy=0.25,dz=1.
        /// However the problem is that this input specifies an infinite cylinder as the
        /// tissue region which will not align with a voxel specification regardless of its
        /// dimensions which needs to occur to have a *single* fluorescent region.
        /// </summary>
        [Test]
        public void Validate_Uniform_sampling_specification_returns_correct_validation()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new FluorescenceEmissionAOfXAndYAndZSourceInput
                {
                    InputFolder = "sourcetest",
                    Infile = "inputAOfXAndYAndZ.txt", // this has fluor region = voxel 
                    InitialTissueRegionIndex = 3,
                    SamplingMethod = SourcePositionSamplingType.Uniform
                },
                new SingleVoxelTissueInput( // specify voxel dimensions that does not match input
                    new VoxelTissueRegion(
                        new DoubleRange(-5, 5),
                        new DoubleRange(-5, 5),
                        new DoubleRange(1, 6),
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new ITissueRegion[]
                        {
                            new LayerTissueRegion(
                                new DoubleRange(double.NegativeInfinity, 0.0),
                                new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                            new LayerTissueRegion(
                                new DoubleRange(0.0, 100.0),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                            new LayerTissueRegion(
                                new DoubleRange(100.0, double.PositiveInfinity),
                                new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                        }),
                new List<IDetectorInput>
                {
                    new ROfXAndYDetectorInput()
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ValidationRule != null, Is.True);
        }
    }
}