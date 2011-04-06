using System.Collections.Generic;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes database for storing and returning collision info (pathlength & number of collisions).
    /// The base class, Database(OfT,OfTSelfReferencingType), exposes the IEnumerable(OfT) list of CollisionInfo objects
    /// </summary>
    public class CollisionInfoDatabase : Database<CollisionInfo, CollisionInfoDatabase> 
        // note: the second argument is just to help the base class figure stuff out, and should be identical to the class name
    {
        public CollisionInfoDatabase(long numPhotons, int numSubRegions)
        {
            NumberOfPhotons = numPhotons;
            NumberOfSubRegions = numSubRegions;

            // BinaryReader is static, so we only need to 
            if (BinaryReader != null)
            {
                BinaryReader = new CollisionInfoSerializer(numSubRegions);
            }
        }

        public CollisionInfoDatabase() : this(1000000, 3) { } // only needed for serialization

        public long NumberOfPhotons { get; set; }
        public int NumberOfSubRegions { get; set; }
    }
}
