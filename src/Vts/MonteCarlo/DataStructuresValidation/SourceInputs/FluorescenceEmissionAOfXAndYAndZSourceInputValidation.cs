using System.IO;
using System.Linq;
using System.Reflection;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies the structure of a FluorescentEmissionAOfXAndYAndZSourceInput
    /// </summary>
    public static class FluorescenceEmissionAOfXAndYAndZSourceInputValidation
    {
        /// <summary>
        /// Method to validate fluorescence emission infile source specification.
        /// Relies on valid excitation simulation has been executed prior to running
        /// this simulation and checks for the validity of the excitation results.
        /// Also, warns that if Uniform sampling is specified, only one
        /// fluorescing voxel can be simulated
        /// </summary>
        /// <param name="input">source input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ISourceInput input)
        {
            if (((FluorescenceEmissionAOfXAndYAndZSourceInput)input).SamplingMethod != SourcePositionSamplingType.Uniform)
                return new ValidationResult(true, "");

            // check that folder with results exists from prior simulation
            var currentAssemblyDirectoryName = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            var excitationSimulationResultsFolder = ((FluorescenceEmissionAOfXAndYAndZSourceInput)input).InputFolder;

            var fullPathToFolder = currentAssemblyDirectoryName + "\\" + excitationSimulationResultsFolder;
            if (!Directory.Exists(fullPathToFolder))
            {
                return new ValidationResult(false,
                    "Source InputFolder specification is invalid",
                    "Make sure specified InputFolder exists");
            }

            var excitationSimulationInfileName = ((FluorescenceEmissionAOfXAndYAndZSourceInput)input).Infile;
            var fullPathToInfile = fullPathToFolder + "\\" + excitationSimulationInfileName;
            if (!File.Exists(fullPathToInfile))
            {
                return new ValidationResult(false,
                    "Source Infile specification is invalid",
                    "Make sure InputFolder has specified Infile");
            }
            var excitationSimulationInput = SimulationInput.FromFile(fullPathToInfile);
            // check input to determine fluorescent region index
            var fluorescentRegionIndex = input.InitialTissueRegionIndex;
            // get region
            var region = excitationSimulationInput.TissueInput.Regions[fluorescentRegionIndex];
            if (!(region is VoxelTissueRegion))
            {
                return new ValidationResult(false,
                    "Fluorescent region needs to be VoxelTissueRegion",
                    "Make sure prior simulation Tissue definition includes VoxelTissueRegion");

            }
            // check if AOfXAndYAndZ exists from excitation simulation
            if (!excitationSimulationInput.DetectorInputs.Any(d => d.TallyType != TallyType.AOfXAndYAndZ))
            {
                return new ValidationResult(false,
                    "No absorbed energy tally generated in excitation simulation",
                    "Make sure excitation simulation specifies AOfXAndYAndZ DetectorInput");
            }
            // get detector 
            var aOfXAndYAndZDetectorInput = (AOfXAndYAndZDetectorInput)excitationSimulationInput.DetectorInputs.First(
                d => d.TallyType == TallyType.AOfXAndYAndZ);
            // region is voxel region already checked and AOfXAndYAndZ dx,dy,dz = voxel size
            if (aOfXAndYAndZDetectorInput.X.Delta != ((VoxelTissueRegion)region).X.Delta ||
                aOfXAndYAndZDetectorInput.Y.Delta != ((VoxelTissueRegion)region).Y.Delta ||
                aOfXAndYAndZDetectorInput.Z.Delta != ((VoxelTissueRegion)region).Z.Delta)
            {
                return new ValidationResult(false,
                    "Fluorescent region needs to be a single voxel",
                    "Make sure AOfXAndYAndZDetectorInput definition aligns with VoxelRegion");
            }
            return new ValidationResult(true, 
                "A Uniform Sampling Type requires that only one voxel is fluorescing");

        }
       
    }
}
