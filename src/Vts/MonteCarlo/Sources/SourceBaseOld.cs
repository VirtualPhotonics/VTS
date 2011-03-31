using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SourceBaseOld : ISource
    {
        private Random _rng;

        /// <summary>
        /// Base constructor for all sources, specifying a location and orientation
        /// </summary>
        /// <param name="location"></param>
        /// <param name="orientation"></param>
        public SourceBaseOld(Position location, Direction orientation)
        {
            Position = location;
            Orientation = orientation;
        }

        /// <summary>
        /// The source position
        /// </summary>
        public Position Position { get; protected set; }

        /// <summary>
        /// The normal orientation of the source. All source descriptions are relative to this axis.
        /// </summary>
        public Direction Orientation { get; protected set; }

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

        /// <summary>
        /// Contract for derived classes to return a new instance of Photon
        /// </summary>
        /// <param name="tissue">The tissue containing the source</param>
        /// <returns>A new instance of the Photon class</returns>
        public abstract Photon GetNextPhoton(ITissue tissue);
    }
}