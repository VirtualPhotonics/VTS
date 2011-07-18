using System;
using NUnit.Framework;
using Vts.Common;
using System.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources
    /// </summary>
    [TestFixture]
    public class SurfaceEmittingSourcesTests
    {
        Position _position;
        Direction _direction;
        Position _translation;
        PolarAzimuthalAngles _angPair;
        DoubleRange _polRange;
        DoubleRange _aziRange;
        double _lengthX;
        double _widthY;
        double _heightZ; 
        double _aParameter;
        double _bParameter;
        double _cParameter;
        double _outRad;
        double _inRad;
        double _bdFWHM;
        double _polarAngle;
        double[] _tp = new double[] {0.0000000000e+000,	0.0000000000e+000,	0.0000000000e+000,	7.0710678119e-001,	7.0710678119e-001,
                                     0.0000000000e+000,	1.0000000000e+000,	-2.5000000000e+000,	1.2000000000e+000,	7.8539816340e-001,	
                                     7.8539816340e-001,	0.0000000000e+000,	1.5707963268e+000,	0.0000000000e+000,	3.1415926536e+000,
                                     1.0000000000e+000,	2.0000000000e+000,	3.0000000000e+000,	8.0000000000e-001,	1.2000000000e+000,
                                     1.5000000000e+000,	1.0000000000e+000,	2.0000000000e+000,	8.0000000000e-001,	7.8539816340e-001,
                                     4.6708668105e-001,	8.6503758455e-001,	1.8313931776e-001,	1.3558626222e+000,	-2.8558626222e+000,
                                     2.7891058611e+000,	4.6708668105e-001,	8.6503758455e-001,	1.8313931776e-001,	1.2812015242e+000,
                                    -2.7812015242e+000,	1.8025610923e+000,	6.0275777831e-001,	7.3715866524e-001,	-3.0541801347e-001,	
                                     1.3558626222e+000,	-2.8558626222e+000,	2.7891058611e+000,	4.4318371553e-001,	7.6505385557e-001,
                                    -4.6720530002e-001,	1.2812015242e+000,	-2.7812015242e+000,	1.8025610923e+000, 4.6708668105e-001,	
                                    8.6503758455e-001,	1.8313931776e-001,	8.4243746094e-001,	-2.3424374609e+000,	1.1218983961e+000,	
                                    5.3044120905e-001,	7.9209406120e-001,	3.0202503529e-001,	7.3820582588e-001,	-2.2382058259e+000,	
                                    1.6511677007e+000,	-7.5207139095e-002,	9.3896259407e-001,	-3.3569797908e-001,	8.4243746094e-001,	
                                    -2.3424374609e+000,	1.1218983961e+000,	3.2584130462e-001,	9.3811159230e-001,	1.1736304608e-001,	
                                    7.3820582588e-001,	-2.2382058259e+000,	1.6511677007e+000,  4.6708668105e-001,	8.6503758455e-001,
                                    1.8313931776e-001,	8.6869788411e-001,	-2.3686978841e+000,	1.1511864976e+000,	5.3044120905e-001,
                                    7.9209406120e-001,	3.0202503529e-001,	7.8614494252e-001,	-2.2861449425e+000,	1.6247716491e+000,
                                    1.1722944862e-002,	9.2240192747e-001,	-3.8605343771e-001,	8.6869788411e-001,	-2.3686978841e+000,
                                    1.1511864976e+000,	3.1725332473e-001,	9.4608294465e-001,	-6.5401756809e-002,	7.8614494252e-001,
                                    -2.2861449425e+000,	1.6247716491e+000} ;


        /// <summary>
        /// Read text data file that has input and output data
        /// </summary>
        public void read_data()
        {
            string testpara = "../../../../matlab/test/monte_carlo/source_test_data_generation/UnitTests_SurfaceEmitting2DSources.txt";

            if (File.Exists(testpara))
            {
                using (TextReader reader = File.OpenText(testpara))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    for (int i = 0; i < 97; i++)
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
            _widthY = _tp[16];
            _heightZ = _tp[17];
            _aParameter = _tp[18];
            _bParameter = _tp[19];
            _cParameter = _tp[20];
            _inRad = _tp[21];
            _outRad = _tp[22];
            _bdFWHM = _tp[23];
            _polarAngle = _tp[24];
        }

        /// <summary>
        /// Validate General Constructor of Custom Flat Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_circular_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();             
            var profile = new FlatSourceProfile();


            var ps = new CustomCircularSource(_outRad, _inRad, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[25]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[26]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[27]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[28]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[29]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[30]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of Custom Gaussian Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_circular_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new CustomCircularSource(_outRad, _inRad, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[31]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[32]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[33]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[34]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[35]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[36]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of Directional Flat Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_circular_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new DirectionalCircularSource(_polarAngle, _outRad, _inRad, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[37]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[38]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[39]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[40]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[41]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[42]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of directional Gaussian Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_directional_circular_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new DirectionalCircularSource(_polarAngle, _outRad, _inRad, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[43]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[44]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[45]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[46]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[47]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[48]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of Custom Flat Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_elliptical_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new CustomEllipticalSource(_aParameter, _bParameter, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[49]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[50]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[51]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[52]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[53]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[54]), 0.0000000001);
        }

        /// <summary>
        /// Validate General Constructor of Custom Gaussian Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_elliptical_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new CustomEllipticalSource(_aParameter, _bParameter, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[55]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[56]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[57]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[58]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[59]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[60]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of Directional Flat Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_elliptical_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new DirectionalEllipticalSource(_polarAngle, _aParameter, _bParameter, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[61]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[62]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[63]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[64]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[65]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[66]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of directional Gaussian Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_directional_elliptical_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new DirectionalEllipticalSource(_polarAngle, _aParameter, _bParameter, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[67]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[68]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[69]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[70]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[71]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[72]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of Custom Flat Rectangular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_rectangular_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new CustomRectangularSource(_lengthX, _widthY, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[73]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[74]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[75]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[76]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[77]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[78]), 0.0000000001);
        }

        /// <summary>
        /// Validate General Constructor of Custom Gaussian Rectangular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_rectangular_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new CustomRectangularSource(_lengthX, _widthY, profile, _polRange, _aziRange, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[79]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[80]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[81]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[82]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[83]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[84]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of Directional Flat Rectangular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_rectangular_source_test()
        {
            read_data();

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();


            var ps = new DirectionalRectangularSource(_polarAngle, _lengthX, _widthY, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[85]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[86]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[87]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[88]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[89]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[90]), 0.0000000001);
        }


        /// <summary>
        /// Validate General Constructor of directional Gaussian Rectangular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_directional_rectangular_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_bdFWHM);


            var ps = new DirectionalRectangularSource(_polarAngle, _lengthX, _widthY, profile, _direction, _translation, _angPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _tp[91]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _tp[92]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _tp[93]), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - _tp[94]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _tp[95]), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _tp[96]), 0.0000000001);
        }
    }
}
