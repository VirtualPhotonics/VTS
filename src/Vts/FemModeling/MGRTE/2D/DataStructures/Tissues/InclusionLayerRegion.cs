using Vts.Common;
using System;
using System.Runtime.Serialization;

namespace Vts.FemModeling.MGRTE._2D
{

    /// <summary>
    /// Implements ITissueRegion.  Defines a layer infinite in extent along
    /// x,y-axes and with extent along z-axis given by ZRange.
    /// </summary>
    public class InclusionLayerRegion : IInclusionRegion
    {
        /// <summary>
        /// constructor for layer region
        /// </summary>
        /// <param name="zRange">specifies extent of layer</param>
        /// <param name="op">optical properties of layer</param>
        public InclusionLayerRegion(OpticalProperties op, Position loc, double rad)
        {
            RegionOP = op;
            Location = loc;
            Radius = rad;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public InclusionLayerRegion()
            : this(
                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                new Position( 0.0, 0.0, 0.01),
                0.01) { }
        
        /// <summary>
        /// optical properties of layer
        /// </summary>
        public OpticalProperties RegionOP { get; set; }
        
        /// <summary>
        /// Radius of inclusion
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Location of inclusion
        /// </summary>
        public Position Location { get; set;}
    

       
    }
}
