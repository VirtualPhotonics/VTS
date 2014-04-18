using System;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Point Sources
    /// </summary>
    [TestFixture]
    public class PointSourceTests
    {
        private const double ACCEPTABLE_PRECISION = 0.00000001;
        Position _position;
        Direction _direction;
        Position _translation;
        PolarAzimuthalAngles _angPair;
        DoubleRange _polRange;
        DoubleRange _aziRange;
        double _polarAngle;
        double[] _tp = new double[] {0.0000000000e+000,	0.0000000000e+000,	0.0000000000e+000,	7.0710678119e-001,	7.0710678119e-001,
                                     0.0000000000e+000,	1.0000000000e+000,	-2.5000000000e+000,	1.2000000000e+000,	7.8539816340e-001,
                                     7.8539816340e-001,	0.0000000000e+000,	1.5707963268e+000,	0.0000000000e+000,	3.1415926536e+000,
                                     7.8539816340e-001,	-1.7806560289e-001,	9.5420510124e-001,	2.4038566061e-001,	1.0000000000e+000,
                                    -2.5000000000e+000,	1.2000000000e+000,	7.0710678119e-001,	7.0710678119e-001,	6.1232339957e-017,
                                     1.0000000000e+000,	-2.5000000000e+000,	1.2000000000e+000,	-8.3062970822e-001,	-5.4820001436e-001,
                                     9.7627004864e-002,	1.0000000000e+000,	-2.5000000000e+000,	1.2000000000e+000};	

        /// <summary>
        /// Read text data file that has input and output data
        /// </summary>
        public void read_data()
        {
            string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_PointSources.txt";

            if (File.Exists(testpara))
            {
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    for (int i = 0; i < 34; i++)
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
            _polarAngle = _tp[15];
        }

        /// <summary>
        /// Validate General Constructor of Custom Point Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_point_source_test()
        {
            read_data();
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove
                       
            var ps = new CustomPointSource(_polRange, _aziRange, _direction, _translation)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[16]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[17]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[18]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[19]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[20]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[21]), ACCEPTABLE_PRECISION);            
        }

        /// <summary>
        /// Validate General Constructor of Directional Point Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_point_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove           

            var ps = new DirectionalPointSource(_direction, _translation)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[22]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[23]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[24]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[25]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[26]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[27]), ACCEPTABLE_PRECISION);    
        }

        /// <summary>
        /// Validate General Constructor of Isotropic Point Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_isotropic_point_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove           

            var ps = new IsotropicPointSource(_translation)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[28]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[29]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[30]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[31]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[32]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[33]), ACCEPTABLE_PRECISION);
        }
    }
}
