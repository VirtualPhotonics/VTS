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
                case VirtualBoundaryType.DiffuseReflectance:
                    return
                        tallyType == TallyType.RDiffuse ||
                        tallyType == TallyType.ROfRho ||
                        tallyType == TallyType.ROfAngle ||
                        tallyType == TallyType.ROfRhoAndTime ||
                        tallyType == TallyType.ROfRhoAndAngle ||
                        tallyType == TallyType.ROfXAndY ||
                        tallyType == TallyType.ROfRhoAndOmega;

                case VirtualBoundaryType.DiffuseTransmittance:
                    return
                        tallyType == TallyType.TDiffuse ||
                        tallyType == TallyType.TOfAngle ||
                        tallyType == TallyType.TOfRho ||
                        tallyType == TallyType.TOfRhoAndAngle;

                case VirtualBoundaryType.SpecularReflectance:
                    return tallyType == TallyType.RSpecular;

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
            // this sql returns all VBs even when only RSpecularDetector in detectors
            var virtualBoundaries =
                from vb in EnumHelper.GetValues<VirtualBoundaryType>() // for each virtual boundary type
                where detectors.Select(d => d.TallyType.AppliesToBoundary(vb)).Any() // where any detectors apply
                let vbDetectors = detectors.Where(d => d.TallyType.AppliesToBoundary(vb)).ToList() // gather the appropriate detectors
                select GetVirtualBoundary(vb, tissue, vbDetectors); // and instantiate the vb with the appropriate detectors
                
            return virtualBoundaries.ToList();
        }

        public static IVirtualBoundary GetVirtualBoundary(
            VirtualBoundaryType vbType, ITissue tissue, IList<IDetector> vbDetectors)
        {
            IVirtualBoundary vb = null;
            // ckh's attempt to fix problem above, doesn't work
            //if (vbDetectors.Count == 0)
            //{
            //    return vb;
            //}

            // todo: predicate defines 
            switch (vbType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                    vb = new DiffuseReflectanceVirtualBoundary(
                        tissue, vbDetectors, VirtualBoundaryType.DiffuseReflectance.ToString());
                    break;
                case VirtualBoundaryType.DiffuseTransmittance:
                    vb = new DiffuseTransmittanceVirtualBoundary(
                        tissue, vbDetectors, VirtualBoundaryType.DiffuseTransmittance.ToString());
                    break;
                case VirtualBoundaryType.SpecularReflectance: 
                    // reflecting off first layer without transporting in medium
                    vb = new SpecularReflectanceVirtualBoundary(
                         tissue, vbDetectors, VirtualBoundaryType.SpecularReflectance.ToString());
                    break;
                case VirtualBoundaryType.GenericVolumeBoundary:
                    vb = new GenericVolumeVirtualBoundary(
                        tissue, vbDetectors, VirtualBoundaryType.GenericVolumeBoundary.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Virtual boundary type not recognized: " + vbType);
            }
            return vb;
        }
    }
}
