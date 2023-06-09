using System.IO;
using System.Linq;
using System.Reflection;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies the structure of a FluorescentEmissionAOfXAndYAndZSourceInput
    /// </summary>
    public class FluorescenceEmissionAOfXAndYAndZSourceInputValidation
    {
        /// <summary>
        /// Method to warn that if Uniform sampling is specified, only one
        /// fluorescing voxel can be simulated
        /// </summary>
        /// <param name="input">source input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ISourceInput input)
        {
            if (((dynamic)input).SamplingMethod != SourcePositionSamplingType.Uniform)
                return new ValidationResult(true, "");

            // check that folder with results exists from prior simulation
            var currentAssemblyDirectoryName = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            var priorSimulationResultsFolder = ((dynamic)input).InputFolder;

            var fullPathToFolder = currentAssemblyDirectoryName + "\\" + priorSimulationResultsFolder;
            if (!Directory.Exists(fullPathToFolder))
            {
                return new ValidationResult(false,
                    "Source InputFolder specification is invalid",
                    "Make sure specified InputFolder exists");
            }

            var priorSimulationInfileName = ((dynamic)input).Infile;
            var fullPathToInfile = fullPathToFolder + "\\" + priorSimulationInfileName;
            if (!File.Exists(fullPathToInfile))
            {
                return new ValidationResult(false,
                    "Source Infile specification is invalid",
                    "Make sure InputFolder has specified Infile");
            }
            // open infile to read
            var priorSimulationInput = SimulationInput.FromFile(fullPathToInfile);
            // check input to determine fluorescent region index
            var fluorescentRegionIndex = input.InitialTissueRegionIndex;
            // get region
            var region = priorSimulationInput.TissueInput.Regions[fluorescentRegionIndex];
            if (!(region is VoxelTissueRegion))
            {
                return new ValidationResult(false,
                    "Fluorescent region needs to be VoxelTissueRegion",
                    "Make sure prior simulation Tissue definition includes VoxelTissueRegion");

            }
            // get detector FIX!
            var aOfXAndYAndZDetectorInput = priorSimulationInput.DetectorInputs[0];
            // region has to be voxel region and AOfXAndYAndZ dx,dy,dz = voxel size
            if (region is VoxelTissueRegion && 
                aOfXAndYAndZDetectorInput.X.Delta != ((VoxelTissueRegion)region).X.Delta ||
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
