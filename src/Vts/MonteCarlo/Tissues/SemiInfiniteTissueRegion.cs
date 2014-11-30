using Vts.Common;
using System;
using System.Runtime.Serialization;

namespace Vts.MonteCarlo.Tissues
{

    /// <summary>
    /// Implements ITissueRegion.  Defines a layer infinite in extent along
    /// x,y-axes and with extent along z-axis given by ZRange.
    /// </summary>
    public class SemiInfiniteTissueRegion : ITissueRegion
    {
        /// <summary>
        /// constructor for SemiInfiniteTissueRegion
        /// </summary>
        /// <param name="op">optical properties of layer</param>
        public SemiInfiniteTissueRegion(OpticalProperties op)
        {
            RegionOP = op;
            TissueRegionType = "SemiInfinite";
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public SemiInfiniteTissueRegion()
            : this(new OpticalProperties(0.01, 1.0, 0.8, 1.4)) { }

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }

        /// <summary>
        /// optical properties of layer
        /// </summary>
        public OpticalProperties RegionOP { get; set; }

        public Position Center
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This checks which region photon is currently in.  
        /// inclusion defined in half-open interval [start,stop) so that continuum of layers do not overlap.
        /// </summary>
        /// <param name="p">Position being checked</param>
        /// <returns>True if photon in region, false if not</returns>
        public bool ContainsPosition(Position p)
        {
            return p.Z >= 0;
        }

        /// <summary>
        /// Method to determine if photon on layer boundary.  Needed to determine which boundary photon is
        /// on when layer region contains inclusion.  Errors in Position accommodated for in test.
        /// </summary>
        /// <param name="p">Position being checked</param>
        /// <returns>True if photon on boundary, false if not</returns>
        public bool OnBoundary(Position p)
        {
            var onBoundary = false;
            if (Math.Abs(p.Z) < 1e-10)
            {
                onBoundary = true;
            }
            return onBoundary;
        }

        /// <summary>
        /// method to determine if photon track or ray intersects layer boundary
        /// </summary>
        /// <param name="p">Photon</param>
        /// <param name="distanceToBoundary">return distance to boundary</param>
        /// <returns>true if intersection, false otherwise</returns>
        public bool RayIntersectBoundary(Photon p, out double distanceToBoundary)
        {
            throw new System.NotImplementedException(); // currently, implemented by MultiLayerTissue...should revisit so this can be independent
        }
    }
}
