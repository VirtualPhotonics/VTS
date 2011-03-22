using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    public class CustomPointSourceNew : ISource
    {
        private SourceHelper _helper;
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        public CustomPointSourceNew(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            SourceOrientation rotationFromInwardNormal)
        {
            _helper = new SourceHelper(translationFromOrigin, rotationFromInwardNormal);
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
        }

        /// <summary>
        /// Returns an instance of CustomPointSource with a specified translation, pointed normally inward
        /// </summary>
        /// <param name="translationFromOrigin"></param>
        public CustomPointSourceNew(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new SourceOrientation(0, 0))
        {
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // var initialPosition = SampleFiberSurface()
            // var finalPosition = initialPosition + _helper.TranslationFromOrigin;

            var finalPosition = _helper.TranslationFromOrigin;

            // sample angular distribution
            var initialDirection = SourceToolbox.SampleAngularDistributionDirection(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);

            var finalDirection = _helper.RotateDirectionToPrincipalAxis(initialDirection);

            // the handling of specular needs work
            var weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

            var dataPoint = new PhotonDataPoint(
                finalPosition,
                finalDirection,
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
