
namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Multigrid Type
    /// </summary>
    public enum WhichMultiGridType
    {
        /// <summary>
        /// Angular MG
        /// </summary>
        MgA = 1,
        /// <summary>
        /// Spatial MG
        /// </summary>
        MgS,
        /// <summary>
        /// MG Method 1
        /// </summary>
        Mg1,
        /// <summary>
        /// MG Method 2
        /// </summary>
        Mg2,
        /// <summary>
        /// MG Method 3
        /// </summary>
        Mg3,
        /// <summary>
        /// Angular -> Spatial MG
        /// </summary>
        Mg4A,
        /// <summary>
        /// Spatial -> Angular MG
        /// </summary>
        Mg4S
    }

    /// <summary>
    /// Source type
    /// </summary>
    public enum FemSourceType
    {
        /// <summary>
        /// External line Source
        /// </summary>
        ExtLineSource,
        /// <summary>
        /// External Point source
        /// </summary>
        ExtPointSource,
        /// <summary>
        /// Internal 2D Circular Source
        /// </summary>
        Int2DCircularSource,
        /// <summary>
        /// Internal 2D Elliptical Source
        /// </summary>
        Int2DEllipticalSource,
        /// <summary>
        /// Internal 2D Rectangular Source
        /// </summary>
        Int2DRectangularSource,
        /// <summary>
        /// Internal 2D Point Source
        /// </summary>
        Int2DPointSource
    }

    /// <summary>
    /// Tally type
    /// </summary>
    public enum TallyType
    {
        /// <summary>
        /// Internal fluence distribution 
        /// </summary>
        FluenceOfXAndZ,
    }

    ///// <summary>
    ///// Tissue type
    ///// </summary>
    //public enum TissueType
    //{
    //    /// <summary>
    //    /// Multilayer tissue type.  Includes homogeneous tissues.
    //    /// </summary>
    //    MultiLayer,
    //}

    /// <summary>
    /// Inclusion type
    /// </summary>
    public enum InclusionType
    {
        /// <summary>
        /// Circular inclusion in multi layer
        /// </summary>
        MultiLayerCircular,
    }
}