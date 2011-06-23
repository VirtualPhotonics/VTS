using System.Collections.Generic;
using System;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate virtual boundary given a list of detectors.
    /// </summary>
    public class VirtualBoundaryFactory
    {
        /// <summary>
        /// This static method instantiates a list of virtual boundaries given a list of detector.
        /// If the VB is not already instantiated, it is created and the appropriate detectors are 
        /// deposited into it.  
        /// </summary>
        /// <param name="detectors"></param>
        /// <returns></returns>
        public static IList<IVirtualBoundary> GetVirtualBoundaries(
            IList<IDetector> detectors,
            ITissue tissue)  
        {
            var VBList = new List<IVirtualBoundary>() { };

            // how to instantiate only those needed?
            IVirtualBoundary reflectionVB = null;
            IVirtualBoundary transmissionVB = null;
            IVirtualBoundary specularVB = null;
            IVirtualBoundary genericVB = null;

            foreach (var detector in detectors)
            {
                switch (detector.TallyType)
                {
                    default:
                    // ITerminationDetector(s):
                    case TallyType.RDiffuse:
                    case TallyType.ROfRho:
                    case TallyType.ROfAngle:
                    case TallyType.ROfRhoAndTime:
                    case TallyType.ROfRhoAndAngle:
                    case TallyType.ROfXAndY:
                    case TallyType.ROfRhoAndOmega:
                        if (reflectionVB == null)
                        {
                            reflectionVB = new PlanarTransmissionVirtualBoundary(
                                VirtualBoundaryAxisType.Z,
                                VirtualBoundaryDirectionType.Decreasing,
                                0.0,
                                VirtualBoundaryType.PlanarTransmissionDomainTopBoundary,
                                VirtualBoundaryType.PlanarTransmissionDomainTopBoundary.ToString());
                            VBList.Add(reflectionVB);
                        }
                        reflectionVB.DetectorController.Detectors.Add(detector);
                        break; 
                    case TallyType.TDiffuse:
                    case TallyType.TOfAngle:
                    case TallyType.TOfRho:
                    case TallyType.TOfRhoAndAngle:
                    // DC: this only works for MultiLayerTissue, need sub case for tissue type?
                        if (transmissionVB == null)
                        {
                            transmissionVB = new PlanarTransmissionVirtualBoundary(
                                VirtualBoundaryAxisType.Z,
                                VirtualBoundaryDirectionType.Increasing,
                                ((LayerRegion)tissue.Regions[tissue.Regions.Count - 1]).ZRange.Stop,
                                VirtualBoundaryType.PlanarTransmissionDomainBottomBoundary,
                                VirtualBoundaryType.PlanarTransmissionDomainBottomBoundary.ToString());
                            VBList.Add(transmissionVB);
                        }
                        transmissionVB.DetectorController.Detectors.Add(detector);
                        break;
                    case TallyType.RSpecular:
                        if (specularVB == null)
                        {
                            specularVB = new PlanarReflectionVirtualBoundary(
                                VirtualBoundaryAxisType.Z,
                                VirtualBoundaryDirectionType.Decreasing,
                                0.0,
                                VirtualBoundaryType.PlanarReflectionDomainTopBoundary,
                                VirtualBoundaryType.PlanarReflectionDomainTopBoundary.ToString());
                            VBList.Add(specularVB);
                        }
                        specularVB.DetectorController.Detectors.Add(detector);
                        break;
                    // IHistoryDetector(s):
                    case TallyType.FluenceOfRhoAndZ:
                    case TallyType.FluenceOfRhoAndZAndTime:
                    case TallyType.AOfRhoAndZ:
                    case TallyType.ATotal:
                    if (genericVB == null)
                        {
                            genericVB = new GenericVolumeVirtualBoundary(
                                VirtualBoundaryType.GenericVolumeBoundary,
                                VirtualBoundaryType.GenericVolumeBoundary.ToString());
                            VBList.Add(genericVB);
                        }
                        genericVB.DetectorController.Detectors.Add(detector);
                        break; 
                }
            }
            return VBList;
        }

        // pMC overload - not sure need yet
        //public static IpMCTerminationDetector GetpMCDetector(
        //    IpMCDetectorInput detectorInput,
        //    ITissue tissue,
        //    bool tallySecondMoment)
        //{
        //    switch (detectorInput.TallyType)
        //    {
        //        default:
        //        case TallyType.pMCROfRhoAndTime:
        //        case TallyType.pMCROfRho:
        //     }
        //}
 
        public static IHistoryDetector GetHistoryDetector(
            IDetectorInput detectorInput,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices)
        {
            throw new NotSupportedException("not implemented yet");
        }
    }
}
