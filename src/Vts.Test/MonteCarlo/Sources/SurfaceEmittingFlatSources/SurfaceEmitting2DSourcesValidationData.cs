using System.IO;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Validation Data
    /// </summary>
    public class SurfaceEmitting2DSourcesValidationData
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
        /// Lambert order of angular distribution
        /// </summary>
        public int LambertOrder { get; private set; }
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
                1.0000000000e+00, 2.0000000000e+00, 3.0000000000e+00, 8.0000000000e-01, 1.2000000000e+00,
                1.5000000000e+00, 1.0000000000e+00, 2.0000000000e+00, 8.0000000000e-01, 7.8539816340e-01,
                4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01, 2.0461858039e+00, -3.5461858039e+00,
                1.9678049083e+00, 4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01, 1.8554491863e+00,
                -3.3554491863e+00, 1.4478043547e+00, 6.0275777831e-01, 7.3715866524e-01, -3.0541801347e-01,
                2.0461858039e+00, -3.5461858039e+00, 1.9678049083e+00, 5.5744328042e-01, 7.0445430168e-01,
                -4.3931893421e-01, 1.8554491863e+00, -3.3554491863e+00, 1.4478043547e+00, 4.6708668105e-01,
                8.6503758455e-01, 1.8313931776e-01, 8.4953565822e-01, -2.3495356582e+00, 1.3023363653e+00,
                5.3044120905e-01, 7.9209406120e-01, 3.0202503529e-01, 1.0673592523e+00, -2.5673592523e+00,
                1.6782073724e+00, -7.5207139095e-02, 9.3896259407e-01, -3.3569797908e-01, 8.4953565822e-01,
                -2.3495356582e+00, 1.3023363653e+00, 3.5567783444e-01, 9.3452389586e-01, 1.2584361593e-02,
                1.0673592523e+00, -2.5673592523e+00, 1.6782073724e+00, 4.6708668105e-01, 8.6503758455e-01,
                1.8313931776e-01, 8.8274863226e-01, -2.3827486323e+00, 1.2967857573e+00, 5.3044120905e-01,
                7.9209406120e-01, 3.0202503529e-01, 1.0123707617e+00, -2.5123707617e+00, 1.5640884293e+00,
                1.1722944862e-02, 9.2240192747e-01, -3.8605343771e-01, 8.8274863226e-01, -2.3827486323e+00,
                1.2967857573e+00, 2.3884092479e-01, 9.5419317945e-01, -1.8019541877e-01, 1.0123707617e+00,
                -2.5123707617e+00, 1.5640884293e+00, 4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01,
                2.0461858039e+00, -3.5461858039e+00, 1.9678049083e+00, 4.6708668105e-01, 8.6503758455e-01,
                1.8313931776e-01, 1.8554491863e+00, -3.3554491863e+00, 1.4478043547e+00, 4.6708668105e-01,
                8.6503758455e-01, 1.8313931776e-01, 8.4953565822e-01, -2.3495356582e+00, 1.3023363653e+00,
                4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01, 1.0673592523e+00, -2.5673592523e+00,
                1.6782073724e+00, 4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01, 8.8274863226e-01,
                -2.3827486323e+00, 1.2967857573e+00, 4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01,
                1.0123707617e+00, -2.5123707617e+00, 1.5640884293e+00,
            ];

            // if you need to regenerate Tp, run matlab/test/ code Unit_Tests_MonteCarlo_Sources_SurfaceEmitting2DSources.m
            if (Tp.Length == 0)
            {
                var testpara = "../../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_SurfaceEmitting2DSources.txt";
                using TextReader reader = File.OpenText(testpara);
                var text = reader.ReadToEnd();
                var bits = text.Split(',');
                for (var i = 0; i < Tp.Length; i++)
                    Tp[i] = double.Parse(bits[i]);
                reader.Close();
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
            LambertOrder = 1; // did not add as parameter to Tp because all indices would shift
        }
    }
}
