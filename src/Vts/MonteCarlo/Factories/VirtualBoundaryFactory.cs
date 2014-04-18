using System;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.MonteCarlo.Factories
{
    public static class VirtualBoundaryExtensions
    {
        /// <summary>
        /// method to determine which tally type belongs to which virtual boundary
        /// </summary>
        /// <param name="tallyType">TallyType</param>
        /// <param name="vbType">VirtualBoundaryTyp</param>
        /// <returns>boolean indicating whether tally type belongs to VB</returns>
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
                        tallyType == TallyType.ROfRhoAndOmega ||
                        tallyType == TallyType.ReflectedMTOfRhoAndSubregionHist;

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
                        tallyType == TallyType.FluenceOfXAndYAndZ ||
                        tallyType == TallyType.AOfRhoAndZ ||
                        tallyType == TallyType.ATotal;

                case VirtualBoundaryType.SurfaceRadiance:
                    return
                        tallyType == TallyType.RadianceOfRho;

                default:
                    throw new ArgumentOutOfRangeException(tallyType.ToString());
            }
        }
    }

    /// <summary>
    /// Instantiates appropriate virtual boundary.
    /// </summary>
    public class VirtualBoundaryFactory
    {
    //    /// <summary>
    //    /// This static method instantiates a list of virtual boundaries
    //    /// and the appropriate detectors are deposited into it.  
    //    /// </summary>
    //    /// <param name="detectors"></param>
    //    /// <returns></returns>
    //    public static IList<IVirtualBoundary> GetVirtualBoundaries(
    //        IList<IVirtualBoundaryInput> virtualBoundaryGroups, ITissue tissue, bool tallySecondMoment)
    //    {
    //        // this sql returns all VBs even when only RSpecularDetector in detectors
    //        //var virtualBoundaries =
    //        //    from vb in EnumHelper.GetValues<VirtualBoundaryType>() // for each virtual boundary type
    //        //    where detectors.Select(d => d.TallyType.AppliesToBoundary(vb)).First()  // where any detectors apply
    //        //    let vbDetectors = detectors.Where(d => d.TallyType.AppliesToBoundary(vb)).ToList() // gather the appropriate detectors
    //        //    select GetVirtualBoundary(vb, tissue, vbDetectors); // and instantiate the vb with the appropriate detectors

    //        var virtualBoundaries = new List<IVirtualBoundary>();
    //        foreach (var vbg in virtualBoundaryGroups)
    //        {
    //            var detectors = DetectorFactory.GetDetectors(
    //                vbg.DetectorInputs, tissue, tallySecondMoment);
                   
    //            var detectorController = DetectorControllerFactory.GetDetectorController(
    //                vbg.VirtualBoundaryType, detectors);
    //            if (detectors.Count > 0)
    //            {
    //                var vb = GetVirtualBoundary(vbg.VirtualBoundaryType, tissue, detectorController);
    //                if (vb != null)
    //                    virtualBoundaries.Add(vb);
    //            }
    //        }
    //        return virtualBoundaries.ToList();
    //    }
        /// <summary>
        /// method that gets appropriate VB 
        /// </summary>
        /// <param name="vbType">VirtualBoundaryType</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <returns>IVirtualBoundary</returns>
        public static IVirtualBoundary GetVirtualBoundary(
            VirtualBoundaryType vbType, ITissue tissue, IDetectorController detectorController)
        {
            IVirtualBoundary vb = null;

            // todo: predicate defines 
            switch (vbType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                    vb = new DiffuseReflectanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseReflectance.ToString());
                    break;
                case VirtualBoundaryType.DiffuseTransmittance:
                    vb = new DiffuseTransmittanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseTransmittance.ToString());
                    break;
                case VirtualBoundaryType.SpecularReflectance:
                    // reflecting off first layer without transporting in medium
                    vb = new SpecularReflectanceVirtualBoundary(
                         tissue, detectorController, VirtualBoundaryType.SpecularReflectance.ToString());
                    break;
                case VirtualBoundaryType.SurfaceRadiance:
                    vb = new RadianceVirtualBoundary(
                        detectorController, VirtualBoundaryType.SurfaceRadiance.ToString());
                    break;
                case VirtualBoundaryType.GenericVolumeBoundary:
                    vb = new GenericVolumeVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.GenericVolumeBoundary.ToString());
                    break;
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    vb = new pMCDiffuseReflectanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseReflectance.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Virtual boundary type not recognized: " + vbType);
            }
            return vb;
        }
    }
}
