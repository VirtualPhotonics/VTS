using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class ExtLineSource : IExtSource
    {
        public ExtLineSource(
            DoubleRange start,
            DoubleRange end,
            DoubleRange thetaRange)
        {
            start = Start;
            end = End;
            thetaRange = ThetaRange;
        }

        public ExtLineSource()
            : this(
             new DoubleRange(-0.25, 0),
             new DoubleRange(0.25, 0),
             new DoubleRange(-0.5 * Math.PI, 0.5 * Math.PI)) { }

        /// <summary>
        /// Starting coordinates (x,z) of the line
        /// </summary>
        public DoubleRange Start { get; set; }

        /// <summary>
        /// Ending coordinates (x,z) of the line
        /// </summary>
        public DoubleRange End { get; set; }

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
