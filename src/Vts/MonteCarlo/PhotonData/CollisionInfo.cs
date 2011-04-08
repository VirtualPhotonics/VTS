using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describse a model object designed to hold extra information related to collisions and
    /// pathlength in each region. Inherits from List(OfSubRegionCollisionInfo) as a simple way 
    /// of having Add() and indexing methods available
    /// </summary>
    public class CollisionInfo : List<SubRegionCollisionInfo>
    {
        public CollisionInfo(int numberOfSubRegions)
            : base(numberOfSubRegions) // sets the initial size to the known number of subregions
        {
        }

        public CollisionInfo() : this(0) // for serialization only
        {
        }
    }
}
