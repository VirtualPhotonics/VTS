using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Virtual boundary group that contains surface detectors;
    /// </summary>
   public class GenericVolumeGroup : IVirtualBoundaryGroup
   {
       /// <summary>
       /// Surface Boundary Group constructor 
       /// </summary>
       public GenericVolumeGroup(IList<IDetectorInput> detectorInputs, bool writeToDatabase)
       {
           DetectorInputs = detectorInputs;
           WriteToDatabase = writeToDatabase;
           VirtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
       }

       /// <summary>
       /// Surface Boundary Group default constructor provides R(rho) detector list
       /// </summary>
       public GenericVolumeGroup()
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
