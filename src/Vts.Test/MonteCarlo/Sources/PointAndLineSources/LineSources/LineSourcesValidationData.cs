using System.IO;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Line Sources Validation Data
    /// </summary>
    public class LineSourcesValidationData
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
        /// source beam full width half max
        /// </summary>
        public double BdFWHM { get; private set; }
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

            // these validation values get generated using MATLAB scripts in matlab/test/monte_carlo/...
            Tp =  new double[] {
                0.0000000000e+00, 0.0000000000e+00, 0.0000000000e+00, 7.0710678119e-01, 7.0710678119e-01, 0.0000000000e+00,
                1.0000000000e+00, -2.5000000000e+00, 1.2000000000e+00, 7.8539816340e-01, 7.8539816340e-01, 0.0000000000e+00,
                1.5707963268e+00, 0.0000000000e+00, 3.1415926536e+00, 1.0000000000e+00, 8.0000000000e-01, 7.8539816340e-01,
                2.0282197879e-01, 8.9391490348e-01, 3.9972414270e-01, 9.7559324878e-01, -2.4755932488e+00, 1.1654836414e+00,
                4.6708668105e-01, 8.6503758455e-01, 1.8313931776e-01, 1.1349100795e+00, -2.6349100795e+00, 1.3907916641e+00,
                6.2618596595e-02, 8.3523227915e-01, -5.4632037417e-01, 9.7559324878e-01, -2.4755932488e+00, 1.1654836414e+00,
                -3.0278343642e-01, 6.6403969080e-01, -6.8364718947e-01, 1.1349100795e+00, -2.6349100795e+00, 1.3907916641e+00,
                6.8877745951e-01, -2.8987993707e-01, -6.6449622524e-01, 9.7559324878e-01, -2.4755932488e+00, 1.1654836414e+00,
                7.3578282320e-03, -8.0893135119e-02, -9.9669562207e-01, 1.1349100795e+00, -2.6349100795e+00, 1.3907916641e+00
            };
            // if need to regenerate Tp, run matlab/test/ code 
            if (Tp.Length == 0)
            {
                const string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_LineSources.txt";
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split(',');
                    for (int i = 0; i < 54; i++)
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
            BdFWHM = Tp[16];
            PolarAngle = Tp[17];
        }
    }
}
