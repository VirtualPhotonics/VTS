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
        public LayerTissueRegion(DoubleRange zRange, OpticalProperties op, string phaseFunctionKey)
        {
            TissueRegionType = "Layer";
            ZRange = zRange;
            RegionOP = op;
            PhaseFunctionKey = phaseFunctionKey;
        }

        /// <summary>
        /// constructor for layer region
        /// </summary>
        /// <param name="zRange">specifies extent of layer</param>
        /// <param name="op">optical properties of layer</param>
        public LayerTissueRegion(DoubleRange zRange, OpticalProperties op)
            : this(zRange, op, "HenyeyGreensteinKey1")
        {
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public LayerTissueRegion()
            : this(
                new DoubleRange(0.0, 10),
                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                "HenyeyGreensteinKey1") { }
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
        /// key for the <string, IPhaseFunctionInput> dictionary in a class that implements ITissueInput
        /// </summary>
        public string PhaseFunctionKey { get; set; }

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
        /// Method to determine if photon track or ray intersects layer boundary.
        /// Note: LayerTissueRegion.RayIntersectBoundary does NOT use photon.S in the calculation
        /// so all photons (unless horizontal) intersect the top or bottom of the layer and the
        /// returned distance is not infinity (unless horizontal).  This is because MonteCarloSimulation
        /// uses the distance to the boundary without intersection in its processing.
        /// This is different that all other TissueRegions.  
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <param name="distanceToBoundary">return: distance to boundary, actual distance if no intersection</param>
        /// <returns>true if intersection, false otherwise</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            if (photon.DP.Direction.Uz == 0.0)
            {
                distanceToBoundary =  double.PositiveInfinity;
                return false;
            }
            // going "up" in negative z-direction
            bool goingUp = photon.DP.Direction.Uz < 0.0;
            distanceToBoundary =
                goingUp
                    ? (ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    : (ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;
            return true;
        }

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
