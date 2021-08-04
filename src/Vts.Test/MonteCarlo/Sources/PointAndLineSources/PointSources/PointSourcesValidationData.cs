using System;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Point sources validation data
    /// </summary>
    [TestFixture]
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

            Tp = new double[] {0.0000000000e+000,    0.0000000000e+000,  0.0000000000e+000,  7.0710678119e-001,  7.0710678119e-001,
                0.0000000000e+000, 1.0000000000e+000,  -2.5000000000e+000, 1.2000000000e+000,  7.8539816340e-001,
                7.8539816340e-001, 0.0000000000e+000,  1.5707963268e+000,  0.0000000000e+000,  3.1415926536e+000,
                7.8539816340e-001, -1.7806560289e-001, 9.5420510124e-001,  2.4038566061e-001,  1.0000000000e+000,
                -2.5000000000e+000, 1.2000000000e+000,  7.0710678119e-001,  7.0710678119e-001,  6.1232339957e-017,
                1.0000000000e+000, -2.5000000000e+000, 1.2000000000e+000,  -8.3062970822e-001, -5.4820001436e-001,
                9.7627004864e-002, 1.0000000000e+000,  -2.5000000000e+000, 1.2000000000e+000};

            // if need to regenerate _tp, run matlab/test/ code 
            if (Tp.Length == 0)
            {
                string testpara = "../../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_PointSources.txt";
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    for (int i = 0; i < 34; i++)
                        Tp[i] = double.Parse(bits[i]);
                    reader.Close();
                }
            }
            Position = new Position(Tp[0], Tp[1], Tp[2]);
            Direction = new Direction(Tp[3], Tp[4], Tp[5]);
            Translation = new Position(Tp[6], Tp[7], Tp[8]);
            AngPair = new PolarAzimuthalAngles(Tp[9], Tp[10]);
            PolRange = new DoubleRange(Tp[11], Tp[12]);
            AziRange = new DoubleRange(Tp[13], Tp[14]);
            PolarAngle = Tp[15];
        }
    }
}
