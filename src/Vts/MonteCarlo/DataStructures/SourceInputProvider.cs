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
                LineSource(),
                GaussianSource()
            };
        }

        #region Point source
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

        #region Line source
        /// <summary>
        /// Line source normally oriented
        /// </summary>
        public static ISourceInput LineSource()
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

        #region Gaussian source 
        /// <summary>
        /// Gaussian normal source with fwhm=1 and outer radius=3mm
        /// </summary>
        public static ISourceInput GaussianSource()
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

    }
}
