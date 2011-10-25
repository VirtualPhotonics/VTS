using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Int2DEllipticalSource : IIntSource
    {
        public Int2DEllipticalSource(
           double aParameter,
           double bParameter,
           DoubleRange center,
           DoubleRange thetaRange)
        {
            AParameter = aParameter;
            BParameter = bParameter;
            Center = center;
            ThetaRange = thetaRange;
        }

        public Int2DEllipticalSource()
            : this(
             0.5,
             0.25,
             new DoubleRange(0.5, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }

        /// <summary>
        /// a Parameter of Elliptical source 
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// b Parameter of Elliptical source 
        /// </summary>
        public double BParameter { get; set; }
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
