using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Int2DPointSource : IIntSource
    {
        public Int2DPointSource(
            DoubleRange center,
            DoubleRange thetaRange)
        {
            Center = center;
            ThetaRange = thetaRange;
        }

        public Int2DPointSource()
            : this(
             new DoubleRange(0.5, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }

        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        public DoubleRange Center { get; set; }
        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }


        public void AssignMeshForIntSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] RHS)
        {
            int test;
        }

    }
}
