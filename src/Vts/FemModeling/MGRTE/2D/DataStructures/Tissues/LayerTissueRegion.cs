using Vts.Common;
using System;
using System.Runtime.Serialization;

namespace Vts.FemModeling.MGRTE._2D
{

    /// <summary>
    /// Implements ITissueRegion.  Defines a layer infinite in extent along
    /// x,y-axes and with extent along z-axis given by ZRange.
    /// </summary>
    public class LayerTissueRegion : ITissueRegion
    {
        /// <summary>
        /// constructor for layer region
        /// </summary>
        /// <param name="zRange">specifies extent of layer</param>
        /// <param name="op">optical properties of layer</param>
        public LayerTissueRegion(DoubleRange zRange, OpticalProperties op)
        {
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
        /// extent of z layer
        /// </summary>
        public DoubleRange ZRange { get; set; }
        /// <summary>
        /// optical properties of layer
        /// </summary>
        public OpticalProperties RegionOP { get; set; }


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
            bool onBoundary = Math.Abs(p.Z - ZRange.Start) < 1e-10 || Math.Abs(p.Z - ZRange.Stop) < 1e-10;
            return onBoundary;
        }

       
    }
}
