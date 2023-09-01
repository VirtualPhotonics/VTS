namespace Vts.MonteCarlo
{
    /// <summary>
    /// Source types defined organized by dimension and geometric type
    /// </summary>
    public static class SourceType
    {
        /// <summary>
        /// built in types for sources
        /// </summary>
        public static readonly string[] BuiltInTypes = new []
            {
                //POINT AND LINE SOURCES

                //Point Sources
                // Point sources: isotropic
                "IsotropicPoint",
                // Point sources: directional point
                "DirectionalPoint",
                // Point sources: custom point
                "CustomPoint",

                // Line Sources
                // 1D Line sources: isotropic line
                "IsotropicLine",
                // 1D Line sources: directional line 
                "DirectionalLine",
                // 1D Line sources: custom
                "CustomLine",
                // 1D Line sources: adjoint line
                "LineAngledFromLine",


                // SURFACE EMITTING FLAT SOURCES (2D SURFACE SOURCES)

                // Circular Surface Sources
                // 2D Circular surface sources: directional 
                "DirectionalCircular",
                // 2D Circular surface sources: custom
                "CustomCircular",
                // 2D Circular source angle determined by point in air
                "CircularAngledFromPoint",
                // 2D Circular source angle determined by circle in air
                "CircularAngledFromCircle",

                // Elliptical Surface Sources
                // 2D Elliptical surface sources: directional 
                "DirectionalElliptical",
                // 2D Elliptical surface sources: custom
                "CustomElliptical",

                // Rectangular Surface Sources
                // 2D Rectangular surface sources: directional
                "DirectionalRectangular",
                // 2D Rectangular surface sources: custom
                "CustomRectangular",
                // 2D Rectangular surface source angle determined by circle in air
                "RectangularAngledFromCircle",
                // 2D Rectangular surface sources: bitmap image
                "DirectionalImage",


                // SURFACE EMITTING BULK SOURCES (3D SURFACE SOURCES)

                // Spherical Surface Sources
                // Spherical surface sources: Lambertian
                "LambertianSurfaceEmittingSpherical", // e.g. change to LambertianSphericalSurface
                // Spherical surface sources: custom
                "CustomSurfaceEmittingSpherical",

                // Cuboidal Surface Sources
                // Cuboidal surface sources: Lambertian surface emitting Cuboidal
                "LambertianSurfaceEmittingCuboidal",

                // Tubular Sources
                // Tubular sources: Lambertian
                "LambertianSurfaceEmittingTubular",

                //Cylindrical Fiber Source
                // Cylindrical fiber sources: Lambertian cylindrial fiber
                "LambertianSurfaceEmittingCylindricalFiber",


                // VOLUMETRIC SOURCES (3D)

                // Cuboidal Volume Sources
                // 3D Cuboidal volume sources: isotropic
                "IsotropicVolumetricCuboidal",
                // 3D Cuboidal volume sources: custom
                "CustomVolumetricCuboidal",

                // Ellipsoidal Volume Sources
                // 3D Ellipsoidal volume sources: isotropic
                "IsotropicVolumetricEllipsoidal",
                // 3D Ellipsoidal volume sources: custom
                "CustomVolumetricEllipsoidal",

                // Fluorescence Emission Volume Sources
                "FluorescenceEmissionAOfXAndYAndZ",
                "FluorescenceEmissionAOfRhoAndZ"

                // ...others, based on Fluence or Radiance?                  
            };
    }
}
