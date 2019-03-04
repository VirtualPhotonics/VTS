using System;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines a layer infinite in extent along
    /// x,y-axes and with extent along z-axis given by ZRange.
    /// </summary>
    public class LayerTissueRegion : ITissueRegion, ILayerOpticalPropertyRegion
    {
        /// <summary>
        /// constructor for layer region
        /// </summary>
        /// <param name="zRange">specifies extent of layer</param>
        /// <param name="op">optical properties of layer</param>
        public LayerTissueRegion(DoubleRange zRange, OpticalProperties op)
        {
            TissueRegionType = "Layer";
            ZRange = zRange;
            RegionOP = op;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public LayerTissueRegion()
            : this(
                new DoubleRange(0.0, 10),
                new OpticalProperties(0.01, 1.0, 0.8, 1.4)) { }

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }

        /// <summary>
        /// extent of z layer
        /// </summary>
        public DoubleRange ZRange { get; set; }
        /// <summary>
        /// optical properties of layer
        /// </summary>
        public OpticalProperties RegionOP { get; set; }

        /// <summary>
        /// center of layer
        /// </summary>
        [IgnoreDataMember]
        public Position Center
        {
            get
            {
                return new Position(
                    0D,
                    0D,
                    (ZRange.Start + ZRange.Stop) / 2);
            }
            set
            {
                var oldCenter = Center;
                var newCenter = value;

                var dz = newCenter.Z - oldCenter.Z;

                ZRange.Start += dz;
                ZRange.Stop += dz;
            }
        }

        /// <summary>
        /// This checks which region photon is currently in.  
        /// inclusion defined in half-open interval [start,stop) so that continuum of layers do not overlap.
        /// </summary>
        /// <param name="p">Position being checked</param>
        /// <returns>True if photon in region, false if not</returns>
        public bool ContainsPosition(Position p)
        {
            return p.Z >= ZRange.Start && p.Z < ZRange.Stop;
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
            if (Math.Abs(p.Z - ZRange.Start) < 1e-10 || Math.Abs(p.Z - ZRange.Stop) < 1e-10)
            {
                onBoundary = true;
            }
            return onBoundary;
        }
        /// <summary>
        /// method to determine normal to surface at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Direction</returns>
        public Direction SurfaceNormal(Position position)
        {
            throw new NotImplementedException();
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

        //public bool RayIntersectBoundary(Photon p)
        //{
        //    throw new System.NotImplementedException(); // currently, implemented by MultiLayerTissue...should revisit so this can be independent
        //}

        //public bool RayExitBoundary(Photon photptr, ref double distanceToBoundary)
        //{
        //    distanceToBoundary = 0.0;  /* distance to boundary */

        //    if (photptr.DP.Direction.Uz < 0.0)
        //        distanceToBoundary = ( Z.Start - photptr.DP.Position.Z) /
        //            photptr.DP.Direction.Uz;
        //    else if (photptr.DP.Direction.Uz > 0.0)
        //        distanceToBoundary = ( Z.Stop - photptr.DP.Position.Z) /
        //            photptr.DP.Direction.Uz;

        //    if ((photptr.DP.Direction.Uz != 0.0) && (photptr.S > distanceToBoundary))
        //    {
        //        //photptr.HitBoundary = true;
        //        ////photptr.SLeft = (photptr.S - distanceToBoundary) * (mua + mus);  // DAW
        //        //photptr.SLeft = (photptr.S - distanceToBoundary) * photptr._tissue.Regions[photptr.CurrentRegionIndex].ScatterLength;
        //        //photptr.S = distanceToBoundary;
        //        return true;
        //    }
        //    else
        //        return false;
        //}
    }
}
