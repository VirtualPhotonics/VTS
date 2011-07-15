using System;
using NUnit.Framework;
using Vts.Common;
using System.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Point Sources
    /// </summary>
    [TestFixture]
    public class PointSourceTests
    {
        Position _position;
        Direction _direction;
        Position _translation;
        PolarAzimuthalAngles _angPair;
        DoubleRange _polRange;
        DoubleRange _aziRange;
        double _lengthX;
        double _bdFWHM;
        double _polarAngle;
        double[] _tp = new double[] {0.0000000000e+000,	0.0000000000e+000,	0.0000000000e+000,	7.0710678119e-001,	7.0710678119e-001,
                                     0.0000000000e+000,	1.0000000000e+000,	-2.5000000000e+000,	1.2000000000e+000,	7.8539816340e-001,
                                     7.8539816340e-001,	0.0000000000e+000,	1.5707963268e+000,	0.0000000000e+000,	3.1415926536e+000,
                                     1.0000000000e+000,	8.0000000000e-001,  7.8539816340e-001,	2.0282197879e-001,	8.9391490348e-001,	
                                     3.9972414270e-001,	1.0000000000e+000, -2.5000000000e+000,	1.1511864976e+000,	4.6708668105e-001,	
                                     8.6503758455e-001,	1.8313931776e-001,  1.0000000000e+000,	-2.5000000000e+000,	1.6247716491e+000,	
                                     6.2618596595e-002,	8.3523227915e-001, -5.4632037417e-001,	1.0000000000e+000,	-2.5000000000e+000,	
                                     1.1511864976e+000,	-5.1261571269e-001, 4.8722307784e-001,	-7.0699278888e-001,	1.0000000000e+000,	
                                     -2.5000000000e+000, 1.6247716491e+000,  6.8877745951e-001,	-2.8987993707e-001,	-6.6449622524e-001,	
                                     1.0000000000e+000,	-2.5000000000e+000, 1.1511864976e+000,	7.3578282320e-003,	-8.0893135119e-002,	
                                     -9.9669562207e-001, 1.0000000000e+000, -2.5000000000e+000,	1.6247716491e+000};

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

        [Test]
        public void validate_general_constructor_with_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);
            var direction = new Direction(1.0, 0.0, 0.0);
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);

            var ps = new CustomPointSource(polRange, aziRange, direction, position)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.54881350243203653), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.800636293032631), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.240385660610712), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 3.0), 0.0000000001);            
        }
        
        [Test]
        public void validate_general_constructor_with_no_optionalparameters_and_getnextphoton_assigns_correct_values1()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove
            
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);

            var ps = new CustomPointSource(polRange, aziRange)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux + 0.240385660610712), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.800636293032631), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.54881350243203653), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 0.0), 0.0000000001);
        }

        [Test]
        public void validate_general_constructor_with_no_optionalparameters_and_getnextphoton_assigns_correct_values2()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var polRange = new DoubleRange(0.0, 0.0);
            var aziRange = new DoubleRange(0.0, 0.0);

            var ps = new CustomPointSource(polRange, aziRange)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 1.0), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 0.0), 0.0000000001);
        }

        //[Test]
        //public void validate_general_constructor_with_getnextphoton_assigns_correct_values()
        //{
        //    Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
        //    ITissue tissue = new MultiLayerTissue(); // todo: remove

        //    var position = new Position(1.0, 2.0, 3.0);
        //    var direction = new Direction(1.0, 0.0, 0.0);

        //    var ps = new DirectionalPointSource(direction, position)
        //    {
        //        Rng = rng
        //    };

        //    var photon = ps.GetNextPhoton(tissue);

        //    Assert.Less(Math.Abs(photon.DP.Direction.Ux - 1.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.0), 0.0000000001);

        //    Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Position.Z - 3.0), 0.0000000001);
        //}

        //[Test]
        //public void validate_general_constructor_with_no_optionalparameters_and_getnextphoton_assigns_correct_values()
        //{
        //    Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
        //    ITissue tissue = new MultiLayerTissue(); // todo: remove

        //    var direction = new Direction(0.0, 1.0, 0.0);

        //    var ps = new DirectionalPointSource(direction)
        //    {
        //        Rng = rng
        //    };

        //    var photon = ps.GetNextPhoton(tissue);

        //    Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Direction.Uy - 1.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.0), 0.0000000001);

        //    Assert.Less(Math.Abs(photon.DP.Position.X - 0.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Position.Y - 0.0), 0.0000000001);
        //    Assert.Less(Math.Abs(photon.DP.Position.Z - 0.0), 0.0000000001);
        //}

        //[Test]
        //public void validate_general_constructor_with_getnextphoton_assigns_correct_values()
        //{
        //    Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
        //    ITissue tissue = new MultiLayerTissue(); // todo: remove

        //    var position = new Position(1.0, 2.0, 3.0);

        //    var ps = new IsotropicPointSource(position)
        //    {
        //        Rng = rng
        //    };

        //    var photon = ps.GetNextPhoton(tissue);

        //    Assert.IsTrue(
        //        photon.DP.Position.X == 1.0 &&
        //        photon.DP.Position.Y == 2.0 &&
        //        photon.DP.Position.Z == 3.0
        //   );
        //}

        //[Test]
        //public void validate_null_constructor_with_getnextphoton_assigns_correct_values()
        //{
        //    Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
        //    ITissue tissue = new MultiLayerTissue(); // todo: remove            

        //    var ps = new IsotropicPointSource()
        //    {
        //        Rng = rng
        //    };

        //    var photon = ps.GetNextPhoton(tissue);

        //    Assert.IsTrue(
        //        photon.DP.Position.X == 0.0 &&
        //        photon.DP.Position.Y == 0.0 &&
        //        photon.DP.Position.Z == 0.0
        //   );
        //}

        //[Test]
        //public void validate_GetNextPhoton_returns_nonuniform_directions()
        //{
        //    Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
        //    ITissue tissue = new MultiLayerTissue(); // todo: remove

        //    var position = new Position(1.0, 2.0, 3.0);

        //    var ps = new IsotropicPointSource(position)
        //    {
        //        Rng = rng
        //    };

        //    var photon1 = ps.GetNextPhoton(tissue);
        //    var photon2 = ps.GetNextPhoton(tissue);
        //    var photon3 = ps.GetNextPhoton(tissue);

        //    Assert.IsFalse(photon1.DP.Direction == photon2.DP.Direction);
        //    Assert.IsFalse(photon2.DP.Direction == photon3.DP.Direction);
        //}

    }
}
