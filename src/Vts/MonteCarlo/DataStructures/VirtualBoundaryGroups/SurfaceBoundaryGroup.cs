using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Virtual boundary group that contains surface detectors;
    /// </summary>
   public class SurfaceBoundaryGroup : IVirtualBoundaryGroup
   {
       /// <summary>
       /// Surface Boundary Group constructor 
       /// </summary>
       public SurfaceBoundaryGroup(IList<IDetectorInput> detectorInputs, bool writeToDatabase)
       {
           DetectorInputs = detectorInputs;
           WriteToDatabase = writeToDatabase;
           VirtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
       }

       /// <summary>
       /// Surface Boundary Group default constructor provides R(rho) detector list
       /// </summary>
       public SurfaceBoundaryGroup()
           : this(
               new List<IDetectorInput> 
                { 
                    new ROfRhoDetectorInput(),
                },
               false)
       {
       }

       public IList<IDetectorInput> DetectorInputs { get; set; }
       public bool WriteToDatabase { get; set; }
       public VirtualBoundaryType VirtualBoundaryType { get; set; }
   }
}
