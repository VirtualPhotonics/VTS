using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class SourceToolboxTests
    {
        Position _position = new Position(1.0, 2.0, 3.0);
        Direction _direction = new Direction(0.707106781186548, 0.707106781186548, 0.0);
        Position _translation = new Position(1.0, -2.5, 1.2);
        PolarAzimuthalAngles _angPair = new PolarAzimuthalAngles(0.25 * Math.PI, 0.25 * Math.PI);        
        DoubleRange _polRange = new DoubleRange(0.0, 0.5 * Math.PI);
        DoubleRange _aziRange = new DoubleRange(0.0,  Math.PI);
        ThreeAxisRotation _angRot = new ThreeAxisRotation(0.5 * Math.PI, 0.5 * Math.PI, 0.25 * Math.PI);
        SourceFlags _flags = new SourceFlags(true, true, true);
        double _aParameter = 2.0;
        double _bParameter = 2.5;
        double _cParameter = 3.0;
        double _lengthX = 1.0;
        double _widthY = 2.0;
        double _heightZ = 3.0;
        double _innerRadius = 1.0;
        double _outerRadius = 2.0;
        double _bdFWHM = 0.8;
        double _limit = 0.5;
        double _factor = 0.5;
        double _polarAngle = 0.25 * Math.PI;
       
        //Validating "GetDirectionForGiven2DPositionAndGivenPolarAngle"     
        [Test]
        public void validate_static_method_getgirectionforgiven2dpositionandgivenpolarangle()
        {
            var pos = _position.Clone();
            var dir = SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(_polarAngle, pos);

            Assert.Less(Math.Abs(dir.Ux - 0.447213595499958), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.894427190999916), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.707106781186548), 0.0000000001);
        }

        //Validating "GetDirectionForGivenPolarAzimuthalAngleRangeRandom"
        [Test]
        public void validate_static_method_getdirectionforgivenpolarazimuthalanglerangerandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var dir = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(_polRange, _aziRange, rng);

            Assert.Less(Math.Abs(dir.Ux + 0.240385660610712), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.800636293032631), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.548813502432037), 0.0000000001);
        }

        //Validating "GetDirectionForIsotropicDistributionRandom"
        [Test]
        public void validate_static_method_getdirectionforisotropicdistributionrandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var dir = SourceToolbox.GetDirectionForIsotropicDistributionRandom(rng);

            Assert.Less(Math.Abs(dir.Ux + 0.830629708217707), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy + 0.548200014362858), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.097627004864073), 0.0000000001);
        }

        //Validating "GetDoubleNormallyDistributedRandomNumbers"
        [Test]
        public void validate_static_method_getdoublenormallydistributedrandomnumbers()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            double nrng1 = 0.0;
            double nrng2 = 0.0;

           SourceToolbox.GetDoubleNormallyDistributedRandomNumbers(ref nrng1, ref nrng2, _limit, rng);

           Assert.Less(Math.Abs(nrng1 + 0.596804014735343), 0.0000000001);
           Assert.Less(Math.Abs(nrng2 + 0.393879446175523), 0.0000000001);            
        }


        //Validating "GetLowerLimit"
        [Test]
        public void validate_static_method_getlowerlimit()
        {
            var limit = SourceToolbox.GetLowerLimit(_factor);

            Assert.Less(Math.Abs(limit - 0.882496902584596), 0.0000000001);
        }


        //Validating "GetPolarAzimuthalPairForGivenAngleRangeRandom"
        [Test]
        public void validate_static_method_getpolarazimuthalpairforgivenanglerangerandom()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var angPair = SourceToolbox.GetPolarAzimuthalPairForGivenAngleRangeRandom(_polRange, _aziRange,rng);

            Assert.Less(Math.Abs(angPair.Theta - 0.989852100466779), 0.0000000001);
            Assert.Less(Math.Abs(angPair.Phi - 1.862476292001275), 0.0000000001);
        }


        //Validating "GetPolarAzimuthalPairFromDirection"
        [Test]
        public void validate_static_method_getpolarazimuthalpairfromdirection()
        {
            Direction dir = new Direction (0.707106781, -0.707106781, 0.0);

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var angPair = SourceToolbox.GetPolarAzimuthalPairFromDirection(dir);

            Assert.Less(Math.Abs(angPair.Theta - 1.570796326794897), 0.0000000001);
            Assert.Less(Math.Abs(angPair.Phi - 5.497787143782138), 0.0000000001);
        }

        //Validating "GetPositionInACircleRandomFlat"
        [Test]
        public void validate_static_method_getpositioninacirclerandomflat()
        {         

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACircleRandomFlat(_position, _innerRadius, _outerRadius, rng);

            Assert.Less(Math.Abs(pos.X + 0.589105861127424), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 1.496734253389650), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }


        //Validating "GetPositionInACircleRandomGaussian"
        [Test]
        public void validate_static_method_getpositioninacirclerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACircleRandomGaussian(_position, _outerRadius, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - 0.397438907668953), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 1.602320990686500), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }

        //Validating "GetPositionInACuboidRandomFlat"
        [Test]
        public void validate_static_method_getpositioninacuboidrandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACuboidRandomFlat(_position, _lengthX, _widthY, _heightZ, rng);

            Assert.Less(Math.Abs(pos.X - 1.048813502432036), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.185689233053869), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 3.645568095414333), 0.0000000001);
        }

        //Validating "GetPositionInACuboidRandomGaussian"
        [Test]
        public void validate_static_method_getpositioninacuboidrandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInACuboidRandomGaussian(_position, _lengthX, _widthY, _heightZ, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - 0.575228350861814), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.302436722670088), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 3.428709966910891), 0.0000000001);
        }

        //Validating "GetPositionInALineRandomFlat"
        [Test]
        public void validate_static_method_getpositioninalinerandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInALineRandomFlat(_position, _lengthX, rng);

            Assert.Less(Math.Abs(pos.X - 1.048813502432036), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - _position.Y), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }

        //Validating "GetPositionInALineRandomGaussian"
        [Test]
        public void validate_static_method_getpositioninalinerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInALineRandomGaussian(_position, _lengthX, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - 0.575228350861814), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - _position.Y), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }

        //Validating "GetPositionInAnEllipseRandomFlat"
        [Test]
        public void validate_static_method_getpositioninanellipserandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipseRandomFlat(_position, _aParameter, _bParameter, rng);

            Assert.Less(Math.Abs(pos.X - 1.195254009728146), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.464223082634672), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }


        //Validating "GetPositionInAnEllipseRandomGaussian"
        [Test]
        public void validate_static_method_getpositioninanellipserandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipseRandomGaussian(_position, _aParameter, _bParameter, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - 1.857911404818847), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 3.073949041976430), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }

        //Validating "GetPositionInAnEllipsoidRandomFlat"
        [Test]
        public void validate_static_method_getpositioninanellipsoidrandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipsoidRandomFlat(_position, _aParameter, _bParameter, _cParameter, rng);

            Assert.Less(Math.Abs(pos.X - 1.19525400972815), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.464223082634672), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 4.29113619082867), 0.0000000001);
        }


        //Validating "GetPositionInAnEllipsoidRandomGaussian"
        [Test]
        public void validate_static_method_getpositioninanellipsoidrandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(_position, _aParameter, _bParameter, _cParameter, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - 0.485914096439413), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.761949256696024), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 2.92131684799626), 0.0000000001);
        }

        //Validating "GetPositionInARectangleRandomFlat"
        [Test]
        public void validate_static_method_getpositioninarectanglerandomflat()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInARectangleRandomFlat(_position, _lengthX, _widthY, rng);

            Assert.Less(Math.Abs(pos.X - 1.048813502432036), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.185689233053869), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }

        //Validating "GetPositionInARectangleRandomGaussian"
        [Test]
        public void validate_static_method_getpositioninarectanglerandomgaussian()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var pos = SourceToolbox.GetPositionInARectangleRandomGaussian(_position, _lengthX, _widthY, _bdFWHM, rng);

            Assert.Less(Math.Abs(pos.X - 0.575228350861814), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.302436722670088), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - _position.Z), 0.0000000001);
        }

        //Validating "GetPositionOfASymmetricalLineRandomFlat"
        [Test]
        public void validate_static_method_getpositionofasymmetricallinerandomflat()
        {
            
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var loc = SourceToolbox.GetPositionOfASymmetricalLineRandomFlat(_lengthX, rng);

            Assert.Less(Math.Abs(loc - 0.048813502432037), 0.0000000001);            
        }

        //Validating "GetSingleNormallyDistributedRandomNumber"
        [Test]
        public void validate_static_method_getsinglenormallydistributedrandomnumber()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var nrng = SourceToolbox.GetSingleNormallyDistributedRandomNumber(_limit, rng);

            Assert.Less(Math.Abs(nrng - -0.596804014735343), 0.0000000001);
        }

        //Validating "UpdateDirectionAfterRotatingAroundThreeAxis"     
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundthreeaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundThreeAxis(_angRot, dir);

            Assert.Less(Math.Abs(dir.Ux - 0.5), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.5), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz -  0.707106781186548), 0.0000000001);
        }

        //Validating "UpdateDirectionAfterRotatingAroundXAxis"     
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundxaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(_angRot.XRotation, dir);

            Assert.Less(Math.Abs(dir.Ux - 0.707106781186548), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.707106781186548), 0.0000000001);
        }

        //Validating "UpdateDirectionAfterRotatingAroundYAxis"     
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundyaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundYAxis(_angRot.YRotation, dir);

            Assert.Less(Math.Abs(dir.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.707106781186548), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz + 0.707106781186548), 0.0000000001);
        }

        //Validating "UpdateDirectionAfterRotatingAroundZAxis"     
        [Test]
        public void validate_static_method_updatedirectionafterrotatingaroundzaxis()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingAroundZAxis(_angRot.ZRotation, dir);

            Assert.Less(Math.Abs(dir.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.0), 0.0000000001);
        }

        //Validating "UpdateDirectionAfterRotatingByGivenAnglePair"     
        [Test]
        public void validate_static_method_updatedirectionafterrotatingbygivenanglepair()
        {
            var dir = _direction.Clone();
            SourceToolbox.UpdateDirectionAfterRotatingByGivenAnglePair(_angPair, dir);

            Assert.Less(Math.Abs(dir.Ux + 0.146446609406726), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.853553390593274), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz + 0.5), 0.0000000001);
        }

        //Validating "UpdateDirectionPositionAfterGivenFlags"     
        [Test]
        public void validate_static_method_updatedirectionpositionaftergivenflags1()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(ref pos, ref dir, _angPair, _translation, _flags);

            Assert.Less(Math.Abs(dir.Ux + 0.146446609406726), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.853553390593274), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz + 0.5), 0.0000000001);

            Assert.Less(Math.Abs(pos.X - 1.585786437626905), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 0.914213562373095), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 2.614213562373095), 0.0000000001);
        }

        //Validating "UpdateDirectionPositionAfterGivenFlags"     
        [Test]
        public void validate_static_method_updatedirectionpositionaftergivenflags2()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(ref pos, ref dir, _angPair, _translation, _angPair, _flags);

            Assert.Less(Math.Abs(dir.Ux + 0.926776695296637), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.280330085889911), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz + 0.25), 0.0000000001);
            
            Assert.Less(Math.Abs(pos.X - 1.585786437626905), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 0.914213562373095), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 2.614213562373095), 0.0000000001);
        }

        //Validating "UpdateDirectionPositionAfterRotatingAroundXAxis"     
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingaroundxaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundXAxis(_angRot.XRotation, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - 0.707106781186548), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.707106781186548), 0.0000000001);

            Assert.Less(Math.Abs(pos.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y + 3.0), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 2.0), 0.0000000001);
        }

        //Validating "UpdateDirectionPositionAfterRotatingAroundYAxis"     
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingaroundyaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundYAxis(_angRot.YRotation, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.707106781186548), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz + 0.707106781186548), 0.0000000001);

            Assert.Less(Math.Abs(pos.X - 3.0), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z + 1.0), 0.0000000001);
        }

        //Validating "UpdateDirectionPositionAfterRotatingAroundZAxis"     
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingaroundzaxis()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundZAxis(_angRot.ZRotation, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz - 0.0), 0.0000000001);

            Assert.Less(Math.Abs(pos.X + 0.707106781186548), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 2.121320343559643), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 3.0), 0.0000000001);
        }

        //Validating "UpdateDirectionPositionAfterRotatingByGivenAnglePair"     
        [Test]
        public void validate_static_method_updatedirectionpositionafterrotatingbygivenanglepair()
        {
            var dir = _direction.Clone();
            var pos = _position.Clone();
            SourceToolbox.UpdateDirectionPositionAfterRotatingByGivenAnglePair(_angPair, ref dir, ref pos);

            Assert.Less(Math.Abs(dir.Ux + 0.146446609406726), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uy - 0.853553390593274), 0.0000000001);
            Assert.Less(Math.Abs(dir.Uz + 0.5), 0.0000000001);

            Assert.Less(Math.Abs(pos.X - 0.585786437626905), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y - 3.414213562373095), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 1.414213562373095), 0.0000000001);
        }

        //Validating "UpdatePositionAfterTranslation"     
        [Test]
        public void validate_static_method_updatepositionaftertranslation()
        {
            
            var pos = _position.Clone();
            pos = SourceToolbox.UpdatePositionAfterTranslation(pos, _translation);           

            Assert.Less(Math.Abs(pos.X - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(pos.Y + 0.5), 0.0000000001);
            Assert.Less(Math.Abs(pos.Z - 4.2), 0.0000000001);
        }
    }
}

