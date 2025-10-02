using NUnit.Framework;
using System;
using System.IO;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class SourceToolboxTests
    {
        private const double AcceptablePrecision = 0.00000001;
        private Position _position;
        private Direction _direction;
        private Position _translation;
        private PolarAzimuthalAngles _angPair;
        private DoubleRange _polRange;
        private DoubleRange _aziRange;
        private ThreeAxisRotation _angRot;
        private SourceFlags _flags;
        private double _aParameter;
        private double _bParameter;
        private double _cParameter;
        private double _lengthX;
        private double _widthY;
        private double _heightZ;
        private double _innerRadius;
        private double _outerRadius;
        private double _bdFwhm;
        private double _limitL;
        private double _limitU;
        private double _factor;
        private double _polarAngle;
        private double[] _tp;

        /// <summary>
        /// read validation data in
        /// </summary>
        [OneTimeSetUp]
        public void Setup_validation_data()
        {
            _tp =
            [
                1.0000000000e+00, 2.0000000000e+00, 3.0000000000e+00, 7.0710678119e-01, 7.0710678119e-01,
                0.0000000000e+00, 1.0000000000e+00, -2.5000000000e+00, 1.2000000000e+00, 7.8539816340e-01,
                7.8539816340e-01, 0.0000000000e+00, 1.5707963268e+00, 0.0000000000e+00, 3.1415926536e+00,
                1.5707963268e+00, 1.5707963268e+00, 7.8539816340e-01, 2.0000000000e+00, 2.5000000000e+00,
                3.0000000000e+00, 1.0000000000e+00, 2.0000000000e+00, 3.0000000000e+00, 1.0000000000e+00,
                2.0000000000e+00, 8.0000000000e-01, 2.5000000000e+00, 5.0000000000e-01, 5.0000000000e-01,
                7.8539816340e-01, 3.1622776602e-01, 6.3245553203e-01, 7.0710678119e-01, -2.4038566061e-01,
                8.0063629303e-01, 5.4881350243e-01, -8.3062970822e-01, -5.4820001436e-01, 9.7627004864e-02,
                -0.0000000000e+00, -0.0000000000e+00, 8.8249690258e-01, 9.8985210047e-01, 1.8624762920e+00,
                1.5707963268e+00, 7.8539816340e-01, -5.8910586113e-01, 1.4967342534e+00, 3.0000000000e+00,
                4.8466333708e-01, 1.6598874767e+00, 3.0000000000e+00, 1.0488135024e+00, 2.1856892331e+00,
                3.6455680954e+00, 7.3017984105e-01, 2.2450786356e+00, 3.4045250143e+00, 1.0488135024e+00,
                2.0000000000e+00, 3.0000000000e+00, 7.3017984105e-01, 2.0000000000e+00, 3.0000000000e+00,
                1.1952540097e+00, 2.4642230826e+00, 3.0000000000e+00, 6.9218831412e-01, 2.1551835684e+00,
                3.0000000000e+00, 1.1952540097e+00, 2.4642230826e+00, 4.2911361908e+00, 3.8437662825e-01,
                2.3103671369e+00, 3.4289404236e+00, 1.0488135024e+00, 2.1856892331e+00, 3.0000000000e+00,
                7.3017984105e-01, 2.2450786356e+00, 3.0000000000e+00, 4.8813502432e-02, -0.0000000000e+00,
                5.0000000000e-01, 5.0000000000e-01, 7.0710678119e-01, 7.0710678119e-01, 4.3297802812e-17,
                7.0710678119e-01, 4.3297802812e-17, 7.0710678119e-01, -7.0710678119e-01, 0.0000000000e+00,
                1.0000000000e+00, 0.0000000000e+00, -1.4644660941e-01, 8.5355339059e-01, -5.0000000000e-01,
                -1.4644660941e-01, 8.5355339059e-01, -5.0000000000e-01, 1.5857864376e+00, 9.1421356237e-01,
                2.6142135624e+00, -9.2677669530e-01, 2.8033008589e-01, -2.5000000000e-01, 6.4644660941e-01,
                1.4644660941e-01, 3.8213203436e+00, 7.0710678119e-01, 4.3297802812e-17, 7.0710678119e-01,
                1.0000000000e+00, -3.0000000000e+00, 2.0000000000e+00, 4.3297802812e-17, 7.0710678119e-01,
                -7.0710678119e-01, 3.0000000000e+00, 2.0000000000e+00, -1.0000000000e+00, 0.0000000000e+00,
                1.0000000000e+00, 0.0000000000e+00, -7.0710678119e-01, 2.1213203436e+00, 3.0000000000e+00,
                -1.4644660941e-01, 8.5355339059e-01, -5.0000000000e-01, 5.8578643763e-01, 3.4142135624e+00,
                1.4142135624e+00, 3.9269908170e-01, 2.0000000000e+00, -5.0000000000e-01, 4.2000000000e+00,
                -9.0666756704e-01, 1.3961632764e+00, 3.0000000000e+00
            ];

            // to regenerate _tp, run matlab/test/ code 
            if (_tp.Length == 0)
            {
                const string testpara = "../../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_SourceToolbox.txt";

                using var reader = File.OpenText(testpara);
                var text = reader.ReadToEnd();
                var bits = text.Split('\t');
                for (var i = 0; i < 140; i++)
                    _tp[i] = double.Parse(bits[i]);
                reader.Close();
            }

            _position = new Position(_tp[0], _tp[1], _tp[2]);
            _direction = new Direction(_tp[3], _tp[4], _tp[5]);
            _translation = new Position(_tp[6], _tp[7], _tp[8]);
            _angPair = new PolarAzimuthalAngles(_tp[9], _tp[10]);
            _polRange = new DoubleRange(_tp[11], _tp[12]);
            _aziRange = new DoubleRange(_tp[13], _tp[14]);
            _angRot = new ThreeAxisRotation(_tp[15], _tp[16], _tp[17]);
            _flags = new SourceFlags(true, true, true);
            _aParameter = _tp[18];
            _bParameter = _tp[19];
            _cParameter = _tp[20];
            _lengthX = _tp[21];
            _widthY = _tp[22];
            _heightZ = _tp[23];
            _innerRadius = _tp[24];
            _outerRadius = _tp[25];
            _bdFwhm = _tp[26];
            _limitL = _tp[27];
            _limitU = _tp[28];
            _factor = _tp[29];
            _polarAngle = _tp[30];
        }

        /// <summary>
        /// Validating "GetDirectionForGiven2DPositionAndGivenPolarAngle"     
        /// </summary>
        [Test]
        public void Validate_static_method_getdirectionforgiven2dpositionandgivenpolarangle()
        {
            var pos = _position.Clone();
            var dir = SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(_polarAngle, pos);

            Assert.That(Math.Abs(dir.Ux - _tp[31]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[32]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[33]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetDirectionForGivenPolarAzimuthalAngleRangeRandom"
        /// </summary>
        [Test]
        public void Validate_static_method_getdirectionforgivenpolarazimuthalanglerangerandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var dir = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(_polRange, _aziRange, rng);

            Assert.That(Math.Abs(dir.Ux - _tp[34]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[35]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[36]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetDirectionForIsotropicDistributionRandom"
        /// </summary>
        [Test]
        public void Validate_static_method_getdirectionforisotropicdistributionrandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var dir = SourceToolbox.GetDirectionForIsotropicDistributionRandom(rng);

            Assert.That(Math.Abs(dir.Ux - _tp[37]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[38]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[39]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetDoubleNormallyDistributedRandomNumbers"
        /// </summary>
        [Test]
        public void Validate_static_method_getdoublenormallydistributedrandomnumbers()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var nrng1 = 0.0;
            var nrng2 = 0.0;

           SourceToolbox.GetDoubleNormallyDistributedRandomNumbers(ref nrng1, ref nrng2, _limitL, _limitU, rng);

           Assert.That(Math.Abs(nrng1 - _tp[40]), Is.LessThan(AcceptablePrecision));
           Assert.That(Math.Abs(nrng2 - _tp[41]), Is.LessThan(AcceptablePrecision));            
        }
        
        /// <summary>
        /// Validating "GetLowerLimit"
        /// </summary>
        [Test]
        public void Validate_static_method_getlowerlimit()
        {
            var limit = SourceToolbox.GetLimit(_factor);

            Assert.That(Math.Abs(limit - _tp[42]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPolarAzimuthalPairForGivenAngleRangeRandom"
        /// </summary>        
        [Test]
        public void Validate_static_method_getpolarazimuthalpairforgivenanglerangerandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var angPair = SourceToolbox.GetPolarAzimuthalPairForGivenAngleRangeRandom(_polRange, _aziRange,rng);

            Assert.That(Math.Abs(angPair.Theta - _tp[43]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(angPair.Phi - _tp[44]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPolarAzimuthalPairFromDirection"
        /// </summary>
        [Test]
        public void validate_static_method_getpolarazimuthalpairfromdirection()
        {
            var dir = _direction;
            var angPair = SourceToolbox.GetPolarAzimuthalPairFromDirection(dir);

            Assert.That(Math.Abs(angPair.Theta - _tp[45]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(angPair.Phi - _tp[46]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInACircleRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninacirclerandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACircleRandomFlat(_position, _innerRadius, _outerRadius, rng);
            Assert.That(Math.Abs(pos.X - _tp[47]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[48]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[49]), Is.LessThan(AcceptablePrecision));
            // test for radius = 0
            pos = SourceToolbox.GetPositionInACircleRandomFlat(_position, 0.0, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInACircleRandomGaussian"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninacirclerandomgaussian()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACircleRandomGaussian(_position, _outerRadius, _innerRadius, _bdFwhm, rng);

            Assert.That(Math.Abs(pos.X - _tp[50]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[51]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[52]), Is.LessThan(AcceptablePrecision));
            // test for radius = 0
            pos = SourceToolbox.GetPositionInACircleRandomGaussian(_position, 0.0, 0.0, _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
            // test for FWHM = 0 but with finite radius
            pos = SourceToolbox.GetPositionInACircleRandomGaussian(_position, _outerRadius, _innerRadius, 0.0, rng);
            Assert.That(Math.Abs(pos.X - double.PositiveInfinity), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - double.NegativeInfinity), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInACuboidRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninacuboidrandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACuboidRandomFlat(_position, _lengthX, _widthY, _heightZ, rng);

            Assert.That(Math.Abs(pos.X - _tp[53]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[54]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[55]), Is.LessThan(AcceptablePrecision));
            // test for length = 0
            pos = SourceToolbox.GetPositionInACuboidRandomGaussian(_position, 0.0, 0.0, 0.0,  _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInACuboidRandomGaussian"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninacuboidrandomgaussian()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACuboidRandomGaussian(_position, 0.5*_lengthX, 0.5*_widthY, 0.5*_heightZ, _bdFwhm, rng);

            Assert.That(Math.Abs(pos.X - _tp[56]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[57]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[58]), Is.LessThan(AcceptablePrecision));
            // test for length = 0
            pos = SourceToolbox.GetPositionInACuboidRandomGaussian(_position, 0.0,  0.0, 0.0, _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
            // test for FWHM = 0 but with finite length
            pos = SourceToolbox.GetPositionInACuboidRandomGaussian(_position, _lengthX, _widthY, _heightZ, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInALineRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninalinerandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInALineRandomFlat(_position, _lengthX, rng);

            Assert.That(Math.Abs(pos.X - _tp[59]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[60]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[61]), Is.LessThan(AcceptablePrecision));
            // test for length = 0
            pos = SourceToolbox.GetPositionInALineRandomFlat(_position, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInALineRandomGaussian"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninalinerandomgaussian()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInALineRandomGaussian(_position, 0.5*_lengthX, _bdFwhm, rng);

            Assert.That(Math.Abs(pos.X - _tp[62]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[63]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[64]), Is.LessThan(AcceptablePrecision));
            // test for length = 0
            pos = SourceToolbox.GetPositionInALineRandomGaussian(_position, 0.0, _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
            // test for FWHM = 0 but with finite length
            pos = SourceToolbox.GetPositionInALineRandomGaussian(_position, _lengthX,  0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInAnEllipseRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninanellipserandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipseRandomFlat(_position, _aParameter, _bParameter, rng);

            Assert.That(Math.Abs(pos.X - _tp[65]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[66]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[67]), Is.LessThan(AcceptablePrecision));
            // test for a,b = 0
            pos = SourceToolbox.GetPositionInAnEllipseRandomFlat(_position, 0.0, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }
        
        /// <summary>
        /// Validating "GetPositionInAnEllipseRandomGaussian"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninanellipserandomgaussian()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipseRandomGaussian(_position, _aParameter, _bParameter, _bdFwhm, rng);

            Assert.That(Math.Abs(pos.X - _tp[68]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[69]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[70]), Is.LessThan(AcceptablePrecision));
            // test for a,b = 0
            pos = SourceToolbox.GetPositionInAnEllipseRandomGaussian(_position, 0.0, 0.0, _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
            // test for FWHM = 0 but with finite a,b
            pos = SourceToolbox.GetPositionInAnEllipseRandomGaussian(_position, _aParameter, _bParameter, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInAnEllipsoidRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninanellipsoidrandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipsoidRandomFlat(_position, _aParameter, _bParameter, _cParameter, rng);

            Assert.That(Math.Abs(pos.X - _tp[71]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[72]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[73]), Is.LessThan(AcceptablePrecision));
            // test for a,b,c = 0
            pos = SourceToolbox.GetPositionInAnEllipsoidRandomFlat(_position, 0.0, 0.0, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }


        /// <summary>
        /// Validating "GetPositionInAnEllipsoidRandomGaussian"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninanellipsoidrandomgaussian()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(_position, _aParameter, _bParameter, _cParameter, _bdFwhm, rng);

            Assert.That(Math.Abs(pos.X - _tp[74]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[75]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[76]), Is.LessThan(AcceptablePrecision));
            // test for a,b = 0
            pos = SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(_position, 0.0, 0.0, 0.0, _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
            // test for FWHM = 0 but with finite a,b
            pos = SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(_position, _aParameter, _bParameter, _cParameter, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInARectangleRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositioninarectanglerandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInARectangleRandomFlat(_position, _lengthX, _widthY, rng);

            Assert.That(Math.Abs(pos.X - _tp[77]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[78]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[79]), Is.LessThan(AcceptablePrecision));
            // test for length, width = 0
            pos = SourceToolbox.GetPositionInARectangleRandomFlat(_position, 0.0, 0.0, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInARectangleRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninarectanglerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInARectangleRandomGaussian(_position, 0.5*_lengthX, 0.5*_widthY, _bdFwhm, rng);

            Assert.That(Math.Abs(pos.X - _tp[80]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[81]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[82]), Is.LessThan(AcceptablePrecision));
            // test for length, width = 0
            pos = SourceToolbox.GetPositionInARectangleRandomGaussian(_position, 0.0, 0.0, _bdFwhm, rng);
            Assert.That(Math.Abs(pos.X - _position.X), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _position.Y), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
            // test for FWHM = 0 but with finite length, width
            pos = SourceToolbox.GetPositionInARectangleRandomGaussian(_position, _lengthX, _widthY, 0.0, rng);
            Assert.That(Math.Abs(pos.X - 1.00000536), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - 2.00000536), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _position.Z), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionOfASymmetricalLineRandomFlat"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositionofasymmetricallinerandomflat()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var loc = SourceToolbox.GetPositionOfASymmetricalLineRandomFlat(_lengthX, rng);

            Assert.That(Math.Abs(loc - _tp[83]), Is.LessThan(AcceptablePrecision));            
        }

        /// <summary>
        /// Validating "GetSingleNormallyDistributedRandomNumber"
        /// </summary>
        [Test]
        public void Validate_static_method_getsinglenormallydistributedrandomnumber()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var nrng = SourceToolbox.GetSingleNormallyDistributedRandomNumber(_limitL, rng);

            Assert.That(Math.Abs(nrng - _tp[84]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundThreeAxis"   
        /// </summary>
  
        [Test]
        public void Validate_static_method_updatedirectionafterrotatingaroundthreeaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundThreeAxis(_angRot, dir);

            Assert.That(Math.Abs(dir.Ux - _tp[85]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[86]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[87]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundXAxis"  
        /// </summary>
   
        [Test]
        public void Validate_static_method_updatedirectionafterrotatingaroundxaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(_angRot.XRotation, dir);

            Assert.That(Math.Abs(dir.Ux - _tp[88]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[89]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[90]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundYAxis"  
        /// </summary>
   
        [Test]
        public void Validate_static_method_updatedirectionafterrotatingaroundyaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundYAxis(_angRot.YRotation, dir);

            Assert.That(Math.Abs(dir.Ux - _tp[91]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[92]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[93]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundZAxis"     
        /// </summary>
        [Test]
        public void Validate_static_method_updatedirectionafterrotatingaroundzaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundZAxis(_angRot.ZRotation, dir);

            Assert.That(Math.Abs(dir.Ux - _tp[94]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[95]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[96]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingByGivenAnglePair"   
        /// </summary>
  
        [Test]
        public void Validate_static_method_updatedirectionafterrotatingbygivenanglepair()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingByGivenAnglePair(_angPair, dir);

            Assert.That(Math.Abs(dir.Ux - _tp[97]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[98]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[99]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// //Validating "UpdateDirectionPositionAfterGivenFlags"  
        /// </summary>   
        [Test]
        public void Validate_static_method_updatedirectionpositionaftergivenflags1()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(ref pos, ref dir, _angPair, _translation, _flags);

            Assert.That(Math.Abs(dir.Ux - _tp[100]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[101]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[102]), Is.LessThan(AcceptablePrecision));

            Assert.That(Math.Abs(pos.X - _tp[103]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[104]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[105]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterGivenFlags"   
        /// </summary>  
        [Test]
        public void Validate_static_method_updatedirectionpositionaftergivenflags2()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(ref pos, ref dir, _angPair, _translation, _angPair, _flags);

            Assert.That(Math.Abs(dir.Ux - _tp[106]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[107]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[108]), Is.LessThan(AcceptablePrecision));

            Assert.That(Math.Abs(pos.X - _tp[109]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[110]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[111]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterGivenFlags"   
        /// </summary>
        [Test]
        public void Validate_static_method_updatedirectionpositionafterrotatingaroundxaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundXAxis(_angRot.XRotation, ref dir, ref pos);

            Assert.That(Math.Abs(dir.Ux - _tp[112]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[113]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[114]), Is.LessThan(AcceptablePrecision));

            Assert.That(Math.Abs(pos.X - _tp[115]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[116]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[117]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterRotatingAroundYAxis"  
        /// </summary>   
        [Test]
        public void Validate_static_method_updatedirectionpositionafterrotatingaroundyaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundYAxis(_angRot.YRotation, ref dir, ref pos);

            Assert.That(Math.Abs(dir.Ux - _tp[118]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[119]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[120]), Is.LessThan(AcceptablePrecision));

            Assert.That(Math.Abs(pos.X - _tp[121]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[122]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[123]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterRotatingAroundZAxis"     
        /// </summary>
        [Test]
        public void Validate_static_method_updatedirectionpositionafterrotatingaroundzaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundZAxis(_angRot.ZRotation, ref dir, ref pos);

            Assert.That(Math.Abs(dir.Ux - _tp[124]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[125]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[126]), Is.LessThan(AcceptablePrecision));

            Assert.That(Math.Abs(pos.X - _tp[127]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[128]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[129]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// //Validating "UpdateDirectionPositionAfterRotatingByGivenAnglePair"
        /// </summary>     
        [Test]
        public void Validate_static_method_updatedirectionpositionafterrotatingbygivenanglepair()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingByGivenAnglePair(_angPair, ref dir, ref pos);

            Assert.That(Math.Abs(dir.Ux - _tp[130]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uy - _tp[131]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(dir.Uz - _tp[132]), Is.LessThan(AcceptablePrecision));

            Assert.That(Math.Abs(pos.X - _tp[133]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[134]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[135]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdatePolarAngleForDirectionalSources"     
        /// </summary>
        [Test]
        public void Validate_static_method_updatepolarangleFordirectionalsources()
        {

            var polAngle = SourceToolbox.UpdatePolarAngleForDirectionalSources(
                _outerRadius,
                _innerRadius,
                _polarAngle);

            Assert.That(Math.Abs(polAngle - _tp[136]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "UpdatePositionAfterTranslation"     
        /// </summary>
        [Test]
        public void Validate_static_method_updatepositionaftertranslation()
        {

            var pos = _position.Clone();
            pos = SourceToolbox.UpdatePositionAfterTranslation(pos, _translation);

            Assert.That(Math.Abs(pos.X - _tp[137]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[138]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[139]), Is.LessThan(AcceptablePrecision));
        }

        /// <summary>
        /// Validating "GetPositionInACircularPerimeter"
        /// </summary>
        [Test]
        public void Validate_static_method_getpositionatcircleperimeter()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionAtCirclePerimeter(_position, _outerRadius, rng);

            Assert.That(Math.Abs(pos.X - _tp[140]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Y - _tp[141]), Is.LessThan(AcceptablePrecision));
            Assert.That(Math.Abs(pos.Z - _tp[142]), Is.LessThan(AcceptablePrecision));
        }
    }
}

