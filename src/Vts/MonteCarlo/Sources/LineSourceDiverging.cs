using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class LineSourceDiverging : ISource
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;
        private Position _translationFromOrigin;
        private PolarAzimuthalRotationAngles _rotationFromInwardNormal;
        private PolarAzimuthalRotationAngles _rotationOfPrincipalSourceAxis;
        private double _lineLength;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public LineSourceDiverging(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            PolarAzimuthalRotationAngles rotationFromInwardNormal,
            PolarAzimuthalRotationAngles rotationOfPrincipalAxis,
            double linelength)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationFromInwardNormal = rotationFromInwardNormal.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalAxis.Clone();
        }
               
        /// <summary>
        /// Returns an instance of CustomPointSource with a specified translation, pointed normally inward
        /// </summary>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public LineSourceDiverging(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new PolarAzimuthalRotationAngles(0, 0),
                new PolarAzimuthalRotationAngles(0, 0),
                1.0)
                
        {
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from the origin
            Position initialPosition = SourceToolbox.GetRandomFlatLinePosition(new Position(0,0,0), _lineLength, Rng);

            _polarAngleEmissionRange.Start = 0.0;
            _polarAngleEmissionRange.Stop = Math.PI / 2;

            _azimuthalAngleEmissionRange.Start = 0.0;
            _azimuthalAngleEmissionRange.Stop = 2 * Math.PI;


            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);                      
         

            //If source rotation angles are not equal to zero, rotate the source, and update the position and the direction
            if ((_rotationOfPrincipalSourceAxis.ThetaRotation == 0.0) && (_rotationOfPrincipalSourceAxis.PhiRotation == 0.0))
            { }
            else
            { SourceToolbox.DoSourceRotationByGivenPolarAndAzimuthalAngle(_rotationOfPrincipalSourceAxis, ref finalDirection, ref initialPosition); }

            Position finalPosition = initialPosition;
            //if translate the photon            
            if ((_translationFromOrigin.X == 0.0) && (_translationFromOrigin.Y == 0.0) && (_translationFromOrigin.Z == 0.0))
            { }
            else
            { finalPosition = SourceToolbox.GetPositionafterTranslation(initialPosition, _translationFromOrigin); }                 




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
