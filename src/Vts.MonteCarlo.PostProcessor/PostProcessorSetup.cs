using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PostProcessing;

namespace Vts.MonteCarlo.PostProcessor
{
    public class PostProcessorSetup
    {
        /// <summary>
        /// method to read the post processor input from a specified or default files
        /// </summary>
        public static PostProcessorInput ReadPostProcessorInputFromFile(string inputFile)
        {
            try
            {
                // read input file then read in elements of input file
                if (string.IsNullOrEmpty(inputFile))
                {
                        Console.WriteLine("\nNo input file specified. Using infile.txt from root mc_post.exe folder... ");
                        return ReadPostProcessorInputFromFile("infile.txt");
                }
            
                //get the full path for the input file
                var fullFilePath = Path.GetFullPath(inputFile);

                if (File.Exists(fullFilePath))
                {
                    return PostProcessorInput.FromFile(fullFilePath);
                }

                if (File.Exists(fullFilePath + ".txt"))
                {
                    return PostProcessorInput.FromFile(fullFilePath + ".txt");
                }

                //throw a file not found exception
                throw new FileNotFoundException("\nThe following input file could not be found: " + fullFilePath + " - type mc_post help=infile for correct syntax");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static ValidationResult ValidatePostProcessorInput(PostProcessorInput input)
        {
            return PostProcessorInputValidation.ValidateInput(input);
        }

        // need to work on following
        /// <summary>
        /// Runs the Monte Carlo Post-processor
        /// </summary>
        public static void RunPostProcessor(PostProcessorInput input, string outputFolderPath)
        {
            // locate root folder for output, creating it if necessary
            var path = string.IsNullOrEmpty(outputFolderPath)
                ? Path.GetFullPath(Directory.GetCurrentDirectory())
                : Path.GetFullPath(outputFolderPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // locate destination folder for output, creating it if necessary
            var resultsFolder = Path.Combine(path, input.OutputName);
            if (!Directory.Exists(resultsFolder))
            {
                Directory.CreateDirectory(resultsFolder);
            }

            SimulationOutput postProcessedOutput = null;

            var databaseGenerationInputFile = SimulationInput.FromFile(Path.Combine(input.InputFolder, input.DatabaseSimulationInputFilename + ".txt"));
            // check for pMC tallies first because could have ReflectanceTallies mixed in and want to load CollisionInfo

            // Why not mirror the "on-the-fly" code, and allow for all kinds of detector inputs simultaneously? (dc 12/21/2011)
            if (input.DetectorInputs.Where(di => di.TallyDetails.IspMCReflectanceTally).Any())
            {
                IList<IDetectorInput> pMCDetectorInputs;
                pMCDetectorInputs = input.DetectorInputs;
                var postProcessor = new PhotonDatabasePostProcessor(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    pMCDetectorInputs,
                    PhotonDatabaseFactory.GetpMCDatabase( // database filenames are assumed to be convention
                        VirtualBoundaryType.pMCDiffuseReflectance,
                        input.InputFolder),
                    databaseGenerationInputFile
                    );
                postProcessedOutput = postProcessor.Run();
            }
            else if (input.DetectorInputs.Where(di => di.TallyDetails.IsReflectanceTally).Any())
            {
              
                var postProcessor = new PhotonDatabasePostProcessor(
                    VirtualBoundaryType.DiffuseReflectance,
                    input.DetectorInputs, 
                    PhotonDatabaseFactory.GetPhotonDatabase( //database filenames are assumed to be convention
                        VirtualBoundaryType.DiffuseReflectance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
                postProcessedOutput = postProcessor.Run();
            }
            else if (input.DetectorInputs.Where(di => di.TallyDetails.IsTransmittanceTally).Any())
            {
                var postProcessor = new PhotonDatabasePostProcessor(
                    VirtualBoundaryType.DiffuseTransmittance,
                    input.DetectorInputs,
                    PhotonDatabaseFactory.GetPhotonDatabase( //database filenames are assumed to be convention
                        VirtualBoundaryType.DiffuseTransmittance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
                postProcessedOutput = postProcessor.Run();
            }
            else if (input.DetectorInputs.Where(di => di.TallyDetails.IsSpecularReflectanceTally).Any())
            {
                var postProcessor = new PhotonDatabasePostProcessor(
                    VirtualBoundaryType.SpecularReflectance,
                    input.DetectorInputs,
                    PhotonDatabaseFactory.GetPhotonDatabase( //database filenames are assumed to be convention
                        VirtualBoundaryType.SpecularReflectance,
                        input.InputFolder),
                    databaseGenerationInputFile
                );
                postProcessedOutput = postProcessor.Run();
            }


            var folderPath = input.OutputName;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // save input file to output folder with results
            input.ToFile(resultsFolder + "\\" + input.OutputName + ".txt");

            // save database generation input file to output folder
            databaseGenerationInputFile.ToFile(resultsFolder + "\\" + input.OutputName + "_database_infile.txt");

            if (postProcessedOutput != null)
            {
                foreach (var result in postProcessedOutput.ResultsDictionary.Values)
                {
                    // save all detector data to the specified folder
                    DetectorIO.WriteDetectorToFile(result, folderPath);
                }
            }
        }

        /// <summary>
        /// Runs multiple Post-Processor tasks in parallel using all available CPU cores
        /// </summary>
        public static void RunPostProcessors(IEnumerable<PostProcessorInput> inputs, string outputFolderPath)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(inputs, options, (input, state, index) =>
            {
                RunPostProcessor(input, outputFolderPath);
            });
        }
    }
}


