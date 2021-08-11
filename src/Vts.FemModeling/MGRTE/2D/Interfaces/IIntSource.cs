using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for Internal source classes.
    /// </summary>
    public interface IIntSource
    {
        /// <summary>
        /// Assign mesh arrays for an internal source
        /// </summary>
        /// <param name="amesh"></param>
        /// <param name="ameshLevel"></param>
        /// <param name="smesh"></param>
        /// <param name="smeshLevel"></param>
        /// <param name="level"></param>
        /// <param name="rhs"></param>
        void AssignMeshForIntSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] rhs);
    }
}
