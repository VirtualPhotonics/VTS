using System;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Line Sources
    /// </summary>
    [TestFixture]
    public class LineSourcesTests
    {
        private const double AcceptablePrecision = 0.00000001; // 1*10^-8
        private Position _position;
        private Direction _direction;
        private Position _translation;
        private PolarAzimuthalAngles _angPair;
        private DoubleRange _polRange;
        private DoubleRange _aziRange;       
        private double _lengthX;
        private double _bdFWHM;
        private double _polarAngle;
        // these validation values get generated using MATLAB scripts in matlab/test/monte_carlo/...
        private readonly double[] _tp =  {
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
        /// <summary>
        /// Read text data file that has input and output data
        /// </summary>
        public void read_data()
        {
            // if need to regenerate Tp, run matlab/test/ code 
            if (_tp.Length == 0)
            {
                string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_LineSources.txt";
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split(',');
                    for (int i = 0; i < 54; i++)
                        _tp[i] = double.Parse(bits[i]);
                    reader.Close();
                }
            }     
            _position = new Position(_tp[0], _tp[1], _tp[2]);
            _direction = new Direction(_tp[3], _tp[4], _tp[5]);
            _translation = new Position(_tp[6], _tp[7], _tp[8]);
            _angPair = new PolarAzimuthalAngles(_tp[9], _tp[10]);
            _polRange = new DoubleRange(_tp[11], _tp[12]);
            _aziRange = new DoubleRange(_tp[13], _tp[14]);
            _lengthX = _tp[15];
            _bdFWHM = _tp[16];
            _polarAngle = _tp[17];
        }

        /// <summary>
        /// Validate General Constructor of Custom Flat Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_line_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();             
            var profile = new FlatSourceProfile();


            var ps = new CustomLineSource(_lengthX, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[18]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[19]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[20]), AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[21]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[22]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[23]), AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of Custom Gaussian Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_line_source_test()
        {         

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);

            var ps = new CustomLineSource(_lengthX, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[24]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[25]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[26]), AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[27]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[28]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[29]), AcceptablePrecision);
        }

        /// <summary>
        /// Validate General Constructor of Directional Flat Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_line_source_test()
        {            

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new DirectionalLineSource(_polarAngle, _lengthX, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[30]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[31]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[32]), AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[33]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[34]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[35]), AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of Directional Gaussian Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_directional_line_source_test()
        {            

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new DirectionalLineSource(_polarAngle, _lengthX, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[36]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[37]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[38]), AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[39]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[40]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[41]), AcceptablePrecision);
        }

        /// <summary>
        /// Validate General Constructor of Isotropic Flat Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_isotropic_line_source_test()
        {            

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new IsotropicLineSource(_lengthX, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[42]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[43]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[44]), AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[45]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[46]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[47]), AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of Isotropic Gaussian Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_isotropic_line_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new IsotropicLineSource(_lengthX, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[48]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[49]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[50]), AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[51]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[52]), AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[53]), AcceptablePrecision);
        }
        /// <summary>
        /// Validate general contructor and implicitly validate GetFinalPosition
        /// and GetFinalDirection
        /// </summary>
        [Test]
        public void validate_LineAngledFromLineSource_general_constructor()
        {
            var tissue = new MultiLayerTissue();
            var source = new LineAngledFromLineSource(
                10.0, // tissue line length
                new FlatSourceProfile(),
                1.0, // line in air length
                new Position(0, 0, -10), // center of line in air
                0);
            var photon = source.GetNextPhoton(tissue);
            // Position.X will be random between [-5 5] and Y and Z should be 0
            Assert.IsTrue(photon.DP.Position.X < 5);
            Assert.IsTrue(photon.DP.Position.X > -5);
            Assert.AreEqual(photon.DP.Position.Y, 0.0);
            Assert.AreEqual(photon.DP.Position.Z, 0.0);
            // Direction.Ux,Uz will be random but Uy should be 0
            Assert.AreEqual(photon.DP.Direction.Uy, 0.0);
        }

    }
}
