using Vts.Common;
using System;
using System.Runtime.Serialization;

namespace Vts.FemModeling.MGRTE._2D
{

    /// <summary>
    /// Implements IInclusionRegion.  Defines a circular inclusion
    /// </summary>
    public class LayerInclusionRegion : IInclusionRegion
    {
        /// <summary>
        /// constructor for layer region
        /// </summary>
        /// <param name="pos">position</param>
        /// <param name="rad">radius</param>
        /// <param name="op">optical properties of layer</param>
        public LayerInclusionRegion(OpticalProperties op, Position pos, double rad)
        {
            RegionOP = op;
            Position = pos;
            Radius = rad;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public LayerInclusionRegion()
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
        /// Position of inclusion
        /// </summary>
        public Position Position { get; set;}
    

       
    }
}
