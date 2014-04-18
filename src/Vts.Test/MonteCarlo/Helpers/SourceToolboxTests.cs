using System;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class SourceToolboxTests
    {
        private const double ACCEPTABLE_PRECISION = 0.00000001;
        Position _position;
        Direction _direction;
        Position _translation;
        PolarAzimuthalAngles _angPair;
        DoubleRange _polRange;
        DoubleRange _aziRange;
        ThreeAxisRotation _angRot;
        SourceFlags _flags;
        double _aParameter;
        double _bParameter;
        double _cParameter;
        double _lengthX ;
        double _widthY;
        double _heightZ;
        double _innerRadius;
        double _outerRadius;
        double _bdFWHM;
        double _limitL;
        double _limitU;
        double _factor;
        double _polarAngle;
        double[] _tp = new double[]{1.0000000000e+000,	2.0000000000e+000,	3.0000000000e+000,	7.0710678119e-001,	7.0710678119e-001,	0.0000000000e+000,
                                    1.0000000000e+000,	-2.5000000000e+000,	1.2000000000e+000,	7.8539816340e-001,	7.8539816340e-001,	0.0000000000e+000,	
                                    1.5707963268e+000,	0.0000000000e+000,	3.1415926536e+000,	1.5707963268e+000,	1.5707963268e+000,	7.8539816340e-001,	
                                    2.0000000000e+000,	2.5000000000e+000,	3.0000000000e+000,	1.0000000000e+000,	2.0000000000e+000,	3.0000000000e+000,	
                                    1.0000000000e+000,	2.0000000000e+000,	8.0000000000e-001,	2.5000000000e+000,	5.0000000000e-001,	5.0000000000e-001,	
                                    7.8539816340e-001,	3.1622776602e-001,	6.3245553203e-001,	7.0710678119e-001,	-2.4038566061e-001,	8.0063629303e-001,	
                                    5.4881350243e-001,	-8.3062970822e-001,	-5.4820001436e-001,	9.7627004864e-002,	0.0000000000e+000,	0.0000000000e+000,	
                                    8.8249690258e-001,	9.8985210047e-001,	1.8624762920e+000,	1.5707963268e+000,	7.8539816340e-001,	-5.8910586113e-001,	
                                    1.4967342534e+000,	3.0000000000e+000,	-3.0673325845e-002,	1.3197749533e+000,	3.0000000000e+000,	1.0488135024e+000,	
                                    2.1856892331e+000,	3.6455680954e+000,	7.3017984105e-001,	2.2450786356e+000,	3.4045250143e+000,	1.0488135024e+000,	
                                    2.0000000000e+000,	3.0000000000e+000,	7.3017984105e-001,	2.0000000000e+000,	3.0000000000e+000,	1.1952540097e+000,	
                                    2.4642230826e+000,	3.0000000000e+000,	3.8437662825e-001,	2.3103671369e+000,	3.0000000000e+000,	1.1952540097e+000,	
                                    2.4642230826e+000,	4.2911361908e+000,	3.8437662825e-001,	2.3103671369e+000,	3.4289404236e+000,	1.0488135024e+000,	
                                    2.1856892331e+000,	3.0000000000e+000,	7.3017984105e-001,	2.2450786356e+000,	3.0000000000e+000,	4.8813502432e-002,	
                                    0.0000000000e+000,	5.0000000000e-001,	5.0000000000e-001,	7.0710678119e-001,	7.0710678119e-001,	4.3297802812e-017,
                                    7.0710678119e-001,	4.3297802812e-017,	7.0710678119e-001,	-7.0710678119e-001,	0.0000000000e+000,	1.0000000000e+000,
                                    0.0000000000e+000,	-1.4644660941e-001,	8.5355339059e-001,	-5.0000000000e-001,	-1.4644660941e-001,	8.5355339059e-001,	
                                    -5.0000000000e-001,	1.5857864376e+000,	9.1421356237e-001,	2.6142135624e+000,	-9.2677669530e-001,	2.8033008589e-001,	
                                    -2.5000000000e-001,	1.5857864376e+000,	9.1421356237e-001,	2.6142135624e+000,	7.0710678119e-001,	4.3297802812e-017,
                                    7.0710678119e-001,	1.0000000000e+000,	-3.0000000000e+000,	2.0000000000e+000,	4.3297802812e-017,	7.0710678119e-001,	
                                    -7.0710678119e-001,	3.0000000000e+000,	2.0000000000e+000,	-1.0000000000e+000,	0.0000000000e+000,	1.0000000000e+000,
                                    0.0000000000e+000,	-7.0710678119e-001,	2.1213203436e+000,	3.0000000000e+000,	-1.4644660941e-001,	8.5355339059e-001,
                                    -5.0000000000e-001,	5.8578643763e-001,	3.4142135624e+000,	1.4142135624e+000,	3.9269908170e-001,	2.0000000000e+000,
                                    -5.0000000000e-001,	4.2000000000e+000};

        /// <summary>
        /// Read text data file that has input and output data
        /// </summary>
        public void read_data()
        {

            string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_SourceToolbox.txt";

            if (File.Exists(testpara))
            {
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    for (int i = 0; i < 140; i++)
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
            _bdFWHM = _tp[26];
            _limitL = _tp[27];
            _limitU = _tp[28];
            _factor = _tp[29];
            _polarAngle = _tp[30];
        }

       
        /// <summary>
        /// Validating "GetDirectionForGiven2DPositionAndGivenPolarAngle"     
        /// </summary>
        [Test]
        public void validate_static_method_getdirectionforgiven2dpositionandgivenpolarangle()
        {
            read_data();
            var pos = _position.Clone();
            var dir = SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(_polarAngle, pos);

            Assert.Less(Math.Abs(dir.Ux - _tp[31]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[32]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[33]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetDirectionForGivenPolarAzimuthalAngleRangeRandom"
        /// </summary>
        [Test]
        public void validate_static_method_getdirectionforgivenpolarazimuthalanglerangerandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var dir = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(_polRange, _aziRange, rng);

            Assert.Less(Math.Abs(dir.Ux - _tp[34]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[35]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[36]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetDirectionForIsotropicDistributionRandom"
        /// </summary>
        [Test]
        public void validate_static_method_getdirectionforisotropicdistributionrandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var dir = SourceToolbox.GetDirectionForIsotropicDistributionRandom(rng);

            Assert.Less(Math.Abs(dir.Ux - _tp[37]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[38]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[39]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetDoubleNormallyDistributedRandomNumbers"
        /// </summary>
        [Test]
        public void validate_static_method_getdoublenormallydistributedrandomnumbers()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            double nrng1 = 0.0;
            double nrng2 = 0.0;

           SourceToolbox.GetDoubleNormallyDistributedRandomNumbers(ref nrng1, ref nrng2, _limitL, _limitU, rng);

           Assert.Less(Math.Abs(nrng1 - _tp[40]), ACCEPTABLE_PRECISION);
           Assert.Less(Math.Abs(nrng2 - _tp[41]), ACCEPTABLE_PRECISION);            
        }
        
        /// <summary>
        /// Validating "GetLowerLimit"
        /// </summary>
        [Test]
        public void validate_static_method_getlowerlimit()
        {
            var limit = SourceToolbox.GetLimit(_factor);

            Assert.Less(Math.Abs(limit - _tp[42]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPolarAzimuthalPairForGivenAngleRangeRandom"
        /// </summary>        
        [Test]
        public void validate_static_method_getpolarazimuthalpairforgivenanglerangerandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var angPair = SourceToolbox.GetPolarAzimuthalPairForGivenAngleRangeRandom(_polRange, _aziRange,rng);

            Assert.Less(Math.Abs(angPair.Theta - _tp[43]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(angPair.Phi - _tp[44]), ACCEPTABLE_PRECISION);
        }


        /// <summary>
        /// Validating "GetPolarAzimuthalPairFromDirection"
        /// </summary>
        [Test]
        public void validate_static_method_getpolarazimuthalpairfromdirection()
        {
            Direction dir = _direction;

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var angPair = SourceToolbox.GetPolarAzimuthalPairFromDirection(dir);

            Assert.Less(Math.Abs(angPair.Theta - _tp[45]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(angPair.Phi - _tp[46]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInACircleRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninacirclerandomflat()
        {         

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACircleRandomFlat(_position, _innerRadius, _outerRadius, rng);

            Assert.Less(Math.Abs(pos.X - _tp[47]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[48]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[49]), ACCEPTABLE_PRECISION);
        }
        
        /// <summary>
        /// Validating "GetPositionInACircleRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninacirclerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACircleRandomGaussian(_position, _outerRadius, _innerRadius, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - _tp[50]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[51]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[52]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInACuboidRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninacuboidrandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACuboidRandomFlat(_position, _lengthX, _widthY, _heightZ, rng);

            Assert.Less(Math.Abs(pos.X - _tp[53]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[54]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[55]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInACuboidRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninacuboidrandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACuboidRandomGaussian(_position, 0.5*_lengthX, 0.5*_widthY, 0.5*_heightZ, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - _tp[56]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[57]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[58]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInALineRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninalinerandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInALineRandomFlat(_position, _lengthX, rng);

            Assert.Less(Math.Abs(pos.X - _tp[59]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[60]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[61]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInALineRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninalinerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInALineRandomGaussian(_position, 0.5*_lengthX, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - _tp[62]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[63]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[64]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInAnEllipseRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninanellipserandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipseRandomFlat(_position, _aParameter, _bParameter, rng);

            Assert.Less(Math.Abs(pos.X - _tp[65]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[66]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[67]), ACCEPTABLE_PRECISION);
        }
        
        /// <summary>
        /// Validating "GetPositionInAnEllipseRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninanellipserandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipseRandomGaussian(_position, _aParameter, _bParameter, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - _tp[68]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[69]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[70]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInAnEllipsoidRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninanellipsoidrandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipsoidRandomFlat(_position, _aParameter, _bParameter, _cParameter, rng);

            Assert.Less(Math.Abs(pos.X - _tp[71]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[72]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[73]), ACCEPTABLE_PRECISION);
        }


        /// <summary>
        /// Validating "GetPositionInAnEllipsoidRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninanellipsoidrandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(_position, _aParameter, _bParameter, _cParameter, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - _tp[74]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[75]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[76]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInARectangleRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninarectanglerandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInARectangleRandomFlat(_position, _lengthX, _widthY, rng);

            Assert.Less(Math.Abs(pos.X - _tp[77]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[78]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[79]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionInARectangleRandomGaussian"
        /// </summary>
        [Test]
        public void validate_static_method_getpositioninarectanglerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInARectangleRandomGaussian(_position, 0.5*_lengthX, 0.5*_widthY, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - _tp[80]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[81]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[82]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "GetPositionOfASymmetricalLineRandomFlat"
        /// </summary>
        [Test]
        public void validate_static_method_getpositionofasymmetricallinerandomflat()
        {
            
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var loc = SourceToolbox.GetPositionOfASymmetricalLineRandomFlat(_lengthX, rng);

            Assert.Less(Math.Abs(loc - _tp[83]), ACCEPTABLE_PRECISION);            
        }

        /// <summary>
        /// Validating "GetSingleNormallyDistributedRandomNumber"
        /// </summary>
        [Test]
        public void validate_static_method_getsinglenormallydistributedrandomnumber()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var nrng = SourceToolbox.GetSingleNormallyDistributedRandomNumber(_limitL, rng);

            Assert.Less(Math.Abs(nrng - _tp[84]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundThreeAxis"   
        /// </summary>
  
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundthreeaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundThreeAxis(_angRot, dir);

            Assert.Less(Math.Abs(dir.Ux - _tp[85]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[86]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[87]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundXAxis"  
        /// </summary>
   
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundxaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(_angRot.XRotation, dir);

            Assert.Less(Math.Abs(dir.Ux - _tp[88]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[89]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[90]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundYAxis"  
        /// </summary>
   
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundyaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundYAxis(_angRot.YRotation, dir);

            Assert.Less(Math.Abs(dir.Ux - _tp[91]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[92]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[93]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingAroundZAxis"     
        /// </summary>
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundzaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundZAxis(_angRot.ZRotation, dir);

            Assert.Less(Math.Abs(dir.Ux - _tp[94]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[95]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[96]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionAfterRotatingByGivenAnglePair"   
        /// </summary>
  
        [Test]
        public void validate_static_method_updatedirectionafterrotatingbygivenanglepair()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingByGivenAnglePair(_angPair, dir);

            Assert.Less(Math.Abs(dir.Ux - _tp[97]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[98]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[99]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// //Validating "UpdateDirectionPositionAfterGivenFlags"  
        /// </summary>   
        [Test]
        public void validate_static_method_updatedirectionpositionaftergivenflags1()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(ref pos, ref dir, _angPair, _translation, _flags);

            Assert.Less(Math.Abs(dir.Ux - _tp[100]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[101]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[102]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(pos.X - _tp[103]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[104]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[105]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterGivenFlags"   
        /// </summary>  
        [Test]
        public void validate_static_method_updatedirectionpositionaftergivenflags2()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(ref pos, ref dir, _angPair, _translation, _angPair, _flags);

            Assert.Less(Math.Abs(dir.Ux - _tp[106]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[107]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[108]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(pos.X - _tp[109]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[110]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[111]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterGivenFlags"   
        /// </summary>
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingaroundxaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundXAxis(_angRot.XRotation, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - _tp[112]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[113]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[114]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(pos.X - _tp[115]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[116]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[117]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterRotatingAroundYAxis"  
        /// </summary>   
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingaroundyaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundYAxis(_angRot.YRotation, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - _tp[118]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[119]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[120]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(pos.X - _tp[121]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[122]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[123]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdateDirectionPositionAfterRotatingAroundZAxis"     
        /// </summary>
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingaroundzaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundZAxis(_angRot.ZRotation, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - _tp[124]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[125]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[126]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(pos.X - _tp[127]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[128]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[129]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// //Validating "UpdateDirectionPositionAfterRotatingByGivenAnglePair"
        /// </summary>     
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingbygivenanglepair()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingByGivenAnglePair(_angPair, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - _tp[130]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uy - _tp[131]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(dir.Uz - _tp[132]), ACCEPTABLE_PRECISION);

            Assert.Less(Math.Abs(pos.X - _tp[133]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[134]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[135]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdatePolarAngleForDirectionalSources"     
        /// </summary>
        [Test]
        public void validate_static_method_updatepolarangleFordirectionalsources()
        {

            var polAngle = SourceToolbox.UpdatePolarAngleForDirectionalSources(
                _outerRadius,
                _innerRadius,
                _polarAngle);

            Assert.Less(Math.Abs(polAngle - _tp[136]), ACCEPTABLE_PRECISION);
        }

        /// <summary>
        /// Validating "UpdatePositionAfterTranslation"     
        /// </summary>
        [Test]
        public void validate_static_method_updatepositionaftertranslation()
        {

            var pos = _position.Clone();
            pos = SourceToolbox.UpdatePositionAfterTranslation(pos, _translation);

            Assert.Less(Math.Abs(pos.X - _tp[137]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Y - _tp[138]), ACCEPTABLE_PRECISION);
            Assert.Less(Math.Abs(pos.Z - _tp[139]), ACCEPTABLE_PRECISION);
        }
    }
}

