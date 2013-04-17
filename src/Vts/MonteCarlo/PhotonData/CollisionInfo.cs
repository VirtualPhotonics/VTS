using System.Collections.Generic;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes a model object designed to hold extra information related to collisions and
    /// pathlength in each region. Inherits from List(OfSubRegionCollisionInfo) as a simple way 
    /// of having Add() and indexing methods available
    /// </summary>
    public class CollisionInfo : List<SubRegionCollisionInfo>
    {
        /// <summary>
        /// class that holds photon information related to collisions and pathlength in each region
        /// </summary>
        /// <param name="numberOfSubRegions">number of tissue subregions</param>
        public CollisionInfo(int numberOfSubRegions)
            : base(numberOfSubRegions) // sets the initial size to the known number of subregions
        {
        }
        /// <summary>
        /// default constructor assumes 0 tissue regions (used for serialization only)
        /// </summary>
        public CollisionInfo() : this(0) // for serialization only
        {
        }
        /// <summary>
        /// method to cline the CollisionInfo class
        /// </summary>
        /// <returns></returns>
        public CollisionInfo Clone()
        {
            var collisionInfo = new CollisionInfo(this.Capacity);
            foreach (var subRegion in collisionInfo)
            {
                collisionInfo.Add(subRegion.Clone());
            }
            return collisionInfo;
        }
    }
}
