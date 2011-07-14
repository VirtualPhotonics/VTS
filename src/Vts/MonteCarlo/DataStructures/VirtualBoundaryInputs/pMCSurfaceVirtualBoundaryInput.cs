using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Virtual boundary group that contains surface detectors;
    /// </summary>
   public class pMCSurfaceVirtualBoundaryInput //: IVirtualBoundaryGroup need to add IpMCVirtualBoundaryGroup?
   {
       /// <summary>
       /// Surface Boundary Group constructor 
       /// </summary>
       public pMCSurfaceVirtualBoundaryInput(
           VirtualBoundaryType type, 
           IList<IpMCDetectorInput> detectorInputs, 
           bool writeToDatabase, 
           string name)
       {
           DetectorInputs = detectorInputs;
           WriteToDatabase = writeToDatabase;
           VirtualBoundaryType = type;
           Name = name;
       }

       /// <summary>
       /// Surface Boundary Group default constructor provides R(rho) detector list
       /// </summary>
       public pMCSurfaceVirtualBoundaryInput()
           : this(
           VirtualBoundaryType.pMCDiffuseReflectance,
               new List<IpMCDetectorInput> 
                { 
                    new pMCROfRhoDetectorInput(),
                },
               false,
               VirtualBoundaryType.pMCDiffuseReflectance.ToString())
       {
       }

       public IList<IpMCDetectorInput> DetectorInputs { get; set; }
       public bool WriteToDatabase { get; set; }
       public VirtualBoundaryType VirtualBoundaryType { get; set; }
       public string Name { get; set; }
   }
}
