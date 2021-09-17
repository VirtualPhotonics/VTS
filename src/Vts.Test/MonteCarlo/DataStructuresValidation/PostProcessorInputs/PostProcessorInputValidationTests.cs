using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.PostProcessorInputs
{
    [TestFixture]
    public class PostProcessorInputValidationTests
    {
        /// <summary>
        /// Note: PostProcessor infile does not expect ".txt" appended to
        /// simulationInput filename.  
        /// </summary>
        PostProcessorInput input;
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfFolders = new List<string>()
        {
            "results", 
        };
        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var folder in listOfFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }
        [OneTimeSetUp]
        public void setup_input()
        {
            {
                var tissueInput = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)), 
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    });
                input = new PostProcessorInput(
                    new List<IDetectorInput>()
                    {
                        new pMCROfRhoDetectorInput()
                        {
                            Rho = new DoubleRange(0.0, 100.0),
                            // set perturbed ops to reference ops
                            PerturbedOps = new List<OpticalProperties>()
                            {
                                tissueInput.Regions[0].RegionOP,
                                tissueInput.Regions[1].RegionOP,
                                tissueInput.Regions[2].RegionOP
                            },
                            PerturbedRegionsIndices = new List<int>() {1}
                        }
                    }, // don't define folder or files, let individual tests do that
                    "", "", ""
                );
            }
        }

        /// <summary>
        /// Test to check that post-processor perturbed OPs are not negative
        /// </summary>
        [Test]
        public void Validate_tissue_optical_properties_are_non_negative()
        {
            // set pMC mua value to be negative
            ((pMCROfRhoDetectorInput) input.DetectorInputs[0]).PerturbedOps[1].Mua = -0.01;
            var result = PostProcessorInputValidation.ValidateInput(input,"");
            Assert.IsTrue(result.ValidationRule.Equals("Tissue optical properties mua, mus', n need to be non-negative"));
            // set pMC mua value back to being positive so passes rest of tests
            ((pMCROfRhoDetectorInput)input.DetectorInputs[0]).PerturbedOps[1].Mua = 0.01;
        }
        /// <summary>
        /// Test to check input folder exists
        /// </summary>
        [Test]
        public void Validate_input_folder_existence()
        {
            var result = PostProcessorInputValidation.ValidateInput(input, "");
            Assert.IsTrue(result.ValidationRule.Equals(
                "PostProcessorInput: the input folder does not exist"));
        }
        /// <summary>
        /// Test to check for simulation input
        /// </summary>
        [Test]
        public void Validate_simulation_input_existence()
        {
            // no ".txt" extension here
            input.DatabaseSimulationInputFilename = "simulationInput";
            input.InputFolder = "results";
            // create test folder, database file and simulation input file
            var folderName = "results";

            // remove directory if already there
            FileIO.DeleteDirectory(folderName);

            // recreate it
            FileIO.CreateDirectory(folderName);

            // put database files in it
            var diffuseReflectanceDatabase = new PhotonDatabase();
            diffuseReflectanceDatabase.WriteToJsonFile(folderName + "/DiffuseReflectanceDatabase");
            var collisionInfoDatabase = new CollisionInfoDatabase();
            collisionInfoDatabase.WriteToJsonFile(folderName + "/CollisionInfoDatabase");
            
            // first check for no existence of simulation input file
            var result = PostProcessorInputValidation.ValidateInput(input, "");
            Assert.IsFalse(result.IsValid);
            // now put file in place and test
            var simulationInput = new SimulationInput();
            simulationInput.ToFile(folderName + "/simulationInput.txt");
            result = PostProcessorInputValidation.ValidateInput(input, "");
            Assert.IsTrue(result.IsValid);
            // remove directory so other tests don't have it
            FileIO.DeleteDirectory(folderName);
            // set back input
            input.DatabaseSimulationInputFilename = "";
            input.InputFolder = "";
        }
        /// <summary>
        /// test that ValidatePhotonDatabaseExistence method returns correct ValidationRule
        /// for different settings of TallyDetails.  To get to this method, validation
        /// that the input folder exists must pass first so create folder.
        /// Note: methods being tested are private so only way to test is through public
        /// calling method.
        /// Tests assume that the existence of the database file will fail and will pass
        /// back a unique ValidationRule that can verify which "if" was used.
        /// </summary>
        [Test]
        public void Verify_database_aligns_with_TallyDetails_of_detector()
        {
            string folderName = "results";
            // create folder
            FileIO.CreateDirectory(folderName);
            // test detector that is defined in the OneTimeSetup input = pMC reflectance tally
            var result = PostProcessorInputValidation.ValidateInput(input, folderName);
            Assert.IsTrue(result.ValidationRule.Equals(
                "PostProcessorInput:  files DiffuseReflectanceDatabase or CollisionInfoDatabase do not exist"));
            // clear out detector list
            input.DetectorInputs.Clear();
            // add reflectance tally detector
            input.DetectorInputs.Add(new ROfRhoDetectorInput()); // this has IsReflectanceTally=true;
            result = PostProcessorInputValidation.ValidateInput(input, folderName);
            Assert.IsTrue(result.ValidationRule.Equals(
                "PostProcessorInput:  file DiffuseReflectanceDatabase does not exist")); 
            // clear out detector list
            input.DetectorInputs.Clear();
            // add transmittance tally detector
            input.DetectorInputs.Add(new TOfRhoDetectorInput()); // this has IsTransmittanceTally=true;
            result = PostProcessorInputValidation.ValidateInput(input, folderName);
            Assert.IsTrue(result.ValidationRule.Equals(
                "PostProcessorInput:  file DiffuseTransmittanceDatabase does not exist"));
            // clear out detector list
            input.DetectorInputs.Clear();
            // add specular reflectance tally detector
            input.DetectorInputs.Add(new RSpecularDetectorInput()); // this has IsSpecularReflectanceTally=true;
            result = PostProcessorInputValidation.ValidateInput(input, folderName);
            Assert.IsTrue(result.ValidationRule.Equals(
                "PostProcessorInput:  file SpecularReflectanceDatabase does not exist"));
        }
    }
}
