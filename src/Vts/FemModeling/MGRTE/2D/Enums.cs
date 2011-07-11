
namespace Vts.FemModeling.MGRTE._2D
{
    // Multigrid Type enums
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

    // Boundary Source enums
    public enum BoundarySourceType
    {
        NoSource,
        Point_Source,
        Point_Source_ISO,
        Nodal_Value_Source_ISO
    }

    // Internal Source enums
    public enum InternalSourceType
    {
        NoSource,
        Point_Source,
        Point_Source_ISO,
        Nodal_Value_Source_ISO
    }
}