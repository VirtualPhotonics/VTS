using System.Collections.Generic;
using System;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.VirtualBoundaries;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Factories
{
    public static class VirtualBoundaryExtensions
    {
        public static bool AppliesToBoundary(this TallyType tallyType, VirtualBoundaryType vbType)
        {
            switch (vbType)
            {
                case VirtualBoundaryType.PlanarReflectionDomainTopBoundary:
                    return
                        tallyType == TallyType.RDiffuse ||
                        tallyType == TallyType.ROfRho ||
                        tallyType == TallyType.ROfAngle ||
                        tallyType == TallyType.ROfRhoAndTime ||
                        tallyType == TallyType.ROfRhoAndAngle ||
                        tallyType == TallyType.ROfXAndY ||
                        tallyType == TallyType.ROfRhoAndOmega;

                case VirtualBoundaryType.PlanarTransmissionDomainBottomBoundary:
                    return
                        tallyType == TallyType.TDiffuse ||
                        tallyType == TallyType.TOfAngle ||
                        tallyType == TallyType.TOfRho ||
                        tallyType == TallyType.TOfRhoAndAngle;

                case VirtualBoundaryType.GenericVolumeBoundary:
                    return
                        tallyType == TallyType.FluenceOfRhoAndZ ||
                        tallyType == TallyType.FluenceOfRhoAndZAndTime ||
                        tallyType == TallyType.AOfRhoAndZ ||
                        tallyType == TallyType.ATotal;
                default:
                    throw new ArgumentOutOfRangeException(tallyType.ToString());
            }
        }
    }

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
            var virtualBoundaries =
                from vb in EnumHelper.GetValues<VirtualBoundaryType>() // for each virtual boundary type
                where detectors.Select(d => d.TallyType.AppliesToBoundary(vb)).Any() // where any detectors apply
                let vbDetectors = detectors.Where(d => d.TallyType.AppliesToBoundary(vb)).ToList() // gather the appropriate detectors
                select GetVirtualBoundary(vb, tissue, vbDetectors); // and instantiate the vb with the appropriate detectors

            return virtualBoundaries.ToList();
        }

        public static IVirtualBoundary GetVirtualBoundary(VirtualBoundaryType vbType, ITissue tissue, IList<IDetector> vbDetectors)
        {
            IVirtualBoundary vb = null;

            // todo: predicate defines 
            switch (vbType)
            {

                case VirtualBoundaryType.DiffuseReflectance:
                    vb = new DiffuseReflectanceVirtualBoundary(tissue, vbDetectors, VirtualBoundaryType.DiffuseReflectance.ToString());
                    break;


                case VirtualBoundaryType.PlanarTransmissionDomainTopBoundary: // aka diffuse reflectance
                    Predicate<PhotonDataPoint> willHitBoundary = dp =>
                        dp.StateFlag.Has(PhotonStateType.Transmitted) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - ((LayerRegion)tissue.Regions[0]).ZRange.Stop) < 10E-16;

                    vb = new PlanarTransmissionVirtualBoundary(
                        willHitBoundary,
                        VirtualBoundaryAxisType.Z,
                        VirtualBoundaryDirectionType.Decreasing,
                        0.0,
                        VirtualBoundaryType.PlanarTransmissionDomainTopBoundary,
                        VirtualBoundaryType.PlanarTransmissionDomainTopBoundary.ToString());
                    break;

                case VirtualBoundaryType.PlanarTransmissionDomainBottomBoundary: // aka diffuse transmittance
                    willHitBoundary = dp =>
                                dp.StateFlag.Has(PhotonStateType.Transmitted) &&
                                dp.Direction.Uz > 0 &&
                                Math.Abs(dp.Position.Z - ((LayerRegion)tissue.Regions[tissue.Regions.Count - 1]).ZRange.Start) < 10E-16;
                    vb = new PlanarTransmissionVirtualBoundary(
                              willHitBoundary,
                              VirtualBoundaryAxisType.Z,
                              VirtualBoundaryDirectionType.Increasing,
                              ((LayerRegion)tissue.Regions[tissue.Regions.Count - 1]).ZRange.Stop,
                              VirtualBoundaryType.PlanarTransmissionDomainBottomBoundary,
                              VirtualBoundaryType.PlanarTransmissionDomainBottomBoundary.ToString());
                    break;
                case VirtualBoundaryType.PlanarReflectionDomainTopBoundary: // aka specular reflectance
                    // reflecting off first layer without transporting in medium
                    willHitBoundary = dp =>
                        dp.StateFlag.Has(PhotonStateType.Reflected) &&
                        dp.Direction.Uz < 0 &&
                        Math.Abs(dp.Position.Z - ((LayerRegion)tissue.Regions[0]).ZRange.Stop) < 10E-16 &&
                        Math.Abs(dp.TotalTime) < 10E-16; // todo: revisit for "off-boundary" sources

                    vb = new PlanarReflectionVirtualBoundary(
                          willHitBoundary,
                          VirtualBoundaryAxisType.Z,
                          VirtualBoundaryDirectionType.Decreasing,
                          0.0,
                          VirtualBoundaryType.PlanarReflectionDomainTopBoundary,
                          VirtualBoundaryType.PlanarReflectionDomainTopBoundary.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Virtual boundary type not recognized: " + vbType);
            }

            foreach (var detector in vbDetectors)
            {
                vb.DetectorController.Detectors.Add(detector);
            }

            return vb;
        }
    }
}
