using System.IO;
using System.Linq;
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
        /// Method to validate fluorescence emission infile source specification.
        /// Relies on valid excitation simulation has been executed prior to running
        /// this simulation and checks for the validity of the excitation results.
        /// Also, warns that if Uniform sampling is specified, only one
        /// fluorescing cylindrical voxel can be simulated
        /// </summary>
        /// <param name="input">source input in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ISourceInput input)
        {
            if (((dynamic)input).SamplingMethod != SourcePositionSamplingType.Uniform)
                return new ValidationResult(true, "");

            // check that folder with results exists from excitation simulation
            var currentAssemblyDirectoryName = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            var excitationSimulationResultsFolder = ((dynamic)input).InputFolder;

            var fullPathToFolder = currentAssemblyDirectoryName + "\\" + excitationSimulationResultsFolder;
            if (!Directory.Exists(fullPathToFolder))
            {
                return new ValidationResult(false,
                    "Source InputFolder specification is invalid",
                    "Make sure specified InputFolder exists");
            }

            var excitationSimulationInfileName = ((dynamic)input).Infile;
            var fullPathToInfile = fullPathToFolder + "\\" + excitationSimulationInfileName;
            if (!File.Exists(fullPathToInfile))
            {
                return new ValidationResult(false,
                    "Source Infile specification is invalid",
                    "Make sure InputFolder has specified Infile");
            }
            // open infile to read: need type specification so not dynamic
            SimulationInput excitationSimulationInput = SimulationInput.FromFile(fullPathToInfile);
            // check input to determine fluorescent region index
            var fluorescentRegionIndex = input.InitialTissueRegionIndex;
            // get region
            var region = excitationSimulationInput.TissueInput.Regions[fluorescentRegionIndex];
            // region has to be cylinder region 
            if (!(region is CaplessCylinderTissueRegion))
            {
                return new ValidationResult(false,
                    "Fluorescent region needs to be cylindrical",
                    "Make sure excitation simulation Tissue definition includes (Capless)CylinderTissueRegion");

            }
            // check if AOfRhoAndZ exists from excitation simulation
            if (!excitationSimulationInput.DetectorInputs.Any(d => d.TallyType == TallyType.AOfRhoAndZ))
            {
                return new ValidationResult(false,
                    "No absorbed energy tally generated in excitation simulation",
                    "Make sure excitation simulation specifies AOfRhoAndZ DetectorInput");
            }
            // get detector
            var aOfRhoAndZDetectorInput = (AOfRhoAndZDetectorInput)excitationSimulationInput.DetectorInputs.First(
                d => d.TallyType == TallyType.AOfRhoAndZ);
            // region is cylinder region already checked and AOfRhoAndZ rho,dz = voxel size
            if (aOfRhoAndZDetectorInput.Rho.Delta != ((CaplessCylinderTissueRegion)region).Radius ||
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
