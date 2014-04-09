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
        private const double ACCEPTABLE_PRECISION = 0.00000001; // 1*10^-8
        Position _position;
        Direction _direction;
        Position _translation;
        PolarAzimuthalAngles _angPair;
        DoubleRange _polRange;
        DoubleRange _aziRange;       
        double _lengthX;
        double _bdFWHM;
        double _polarAngle;
        double[] _tp = new double[] { 0.0000000000e+000, 0.0000000000e+000, 0.0000000000e+000, 7.0710678119e-001, 7.0710678119e-001, 0.0000000000e+000,
                                      1.0000000000e+000, -2.5000000000e+000, 1.2000000000e+000, 7.8539816340e-001, 7.8539816340e-001, 0.0000000000e+000, 
                                      1.5707963268e+000, 0.0000000000e+000, 3.1415926536e+000, 1.0000000000e+000, 8.0000000000e-001, 7.8539816340e-001, 
                                      2.0282197879e-001, 8.9391490348e-001, 3.9972414270e-001, 1.0000000000e+000, -2.5000000000e+000, 1.1511864976e+000, 
                                      4.6708668105e-001, 8.6503758455e-001, 1.8313931776e-001, 1.0000000000e+000, -2.5000000000e+000, 1.4698201590e+000, 
                                      6.2618596595e-002, 8.3523227915e-001, -5.4632037417e-001, 1.0000000000e+000, -2.5000000000e+000, 1.1511864976e+000, 
                                      -3.0278343642e-001, 6.6403969080e-001, -6.8364718947e-001, 1.0000000000e+000, -2.5000000000e+000, 1.4698201590e+000, 
                                      6.8877745951e-001, -2.8987993707e-001, -6.6449622524e-001, 1.0000000000e+000, -2.5000000000e+000, 1.1511864976e+000, 
                                      7.3578282320e-003, -8.0893135119e-002, -9.9669562207e-001, 1.0000000000e+000, -2.5000000000e+000, 1.4698201590e+000};

        /// <summary>
        /// Read text data file that has input and output data
        /// </summary>
        public void read_data()
        {
            string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_LineSources.txt";

            if (File.Exists(testpara))
            {
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[18]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[19]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[20]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[21]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[22]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[23]), ACCEPTABLE_PRECISION);
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[24]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[25]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[26]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[27]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[28]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[29]), ACCEPTABLE_PRECISION);
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[30]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[31]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[32]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[33]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[34]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[35]), ACCEPTABLE_PRECISION);
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[36]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[37]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[38]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[39]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[40]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[41]), ACCEPTABLE_PRECISION);
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[42]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[43]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[44]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[45]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[46]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[47]), ACCEPTABLE_PRECISION);
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[48]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[49]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[50]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[51]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[52]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[53]), ACCEPTABLE_PRECISION);
        }

    }
}
