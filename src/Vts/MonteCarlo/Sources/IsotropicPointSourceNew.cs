using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    public class IsotropicPointSource2 : ISource
    {
        private SourceHelper _helper;

        /// <summary>
        /// Creates an instance of IsotropicPointsource given a specified translation from the origin
        /// </summary>
        /// <param name="translationFromOrigin">Translation vector (x,y,z) from origin</param>
        public IsotropicPointSource2(Position translationFromOrigin)
        {
            _helper = new SourceHelper(translationFromOrigin, new SourceOrientation(0, 0));
        }

        /// <summary>
        /// Creates an instance of IsotropicPointSource located at the (0,0,0) Origin
        /// </summary>
        public IsotropicPointSource2()
            : this(new Position(0, 0, 0))
        {
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // create a random radial direction along axis orthogonal to line direction
            var direction = SourceToolbox.SampleIsotropicRadialDirection(Rng);
            var position = _helper.TranslationFromOrigin;

            // for more complex sources with orientation:
            // var sourcePosition = CalculateWhereToPutThePhoton();  //i.e. on the surface of a fiber
            // var finalPosition = _helper.TranslateFromOrigin(sourcePosition);
            // var finalDirection = _helper.RotateDirectionToPrincipalAxis(direction);         

            // the handling of specular needs work
            var weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

            var dataPoint = new PhotonDataPoint(
                position,
                direction,
                weight,
                0.0,
                PhotonStateType.NotSet);

            var photon = new Photon { DP = dataPoint };

            return photon;
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion
    }
}
