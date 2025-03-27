using System.IO;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Point sources validation data
    /// </summary>
    public class PointSourcesValidationData
    {
        /// <summary>
        /// acceptable precision when validating values
        /// </summary>
        public double AcceptablePrecision { get; private set; }
        /// <summary>
        /// photon position
        /// </summary>
        public Position Position { get; private set; }
        /// <summary>
        /// photon direction
        /// </summary>
        public Direction Direction { get; private set; }
        /// <summary>
        /// translation from origin
        /// </summary>
        public Position Translation { get; private set; }
        /// <summary>
        /// polar and azimuthal angles
        /// </summary>
        public PolarAzimuthalAngles AngPair { get; private set; }
        /// <summary>
        /// polar angle range
        /// </summary>
        public DoubleRange PolRange { get; private set; }
        /// <summary>
        /// azimuthal range
        /// </summary>
        public DoubleRange AziRange { get; private set; }        
        /// <summary>
        /// line source Lambert Order
        /// </summary>
        public int LambertOrder { get; private set; }
        /// <summary>
        /// rectangular source parameter length along x-axis
        /// </summary>
        public double LengthX { get; private set; }
        /// <summary>
        /// polar angle
        /// </summary>
        public double PolarAngle { get; private set; }
        /// <summary>
        /// Test values
        /// </summary>
        public double[] Tp { get; private set; }

        /// <summary>
        /// Read text data file that has input and output data
        /// </summary>
        public void ReadData()
        {
            AcceptablePrecision = 0.00000001;

            Tp =
            [
                0.0000000000e+00, 0.0000000000e+00, 0.0000000000e+00, 7.0710678119e-01, 7.0710678119e-01,
                0.0000000000e+00, 1.0000000000e+00, -2.5000000000e+00, 1.2000000000e+00, 7.8539816340e-01,
                7.8539816340e-01, 0.0000000000e+00, 1.5707963268e+00, 0.0000000000e+00, 3.1415926536e+00,
                7.8539816340e-01, -1.7806560289e-01, 9.5420510124e-01, 2.4038566061e-01, 1.0000000000e+00,
                -2.5000000000e+00, 1.2000000000e+00, 7.0710678119e-01, 7.0710678119e-01, 6.1232339957e-17,
                1.0000000000e+00, -2.5000000000e+00, 1.2000000000e+00, -8.3062970822e-01, -5.4820001436e-01,
                9.7627004864e-02, 1.0000000000e+00, -2.5000000000e+00, 1.2000000000e+00, -5.6061546050e-01,
                -3.6999567973e-01, 7.4081948033e-01, 1.0000000000e+00, -2.5000000000e+00, 1.2000000000e+00,
            ];
            // if you need to regenerate _tp, run matlab/test/ code Unit_Tests_MonteCarlo_Sources_PointSources
            if (Tp.Length == 0)
            {
                const string testpara = "../../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_PointSources.txt";
                using TextReader reader = File.OpenText(testpara);
                string text = reader.ReadToEnd();
                string[] bits = text.Split('\t');
                for (int i = 0; i < Tp.Length; i++)
                    Tp[i] = double.Parse(bits[i]);
                reader.Close();
            }
            Position = new Position(Tp[0], Tp[1], Tp[2]);
            Direction = new Direction(Tp[3], Tp[4], Tp[5]);
            Translation = new Position(Tp[6], Tp[7], Tp[8]);
            AngPair = new PolarAzimuthalAngles(Tp[9], Tp[10]);
            PolRange = new DoubleRange(Tp[11], Tp[12]);
            AziRange = new DoubleRange(Tp[13], Tp[14]);
            PolarAngle = Tp[15];
            LambertOrder = 1; // didn't add to header data since would shift all indices
        }
    }
}
