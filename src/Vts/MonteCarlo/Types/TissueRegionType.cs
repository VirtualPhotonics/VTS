namespace Vts.MonteCarlo
{
    /// <summary>
    /// Tissue region type, sub-regions of layers
    /// </summary>
    public static class TissueRegionType
    {
        /// <summary>
        /// built in types for tissue regions
        /// </summary>
        public static readonly string[] BuiltInTypes = new []
        {
            // voxel tissue region
            "Voxel",
            // capless voxel tissue region
            "CaplessVoxel",
            // layer tissue region
            "Layer",
            // ellipsoid tissue region
            "Ellipsoid",
            // cylinder tissue region
            "Cylinder",
            // capless cylinder tissue region
            "CaplessCylinder",
            // infinite cylinder tissue region
            "InfiniteCylinder",
            // surface fiber tissue region 
            "SurfaceFiber"
        };
    }
}