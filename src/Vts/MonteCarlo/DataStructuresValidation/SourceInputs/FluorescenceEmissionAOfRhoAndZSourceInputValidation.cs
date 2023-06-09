using System.IO;
using System.Reflection;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies the structure of a FluorescentEmissionAOfRhoAndZSourceInput
    /// </summary>
    public class FluorescenceEmissionAOfRhoAndZSourceInputValidation
    {
        /// <summary>
        /// Method to warn that if Uniform sampling is specified, only one
        /// fluorescing cylindrical voxel can be simulated
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
            if (!(region is CaplessCylinderTissueRegion))
            {
                return new ValidationResult(false,
                    "Fluorescent region needs to be cylindrical",
                    "Make sure prior simulation Tissue definition includes (Capless)CylinderTissueRegion");

            }
            // region has to be cylinder region and AOfRhoAndZ rho,dz = voxel size
            // FIX following
            var aOfRhoAndZDetectorInput = priorSimulationInput.DetectorInputs[0];
            if (region is CaplessCylinderTissueRegion &&
                aOfRhoAndZDetectorInput.Rho.Delta != ((CaplessCylinderTissueRegion)region).Radius ||
                aOfRhoAndZDetectorInput.Z.Delta != ((CaplessCylinderTissueRegion)region).Height)
            {
                return new ValidationResult(false,
                    "Fluorescent region needs to be a single voxel",
                    "Make sure AOfRhoAndZDetectorInput definition aligns with (Capless)CylinderTissueRegion");
            }
            return new ValidationResult(true,
                "A Uniform Sampling Type requires that only one voxel is fluorescing");

        }
    }
    
}
