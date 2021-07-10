using System.IO;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Validation Data
    /// </summary>
    public class SourceValidationData
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
        /// rectangular source parameter width along y-axis
        /// </summary>
        public double WidthY { get; private set; }
        /// <summary>
        /// source parameter height along z-axis
        /// </summary>
        public double HeightZ { get; private set; }
        /// <summary>
        /// elliptical source parameter A
        /// </summary>
        public double AParameter { get; private set; }
        /// <summary>
        /// elliptical source parameter B
        /// </summary>
        public double BParameter { get; private set; }
        /// <summary>
        /// elliptical source parameter C
        /// </summary>
        public double CParameter { get; private set; }
        /// <summary>
        /// outer radius of circular source
        /// </summary>
        public double OutRad { get; private set; }
        /// <summary>
        ///  inner radius of circular source
        /// </summary>
        public double InRad { get; private set; }
        /// <summary>
        /// source beam full width half max
        /// </summary>
        public double BdFWHM { get; private set; }
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
            Tp = new double[] { 0.0000000000e+000, 0.0000000000e+000, 0.0000000000e+000, 7.0710678119e-001, 7.0710678119e-001, 0.0000000000e+000,
                                      1.0000000000e+000, -2.5000000000e+000, 1.2000000000e+000, 7.8539816340e-001, 7.8539816340e-001, 0.0000000000e+000,
                                      1.5707963268e+000, 0.0000000000e+000, 3.1415926536e+000, 1.0000000000e+000, 2.0000000000e+000, 3.0000000000e+000,
                                      8.0000000000e-001, 1.2000000000e+000, 1.5000000000e+000, 1.0000000000e+000, 2.0000000000e+000, 8.0000000000e-001,
                                      7.8539816340e-001, 4.6708668105e-001, 8.6503758455e-001, 1.8313931776e-001, 1.3558626222e+000, -2.8558626222e+000,
                                      2.7891058611e+000, 4.6708668105e-001, 8.6503758455e-001, 1.8313931776e-001, 1.4809917432e+000, -2.9809917432e+000,
                                      2.2306733258e+000, 6.0275777831e-001, 7.3715866524e-001, -3.0541801347e-001, 1.3558626222e+000, -2.8558626222e+000,
                                      2.7891058611e+000, 5.5744328042e-001, 7.0445430168e-001, -4.3931893421e-001, 1.4809917432e+000, -2.9809917432e+000,
                                      2.2306733258e+000, 4.6708668105e-001, 8.6503758455e-001, 1.8313931776e-001, 8.4243746094e-001, -2.3424374609e+000,
                                      1.1218983961e+000, 5.3044120905e-001, 7.9209406120e-001, 3.0202503529e-001, 8.0852649793e-001, -2.3085264979e+000,
                                      1.6055029281e+000, -7.5207139095e-002, 9.3896259407e-001, -3.3569797908e-001, 8.4243746094e-001, -2.3424374609e+000,
                                      1.1218983961e+000, 3.5567783444e-001, 9.3452389586e-001, 1.2584361593e-002, 8.0852649793e-001, -2.3085264979e+000,
                                      1.6055029281e+000, 4.6708668105e-001, 8.6503758455e-001, 1.8313931776e-001, 8.6869788411e-001, -2.3686978841e+000,
                                      1.1511864976e+000, 5.3044120905e-001, 7.9209406120e-001, 3.0202503529e-001, 8.2670323483e-001, -2.3267032348e+000,
                                      1.4698201590e+000, 1.1722944862e-002, 9.2240192747e-001, -3.8605343771e-001, 8.6869788411e-001, -2.3686978841e+000,
                                      1.1511864976e+000, 2.3884092479e-001, 9.5419317945e-001, -1.8019541877e-001, 8.2670323483e-001, -2.3267032348e+000,
                                      1.4698201590e+000};

            string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_SurfaceEmitting2DSources.txt";

            if (File.Exists(testpara))
            {
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    for (int i = 0; i < 97; i++)
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
            LengthX = Tp[15];
            WidthY = Tp[16];
            HeightZ = Tp[17];
            AParameter = Tp[18];
            BParameter = Tp[19];
            CParameter = Tp[20];
            InRad = Tp[21];
            OutRad = Tp[22]; 
            BdFWHM = Tp[23];
            PolarAngle = Tp[24];
        }
    }
}
