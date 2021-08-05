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
            return new List<ISourceInput>()
            {
                PointSource(),
                FlatLineSource(),
                GaussianCircularSource(),
                GaussianEllipticalSource(),
                GaussianRectangularSource(),
                LambertianSphericalSource(),
                LambertianCuboidalSource(),
                LambertianTubularSource()
            };
        }

        #region PointAndLine: Point source
        /// <summary>
        /// Point source normally oriented
        /// </summary>
        /// <returns></returns>
        public static ISourceInput PointSource()
        {
            return new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region PointAndLine: Flat Line source
        /// <summary>
        /// Line source normally oriented and flat
        /// </summary>
        public static ISourceInput FlatLineSource()
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

        #region SurfaceEmittingFlat: Gaussian circular source 
        /// <summary>
        /// Gaussian normal source with fwhm=1 and outer radius=3mm
        /// </summary>
        public static ISourceInput GaussianCircularSource()
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

        #region SurfaceEmittingFlat: Gaussian elliptical source 
        /// <summary>
        /// Gaussian normal source with fwhm=1 and ellipse parameters a and b
        /// </summary>
        public static ISourceInput GaussianEllipticalSource()
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

        #region SurfaceEmittingFlat: Gaussian rectangular source 
        /// <summary>
        /// Gaussian normal source with fwhm=1 and rectangular length and width
        /// </summary>
        public static ISourceInput GaussianRectangularSource()
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

        #region SurfaceEmittingBulk: Lambertian spherical
        /// <summary>
        /// Lambertian spherical source
        /// </summary>
        public static ISourceInput LambertianSphericalSource()
        {
            return new LambertianSurfaceEmittingSphericalSourceInput(
                3.0, // radius
                new Position(0, 0, 0), // center of beam on surface
                0); // 0=start in air, 1=start in tissue
        }
        #endregion

        #region SurfaceEmittingBulk: Lambertian cuboidal
        /// <summary>
        /// Lambertian cuboidal source
        /// </summary>
        public static ISourceInput LambertianCuboidalSource()
        {
            return new LambertianSurfaceEmittingCuboidalSourceInput(
                3.0, // lengthX
                2.0, // widthY
                1.0, // heightZ
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0);
        }
        #endregion

        #region SurfaceEmittingBulk: Lambertian tubular
        /// <summary>
        /// Lambertian tubular source
        /// </summary>
        public static ISourceInput LambertianTubularSource()
        {
            return new LambertianSurfaceEmittingTubularSourceInput(
                1.0, // tubeRadius
                1.0, // tubeHeight
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0);
        }
        #endregion

    }
}
