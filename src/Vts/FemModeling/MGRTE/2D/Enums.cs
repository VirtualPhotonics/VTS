
namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Multigrid Type
    /// </summary>
    public enum WhichMultiGridType
    {
        AMG = 1,
        SMG,
        MG1,
        MG2,
        MG3,
        MG4_a,
        MG4_s
    }

    /// <summary>
    /// Boundary Source enums
    /// </summary>
    public enum BoundarySourceType
    {
        NoSource,
        Point_Source,
        Point_Source_ISO,
        Nodal_Value_Source_ISO
    }

    /// <summary>
    /// Internal Source enums
    /// </summary>
    public enum InternalSourceType
    {
        NoSource,
        Point_Source,
        Point_Source_ISO,
        Nodal_Value_Source_ISO
    }
}