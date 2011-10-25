using System;
using Vts.Common;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for Internal source classes.
    /// </summary>
    public interface IIntSource
    {
        //Assign mesh values for an internal source
        void AssignMeshForIntSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] RHS);
    }
}
