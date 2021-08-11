using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    ///  Internal 2D Elliptical source
    /// </summary>
    public class Int2DEllipticalSource : IIntSource
    {
        /// <summary>
        /// General constructor for 2D elliptical source
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="center"></param>
        /// <param name="thetaRange"></param>
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

        /// <summary>
        /// Default constructor for 2D elliptical source
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amesh"></param>
        /// <param name="ameshLevel"></param>
        /// <param name="smesh"></param>
        /// <param name="smeshLevel"></param>
        /// <param name="level"></param>
        /// <param name="rhs"></param>
        public void AssignMeshForIntSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] rhs)
        {
        }
    }

}
