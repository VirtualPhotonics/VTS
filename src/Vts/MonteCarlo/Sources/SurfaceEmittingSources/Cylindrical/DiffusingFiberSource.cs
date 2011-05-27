using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources.SurfaceEmittingSources.Cylindrical
{
    public class DiffusingFiberSource : ISource
    {
        private CustomCircularSource _fiberFace;
        private LambertianSurfaceEmittingTubularSource _fiberWall;

        public DiffusingFiberSource(
            // whatever you need...defines DiffusingFiberSourceInput
            Position translationFromOrigin,
            Direction newDirectionOfPrincipalSourceAxis)
        {
            // _fiberFace = new CustomCircularSource(...);
            // _fiberWall = new LambertianSurfaceEmittingTubularSource(...);

            // todo: more stuff
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // decide which surface will emmit this photon

            // call GetNextPhoton for that surface

            throw new NotImplementedException();
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