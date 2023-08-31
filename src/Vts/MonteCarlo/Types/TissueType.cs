namespace Vts.MonteCarlo
{
    /// <summary>
    /// Tissue types
    /// </summary>
    public static class TissueType
    {
        /// <summary>
        /// built in type for tissue
        /// </summary>
        public static readonly string[] BuiltInTypes = new []
        {
            // SemiInfinite tissue type.  Includes homogeneous tissues.
            "SemiInfinite",
            // Multilayer tissue type.  Includes homogeneous tissues.
            "MultiLayer",
            // Tissue slab with embedded ellipsoid
            "SingleEllipsoid",
            // Tissue slab with embedded cylinder
            "SingleCylinder",
            // Tissue slab with multiple embedded ellipsoids
            "MultiEllipsoid", 
            // Tissue slab with embedded voxel
            "SingleVoxel",
            // Tissue slab with embedded infinite cylinder
            "SingleInfiniteCylinder",
            // MultiLayer tissue with a surface fiber circle with different OPs
            "MultiLayerWithSurfaceFiber",
            // Multiple (2 right now) concentric infinite cylinder
            "MultiConcentricInfiniteCylinder",
            // Multilayer tissue bounded by vertical cylinder laterally
            "BoundingCylinder",
            // Multilayer tissue bounded by voxel laterally
            "BoundingVoxel"
        };
    }
}