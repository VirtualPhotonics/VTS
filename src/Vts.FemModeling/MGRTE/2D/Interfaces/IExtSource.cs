using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for Ext source classes.
    /// </summary>
    public interface IExtSource
    {
        /// <summary>
        /// Assign mesh arrays for an external source
        /// </summary>
        /// <param name="amesh"></param>
        /// <param name="ameshLevel"></param>
        /// <param name="smesh"></param>
        /// <param name="smeshLevel"></param>
        /// <param name="level"></param>
        /// <param name="q"></param>
        void AssignMeshForExtSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] q);
    }
}
