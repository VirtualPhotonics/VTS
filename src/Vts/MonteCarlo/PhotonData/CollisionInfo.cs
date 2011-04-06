using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

namespace Vts.MonteCarlo.PhotonData
{
    public class CollisionInfo : List<SubRegionCollisionInfo>
    {
        public CollisionInfo(int numberOfSubRegions)
            : base(numberOfSubRegions) // sets the initial size to the known number of subregions
        {
        }
    }
}
