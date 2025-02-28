using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo
{

    /// <summary>
    /// Implements various commonly used SourceInput classes.
    /// </summary>
    public class SourceInputProvider
    {
        /// <summary>
        /// Method that provides instances of all inputs in this class.
        /// </summary>
        /// <returns>a list of the ISourceInputs generated</returns>
        public static IList<ISourceInput> GenerateAllSourceInputs()
        {
            return new List<ISourceInput>
            {
                CustomPointSourceInput(),
                DirectionalPointSourceInput(),
                IsotropicPointSourceInput(),
                LambertianPointSourceInput(),
                FlatLineSourceInput(),
                GaussianLineSourceInput(),
                IsotropicLineSourceInput(),
                LambertianLineSourceInput(),
                LambertianSphericalSourceInput(),
                LambertianCuboidalSourceInput(),
                LambertianTubularSourceInput(),
                GaussianCircularSourceInput(),
                LambertianCircularSourceInput(),
                GaussianEllipticalSourceInput(),
                LambertianEllipticalSourceInput(),
                GaussianRectangularSourceInput(),
                LambertianRectangularSourceInput()
            };
        }

        #region PointAndLine: Custom Point source input
        /// <summary>
        /// Point source pointed along normal into tissue
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput CustomPointSourceInput()
        {
            return new CustomPointSourceInput(
                new DoubleRange(0.0, 0.0),
                new DoubleRange(0.0, 0.0),
                new Position(0.0, 0.0, 0.0),
                new Direction(0.0, 0.0, 1.0),
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Directional Point source input
        /// <summary>
        /// Point source pointed along normal into
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput DirectionalPointSourceInput()
        {
            return new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Isotropic Point source input
        /// <summary>
        /// Point source pointed along normal into
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput IsotropicPointSourceInput()
        {
            return new IsotropicPointSourceInput(
                new Position(0.0, 0.0, 0.0),
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Lambertian Point source input
        /// <summary>
        /// Point source pointed along normal into
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianPointSourceInput()
        {
            return new LambertianPointSourceInput(
                new Position(0.0, 0.0, 0.0),
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Flat Line source input
        /// <summary>
        /// Line source normally oriented and flat
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput FlatLineSourceInput()
        {
            return new CustomLineSourceInput(
                    10.0, // line length
                    new FlatSourceProfile(),
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Gaussian Line source input
        /// <summary>
        /// Line source normally oriented with Gaussian distribution
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput GaussianLineSourceInput()
        {
            return new CustomLineSourceInput(
                10.0, // line length
                new GaussianSourceProfile(),
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Isotropic Line source input
        /// <summary>
        /// Line source normally oriented with Isotropic distribution
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput IsotropicLineSourceInput()
        {
            return new IsotropicLineSourceInput(
                10.0, // line length
                new GaussianSourceProfile(),
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Lambertian Line source input
        /// <summary>
        /// Line source normally oriented with Lambertian distribution
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianLineSourceInput()
        {
            return new LambertianLineSourceInput(
                10.0, // line length
                new FlatSourceProfile(),
                1,
                new Direction(0.0, 0.0, 1.0), // direction principal axis
                new Position(0.0, 0.0, 0.0), // translation
                new PolarAzimuthalAngles(0, 0), 
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingBulk: Lambertian spherical input
        /// <summary>
        /// Lambertian spherical source
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianSphericalSourceInput()
        {
            return new LambertianSurfaceEmittingSphericalSourceInput(
                3.0, // radius
                1,
                new Position(0, 0, 0), // center of beam on surface
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingBulk: Lambertian cuboidal input
        /// <summary>
        /// Lambertian cuboidal source
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianCuboidalSourceInput()
        {
            return new LambertianSurfaceEmittingCuboidalSourceInput(
                3.0, // lengthX
                2.0, // widthY
                1.0, // heightZ
                new FlatSourceProfile(),
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0);
        }
        #endregion

        #region SurfaceEmittingBulk: Lambertian tubular input
        /// <summary>
        /// Lambertian tubular source
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianTubularSourceInput()
        {
            return new LambertianSurfaceEmittingTubularSourceInput(
                1.0, // tubeRadius
                1.0, // tubeHeight
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0);
        }
        #endregion

        #region SurfaceEmittingFlat: Gaussian circular source input
        /// <summary>
        /// Gaussian normal source with fwhm=1 and outer radius=3mm
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput GaussianCircularSourceInput()
        {
            return new CustomCircularSourceInput(
                    3.0, // outer radius
                    0.0, // inner radius
                    new GaussianSourceProfile(1.0), // fwhm
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingFlat: Lambertian circular source input
        /// <summary>
        /// Flat normal source with outer radius=3mm
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianCircularSourceInput()
        {
            return new LambertianCircularSourceInput(
                3.0, // outer radius
                0.0, // inner radius
                new GaussianSourceProfile(1.0), // fwhm
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                1, // Lambert order
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingFlat: Gaussian elliptical source input
        /// <summary>
        /// Gaussian normal source with fwhm=1 and ellipse parameters a and b
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput GaussianEllipticalSourceInput()
        {
            return new CustomEllipticalSourceInput(
                3.0, // a parameter
                2.0, // b parameter
                new GaussianSourceProfile(1.0), // fwhm
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingFlat: Lambertian elliptical source input
        /// <summary>
        /// Gaussian normal source with fwhm=1 and ellipse parameters a and b
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianEllipticalSourceInput()
        {
            return new LambertianEllipticalSourceInput(
                3.0, // a parameter
                2.0, // b parameter
                new GaussianSourceProfile(1.0), // fwhm
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                1, // Lambert order
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingFlat: Gaussian rectangular source input
        /// <summary>
        /// Gaussian normal source with fwhm=1 and rectangular length and width
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput GaussianRectangularSourceInput()
        {
            return new CustomRectangularSourceInput(
                3.0, // a parameter
                2.0, // b parameter
                new GaussianSourceProfile(1.0), // fwhm
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingFlat: Lambertian rectangular source input
        /// <summary>
        /// Gaussian normal source with fwhm=1 and rectangular length and width
        /// </summary>
        /// <returns>source input class that implements ISourceInput</returns>
        public static ISourceInput LambertianRectangularSourceInput()
        {
            return new LambertianRectangularSourceInput(
                3.0, // a parameter
                2.0, // b parameter
                new GaussianSourceProfile(1.0), // fwhm
                new DoubleRange(0.0, 0.0), // polar angle emission range
                new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                1, // Lambert order
                new Direction(0, 0, 1), // normal to tissue
                new Position(0, 0, 0), // center of beam on surface
                new PolarAzimuthalAngles(0, 0), // no beam rotation         
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

    }
}
