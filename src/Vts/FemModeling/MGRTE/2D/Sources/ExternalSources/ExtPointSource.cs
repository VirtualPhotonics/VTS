using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class ExtPointSource : IExtSource
    {
        public ExtPointSource(
            DoubleRange launchPoint,
            DoubleRange thetaRange)
        {
            launchPoint = LaunchPoint;
            thetaRange = ThetaRange;
        }

        public ExtPointSource()
            : this(
             new DoubleRange(0, 0),
             new DoubleRange(-0.5 * Math.PI, 0.5 * Math.PI)) { }

        /// <summary>
        /// Launching coordinates (x,z) of the line
        /// </summary>
        public DoubleRange LaunchPoint { get; set; }

        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }


        public void AssignMeshForExtSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] q)
        {
            int test;
        }
    }

}
