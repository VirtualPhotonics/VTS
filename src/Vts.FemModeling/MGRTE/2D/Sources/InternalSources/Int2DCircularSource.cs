using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    ///  Internal 2D Circular source
    /// </summary>
    public class Int2DCircularSource : IIntSource
    {
        /// <summary>
        /// General constructor for 2D circular source
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="center"></param>
        /// <param name="thetaRange"></param>
        public Int2DCircularSource(
            double radius,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            Radius = radius;
            Center = center;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for 2D circular source
        /// </summary>
        public Int2DCircularSource()
            : this(
             0.5,
             new DoubleRange(0.5, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }

        /// <summary>
        /// Radius of the circular source 
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        public DoubleRange Center { get; set; }

        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }

        public void AssignMeshForIntSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] rhs)
        {
        }

    }
}
