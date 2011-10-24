
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

    public enum FemSourceType
    {
        ExtLineSource,
        ExtPointSource,
        Int2DCircularSource,
        Int2DEllipticalSource,
        Int2DRectangularSource,
        Int2DPointSource
    }
}