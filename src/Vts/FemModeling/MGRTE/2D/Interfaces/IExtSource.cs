using System;
using Vts.Common;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for Ext source classes.
    /// </summary>
    public interface IExtSource
    {
        //Assign mesh values for an external source
        void AssignMeshForExtSource(AngularMesh[] amesh, int ameshLevel, SpatialMesh[] smesh, int smeshLevel, int level, double[][][][] q);
    }
}
