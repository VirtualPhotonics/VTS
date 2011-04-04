using Vts.Common;
using System;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{       
    public class SourceHelper
    {
        private Position _translationFromOrigin;
        private PolarAzimuthalAngles _rotationFromInwardNormal;
        private Direction _rotatedPrincipalSourceAxis;

        public SourceHelper(
            Position _translationFromOrigin,
            PolarAzimuthalAngles _rotationFromInwardNormal)
        {
            _translationFromOrigin = _translationFromOrigin.Clone();
            _rotationFromInwardNormal = _rotationFromInwardNormal.Clone();
            _rotatedPrincipalSourceAxis = GetUltimatePrincipalAxisFromPolarOrientation(_rotationFromInwardNormal);
        }

        private Direction GetUltimatePrincipalAxisFromPolarOrientation(
            PolarAzimuthalAngles polarAzimuthalOrientation)
        {
            // todo: Janaka help
            return new Direction();
        }

        public Position TranslationFromOrigin
        {
            get { return _translationFromOrigin; }
            set { _translationFromOrigin = value; }
        }

        public PolarAzimuthalAngles RotationFromInwardNormal
        {
            get { return _rotationFromInwardNormal; }
            set { _rotationFromInwardNormal = value; }
        }

        public Direction RotatedPrincipalSourceAxis
        {
            get { return _rotatedPrincipalSourceAxis; }
            set { _rotatedPrincipalSourceAxis = value; }
        }

        public Direction RotateDirectionToPrincipalAxis(Direction input)
        {
            // combine input and RotatedPrincipalSourceAxis (or RotationFromInwardNormal)
            // to create a final direction
            // todo: implement

            var finalDirection = input; //temp
            return finalDirection;
        }

    }



    public static class DemoRngInjection
    {
        public static void Demo()
        {
            var seed = 2;
            var rng = new MathNet.Numerics.Random.MersenneTwister(seed);
            var tissue = new MultiLayerTissue();

            var isotropicSource = new PointSourceIsotropic(new Position(0, 0, 1))
            {
                Rng = rng
            };

            var customSource = new PointSourceCustom(
                new DoubleRange(0, Math.PI / 2),
                new DoubleRange(0, Math.PI),
                new Position(0, 0, 1),
                new PolarAzimuthalAngles(0, 0)) //todo: is SourceOrientation still desirable?
            {
                Rng = rng
            };

            isotropicSource.GetNextPhoton(tissue);
        }
    }
}
