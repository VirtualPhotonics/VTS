using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Internal 2D Rectangular source
    /// </summary>
    public class Int2DRectangularSource : IIntSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xLength"></param>
        /// <param name="zHeight"></param>
        /// <param name="center"></param>
        /// <param name="thetaRange"></param>
        public Int2DRectangularSource(
            double xLength,
            double zHeight,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            XLength = xLength;
            ZHeight = zHeight;
            Center = center;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int2DRectangularSource()
            : this(
                0.5,
                0.5,
                new DoubleRange(0.5, 0.5),
                new DoubleRange(0, 2 * Math.PI)) { }

        /// <summary>
        /// Length of the Rectangular source 
        /// </summary>
        public double XLength { get; set; }
        /// <summary>
        /// Height of the Rectangular source 
        /// </summary>
        public double ZHeight { get; set; }
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
