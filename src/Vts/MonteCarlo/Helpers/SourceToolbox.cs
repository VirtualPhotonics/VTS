using System;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Utilities shared by Sources.  For the following algorithms the following
    /// references and definitions are provided:
    /// Isotropic source: samples theta according to uniform cos(theta)
    /// (ref: https://en.wikipedia.org/wiki/Isotropic_radiation).
    /// The PDF is p(theta)=sin(theta) which integrated to obtain CDF
    /// P(theta)=cos(theta).  Inverted to sample: RN = cos(theta)
    /// Lambertian source: samples theta according to Lamert's cosine law
    /// (ref: https://en.wikipedia.org/wiki/Lambert%27s_cosine_law)
    /// which states that the angular emission is directly proportional to
    /// the cosine of the angle theta between the angle of propagation and the
    /// surface normal. The PDF is p(theta)=2cos(theta)sin(theta) and associated
    /// CDF P(theta)=sin^2(theta) 
    /// </summary>
    public class SourceToolbox
    {
        /// <summary>
        /// Provides a direction for a given two-dimensional position and a polar angle
        /// </summary>
        /// <param name="polarAngle">Constant polar angle</param>
        /// <param name="position">The position </param>
        /// <returns>direction</returns>
        public static Direction GetDirectionForGiven2DPositionAndGivenPolarAngle(
            double polarAngle,
            Position position)
        {
            var radius = Math.Sqrt(position.X * position.X + position.Y * position.Y);

            if (radius == 0.0)
                return new Direction(
                    0.0,
                    0.0,
                    Math.Cos(polarAngle));
            return new Direction(
                Math.Sin(polarAngle) * position.X / radius,
                Math.Sin(polarAngle) * position.Y / radius,
                Math.Cos(polarAngle));
        }
        
        /// <summary>
        /// Provides an Isotropic direction after uniform sampling of given polar angle range and
        /// azimuthal angle range 
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>direction</returns>
        public static Direction GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            if (polarAngleEmissionRange.Start == polarAngleEmissionRange.Stop && azimuthalAngleEmissionRange.Start == azimuthalAngleEmissionRange.Stop)
                return new Direction(0.0, 0.0, 1.0);

            //sampling cost           
            var cost = rng.NextDouble(Math.Cos(polarAngleEmissionRange.Stop), Math.Cos(polarAngleEmissionRange.Start));
            var sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            var phi = rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop);
            var cosp = Math.Cos(phi);
            var sinp = Math.Sin(phi);

            return new Direction(
                sint * cosp,
                sint * sinp,
                cost);
        }

        /// <summary>
        /// Provides a random direction for an Isotropic point source
        /// </summary>
        /// <param name="rng">The random number generator</param>
        /// <returns>direction</returns>
        public static Direction GetDirectionForIsotropicDistributionRandom(Random rng)
        {
            //sampling cost           
            var cost = 2 * rng.NextDouble() - 1;
            var sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            var phi = rng.NextDouble(0, 2 * Math.PI);
            var cosp = Math.Cos(phi);
            var sinp = Math.Sin(phi);

            return new Direction(
                sint * cosp,
                sint * sinp,
                cost);
        }

        /// <summary>
        /// Provides a Lambertian direction after uniform sampling of given polar angle range and
        /// azimuthal angle range 
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="order">The order of cosine assumed in theta distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>direction</returns>
        public static Direction GetDirectionForGivenPolarAzimuthalAngleRangeLambertianRandom(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            int order,
            Random rng)
        {
            double cost, sint, phi, cosp, sinp;
            if (polarAngleEmissionRange.Start == polarAngleEmissionRange.Stop && azimuthalAngleEmissionRange.Start == azimuthalAngleEmissionRange.Stop)
                return new Direction(0.0, 0.0, 1.0);

            //sampling cost for Lambertian and do until find cost within range specified
            do
            {
                cost = Math.Pow(rng.NextDouble(), 1.0 / (order + 1));
            } while (cost < polarAngleEmissionRange.Start || cost > polarAngleEmissionRange.Stop);
            sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            phi = rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop);
            cosp = Math.Cos(phi);
            sinp = Math.Sin(phi);

            return new Direction(
                sint * cosp,
                sint * sinp,
                cost);
        }

        /// <summary>
        /// Provides a direction after Lambertian emission of given polar angle range and azimuthal angle range 
        /// </summary>
        /// <param name="order">The order of cosine assumed in theta distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>direction</returns>
        public static Direction GetDirectionForLambertianRandom(int order, Random rng)
        {
            double cost, sint, phi, cosp, sinp;
            //sampling cost           
            cost = Math.Pow(rng.NextDouble(), 1.0 / (order + 1));
            sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            phi = rng.NextDouble(0, 2 * Math.PI);
            cosp = Math.Cos(phi);
            sinp = Math.Sin(phi);

            return new Direction(
                sint * cosp,
                sint * sinp,
                cost);
        }       

        /// <summary>
        /// Generate two normally (Gaussian) distributed random numbers by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="lowerLimit">lower limit of the uniform random number</param>
        /// <param name="upperLimit">lower limit of the uniform random number</param>
        /// <param name="rng">The random number generator</param>
        public static void GetDoubleNormallyDistributedRandomNumbers(
            ref double nrng1,
            ref double nrng2,
            double lowerLimit,
            double upperLimit,
            Random rng)
        {
            var rn1 = Math.Sqrt(-2 * Math.Log(rng.NextDouble(lowerLimit, upperLimit)));
            var rn2 = 2 * Math.PI * rng.NextDouble();

            nrng1 = rn1 * Math.Cos(rn2);
            nrng2 = rn1 * Math.Sin(rn2);
        }
                

        /// <summary>
        /// Provides the limit of a flat random distribution between 0 and 1
        /// </summary>
        /// <param name="factor">factor</param>
        /// <returns>lower limit</returns>
        public static double GetLimit(double factor)
        {
            return Math.Exp(-0.5 * factor * factor);
        }


        /// <summary>
        /// Provides polar azimuthal angle pair after uniform sampling of given polar angle range and azimuthal angle range
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>polar azimuthal angle pair</returns>
        public static PolarAzimuthalAngles GetPolarAzimuthalPairForGivenAngleRangeRandom(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            var cosMax = Math.Cos(polarAngleEmissionRange.Start);
            var cosMin = Math.Cos(polarAngleEmissionRange.Stop);
            return new PolarAzimuthalAngles(
                Math.Acos(rng.NextDouble(cosMin, cosMax)),
                rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop));
        }

        /// <summary>
        /// Provides polar azimuthal angle pair for Lambertian Emission of specified order
        /// </summary>
        /// <param name="rng">The random number generato</param>
        /// <returns>polar azimuthal angle pair</returns>
        public static PolarAzimuthalAngles GetPolarAzimuthalPairForLambertianRandom(int lambertOrder, Random rng)
        {
            return new PolarAzimuthalAngles(
                Math.Acos(Math.Pow(rng.NextDouble(0.0, 1.0), 1.0 / (lambertOrder + 1))),
                rng.NextDouble(0, 2 * Math.PI));
        }

        /// <summary>
        /// Provides polar azimuthal angle pair for Lambertian Emission
        /// </summary>
        /// <param name="rng">The random number generato</param>
        /// <returns>polar azimuthal angle pair</returns>
        public static PolarAzimuthalAngles GetPolarAzimuthalPairForLambertianRandom(Random rng)
        {
            return new PolarAzimuthalAngles(
                Math.Acos(Math.Sqrt(rng.NextDouble(0.0, 1.0))),
                rng.NextDouble(0, 2 * Math.PI));
        }

        /// <summary>
        /// Provide corresponding Polar Azimuthal Angle pair for a given direction
        /// </summary>
        /// <param name="direction">Current direction</param>
        /// <returns>polar azimuthal angle pair</returns>
        public static PolarAzimuthalAngles GetPolarAzimuthalPairFromDirection(
            Direction direction)
        {
            if (direction == SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone())
            {
                return new PolarAzimuthalAngles(0.0, 0.0);
            }

            double phi;
            var x = direction.Ux;
            var y = direction.Uy;
            var z = direction.Uz;

            var theta = Math.Acos(z);

            if (x != 0.0 || y != 0.0)
            {
                var r = Math.Sqrt(x * x + y * y);

                if (y >= 0.0)
                    phi = Math.Acos(x / r);
                else
                    phi = 2 * Math.PI - Math.Acos(x / r);
            }
            else
                phi = 0;

            var polarAzimuthalAngles = new PolarAzimuthalAngles(
                theta,
                phi);

            return polarAzimuthalAngles;
        }

        /// <summary>
        /// Provides a random position in a circle (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the circle</param>
        /// <param name="innerRadius">The inner radius of the circle</param>
        /// <param name="outerRadius">The outer radius of the circle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInACircleRandomFlat(
            Position center,
            double innerRadius,
            double outerRadius,
            Random rng)
        {
            if (outerRadius == 0.0)  return center;
            var angle = 2 * Math.PI * rng.NextDouble();
            var radius = Math.Sqrt(innerRadius * innerRadius + (outerRadius * outerRadius - innerRadius * innerRadius) * rng.NextDouble());
            return new Position(
                center.X + radius * Math.Cos(angle),
                center.Y + radius * Math.Sin(angle),
                center.Z);
        }

        /// <summary>
        /// Provides a random position at circle perimeter
        /// </summary>
        /// <param name="center">The center coordinates of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionAtCirclePerimeter(
            Position center,
            double radius,
            Random rng)
        {            
            var angle = 2 * Math.PI * rng.NextDouble();            
            return new Position(
                center.X + radius * Math.Cos(angle),
                center.Y + radius * Math.Sin(angle),
                center.Z);
        }

        /// <summary>
        /// Provides a random position in a circle (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the circle</param>
        /// <param name="outerRadius">The outer radius of the circle</param>
        /// <param name="innerRadius">The inner radius of the circle</param>
        /// <param name="beamDiameterFwhm">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>       
        public static Position GetPositionInACircleRandomGaussian(
            Position center,
            double outerRadius,
            double innerRadius,
            double beamDiameterFwhm,
            Random rng)
        {
            if (outerRadius == 0.0) return center;

            if (beamDiameterFwhm <= 0.0)
                beamDiameterFwhm = 1e-20;

            //https://support.zemax.com/hc/en-us/articles/1500005488161-How-to-convert-FWHM-measurements-to-1-e-2-halfwidths
            var x = 0.0;
            var y = 0.0;
            var factorL = outerRadius / (0.8493218 * beamDiameterFwhm);
            var factorU = innerRadius / (0.8493218 * beamDiameterFwhm);

            GetDoubleNormallyDistributedRandomNumbers(
                ref x,
                ref y,
                GetLimit(factorL),
                GetLimit(factorU),
                rng);

            return new Position(
                center.X + 0.8493218 * beamDiameterFwhm * x,
                center.Y + 0.8493218 * beamDiameterFwhm * y,
                center.Z);
        }

        /// <summary>
        /// Returns a random position in a cuboid volume (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInACuboidRandomFlat(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            Random rng)
        {
            if (lengthX == 0.0 && lengthY == 0.0 && lengthZ == 0.0)
            {
                return center;
            }

            var position = new Position
            {
                X = center.X + GetPositionOfASymmetricalLineRandomFlat(lengthX, rng),
                Y = center.Y + GetPositionOfASymmetricalLineRandomFlat(lengthY, rng),
                Z = center.Z + GetPositionOfASymmetricalLineRandomFlat(lengthZ, rng)
            };

            return position;
        }

        /// <summary>
        /// Returns a random position in a cuboid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the cuboid</param>
        /// <param name="lengthX">The x-coordinate of the length</param>
        /// <param name="lengthY">The y-coordinate of the width</param>
        /// <param name="lengthZ">The z-coordinate of the height</param>
        /// <param name="beamDiameterFwhm">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInACuboidRandomGaussian(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            double beamDiameterFwhm,
            Random rng)
        {
            if (lengthX == 0.0 && lengthY == 0.0 && lengthZ == 0.0) return center;

            var position = new Position(0, 0, 0);

            if (beamDiameterFwhm <= 0.0)
                beamDiameterFwhm = 1e-20;

            var factor = lengthX / (0.8493218 *beamDiameterFwhm);
            position.X = center.X + 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                GetLimit(factor),
                rng);

            factor = lengthY / (0.8493218 * beamDiameterFwhm);
            position.Y = center.Y + 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                GetLimit(factor),
                rng);

            factor = lengthZ / (0.8493218 * beamDiameterFwhm);
            position.Z = center.Z + 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                GetLimit(factor),
                rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordinates of the line</param>
        /// <param name="lengthX">The x-length of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInALineRandomFlat(
            Position center,
            double lengthX,
            Random rng)
        {
            if (lengthX == 0.0)  return center;

            return new Position(
                center.X + GetPositionOfASymmetricalLineRandomFlat(lengthX, rng),
                center.Y,
                center.Z);
        }

        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordinates of the line</param>
        /// <param name="lengthX">The x-coordinate of the length</param>   
        /// <param name="beamDiameterFwhm">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInALineRandomGaussian(
            Position center,
            double lengthX,
            double beamDiameterFwhm,
            Random rng)
        {
            if (lengthX == 0.0) return center;

            if (beamDiameterFwhm <= 0.0)
                beamDiameterFwhm = 1e-20;

            var factor = lengthX / (0.8493218 * beamDiameterFwhm);
            return new Position(
                center.X + 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                    GetLimit(factor),
                    rng),
                center.Y,
                center.Z);

        }

        /// <summary>
        /// Provides a random position in an ellipse (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInAnEllipseRandomFlat(
            Position center,
            double a,
            double b,
            Random rng)
        {
            if (a == 0.0 && b == 0.0) return center;

            double x, y;
            /*eliminate points outside the ellipse */
            do
            {
                x = a * (2.0 * rng.NextDouble() - 1);
                y = b * (2.0 * rng.NextDouble() - 1);
            }
            while (x * x / (a * a) + y * y / (b * b) >= 1.0);
            return new Position(
                center.X + x,
                center.Y + y,
                center.Z);
        }

        /// <summary>
        /// Provides a random position in an ellipse (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="beamDiameterFwhm">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>   
        public static Position GetPositionInAnEllipseRandomGaussian(
            Position center,
            double a,
            double b,
            double beamDiameterFwhm,
            Random rng)
        {
            if (a == 0.0 && b == 0.0)   return center;

            if (beamDiameterFwhm <= 0.0)
                beamDiameterFwhm = 1e-20;

            double x, y;
            var factorA = a / (0.8493218 * beamDiameterFwhm);
            var factorB = b / (0.8493218 * beamDiameterFwhm);


            /*eliminate points outside the ellipse */
            do
            {
                x = 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                    GetLimit(factorA),
                    rng);
                y = 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                    GetLimit(factorB),
                    rng);
            }
            while (x * x / (a * a) + y * y / (b * b) >= 1.0);            
            return new Position(
                center.X + x,
                center.Y + y,
                center.Z);
        }

        /// <summary>
        /// Returns a random position in an ellipsoid volume (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipsoid</param>
        /// <param name="b">'b' parameter of the ellipsoid</param>
        /// <param name="c">'c' parameter of the ellipsoid</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInAnEllipsoidRandomFlat(
            Position center,
            double a,
            double b,
            double c,
            Random rng)
        {
            if (a == 0.0 && b == 0.0 && c == 0.0) return center;

            double x, y, z;
            /*eliminate points outside the ellipse */
            do
            {
                x = a * (2.0 * rng.NextDouble() - 1);
                y = b * (2.0 * rng.NextDouble() - 1);
                z = c * (2.0 * rng.NextDouble() - 1);
            }
            while (x * x / (a * a) + (y * y / (b * b) + z * z / (c * c)) >= 1.0);
            return new Position(
                center.X + x,
                center.Y + y,
                center.Z + z);
        }

        /// <summary>
        /// Returns a random position in an ellipsoid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the ellipsoid</param>
        /// <param name="a">'a' parameter of the ellipsoid</param>
        /// <param name="b">'b' parameter of the ellipsoid</param>
        /// <param name="c">'c' parameter of the ellipsoid</param>
        /// <param name="beamDiameterFwhm">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInAnEllipsoidRandomGaussian(
            Position center,
            double a,
            double b,
            double c,
            double beamDiameterFwhm,
            Random rng)
        {
            if (a == 0.0 && b == 0.0 && c == 0.0) return center;

            if (beamDiameterFwhm <= 0.0)
                beamDiameterFwhm = 1e-20;

            double x, y, z;
            var factorX = a / (0.8493218 * beamDiameterFwhm);
            var factorY = b / (0.8493218 * beamDiameterFwhm);
            var factorZ = c / (0.8493218 * beamDiameterFwhm);


            /*eliminate points outside the ellipse */
            do
            {
                x = 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                    GetLimit(factorX),
                    rng);
                y = 0.8493218 * beamDiameterFwhm *GetSingleNormallyDistributedRandomNumber(
                    GetLimit(factorY),
                    rng);
                z = 0.8493218 * beamDiameterFwhm *GetSingleNormallyDistributedRandomNumber(
                    GetLimit(factorZ),
                    rng);
            }
            while (x * x / (a * a) + (y * y / (b * b) + z * z / (c * c)) >= 1.0);

            return new Position(
                center.X + x,
                center.Y + y,
                center.Z + z);
        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInARectangleRandomFlat(
            Position center,
            double lengthX,
            double lengthY,
            Random rng)
        {
            if (lengthX == 0.0 && lengthY == 0.0) return center;

            var position = new Position
            {
                Z = center.Z,
                X = center.X + GetPositionOfASymmetricalLineRandomFlat(lengthX, rng),
                Y = center.Y + GetPositionOfASymmetricalLineRandomFlat(lengthY, rng)
            };
            return position;
        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordinates of the rectangle</param>
        /// <param name="lengthX">The x-coordinate of the lengthX</param>
        /// <param name="lengthY">The y-coordinate of the widthY</param>
        /// <param name="beamDiameterFwhm">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position</returns>
        public static Position GetPositionInARectangleRandomGaussian(
            Position center,
            double lengthX,
            double lengthY,
            double beamDiameterFwhm,
            Random rng)
        {
            if (lengthX == 0.0 && lengthY == 0.0) return center;

            var position = new Position { Z = center.Z };

            if (beamDiameterFwhm <= 0.0)
                beamDiameterFwhm = 1e-5;
            var factor = lengthX / (0.8493218 * beamDiameterFwhm);             
            position.X = center.X + 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                GetLimit(factor),
                rng);

            factor = lengthY / (0.8493218 * beamDiameterFwhm);
            position.Y = center.Y + 0.8493218 * beamDiameterFwhm * GetSingleNormallyDistributedRandomNumber(
                GetLimit(factor),
                rng);
            return position;
        }

        /// <summary>
        /// Provides a flat random location of a symmetrical line
        /// </summary>
        /// <param name="length">The length of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>position in a line</returns>
        public static double GetPositionOfASymmetricalLineRandomFlat(
            double length,
            Random rng)
        {
            return length * (rng.NextDouble() - 0.5);
        }
        
        /// <summary>
        /// Generate one normally (Gaussian) distributed random number by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="lowerLimit">lower limit of the uniform random number</param>
        /// <param name="rng">The random number generator</param>
        /// <returns>normally distributed random number</returns>
        public static double GetSingleNormallyDistributedRandomNumber(
            double lowerLimit,
            Random rng)
        {
            var a1 = Math.Sqrt(-2 * Math.Log(rng.NextDouble(lowerLimit, 1.0)));
            var b1 = Math.Cos(2 * Math.PI * rng.NextDouble());
            return a1 * b1; 
        }            

        /// <summary>
        /// Update the direction after rotating around three axis
        /// </summary>
        /// <param name="rotationAngles">x, y, z rotation angles </param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns>direction</returns>
        public static Direction UpdateDirectionAfterRotatingAroundThreeAxis(
            ThreeAxisRotation rotationAngles,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            var ux = currentDirection.Ux;
            var uy = currentDirection.Uy;
            var uz = currentDirection.Uz;

            // cosine and sine of rotation angles
            var cosx = Math.Cos(rotationAngles.XRotation);
            var cosy = Math.Cos(rotationAngles.YRotation);
            var cosz = Math.Cos(rotationAngles.ZRotation);
            var sinx = Math.Sin(rotationAngles.XRotation);
            var siny = Math.Sin(rotationAngles.YRotation);
            var sinz = Math.Sin(rotationAngles.ZRotation);

            currentDirection.Ux = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
            currentDirection.Uy = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
            currentDirection.Uz = ux * siny + uy * sinx * cosy + uz * cosx * cosy;

            return currentDirection;
        }

        /// <summary>
        /// Update the direction after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param> 
        /// <returns>Direction class</returns>
        public static Direction UpdateDirectionAfterRotatingAroundXAxis(
            double xRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            var uy = currentDirection.Uy;
            var uz = currentDirection.Uz;

            // cosine and sine of rotation angle
            var cost = Math.Cos(xRotation);
            var sint = Math.Sin(xRotation);

            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Update the direction after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection">The direction to be updated</param> 
        /// <returns>Direction class</returns>
        public static Direction UpdateDirectionAfterRotatingAroundYAxis(
            double yRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            var ux = currentDirection.Ux;
            var uz = currentDirection.Uz;

            // cosine and sine of rotation angle
            var cost = Math.Cos(yRotation);
            var sint = Math.Sin(yRotation);

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uz = -ux * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Update the direction after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns>direction</returns>
        public static Direction UpdateDirectionAfterRotatingAroundZAxis(
            double zRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            var ux = currentDirection.Ux;
            var uy = currentDirection.Uy;

            // cosine and sine of rotation angle
            var cost = Math.Cos(zRotation);
            var sint = Math.Sin(zRotation);

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uy * cost;
            return currentDirection;
        }     
                
        /// <summary>
        /// Update the direction after rotating by given polar and azimuthal angle
        /// </summary>
        /// <param name="rotationAnglePair">polar and azimuthal angle pair</param>        
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns>direction</returns>
        public static Direction UpdateDirectionAfterRotatingByGivenAnglePair(
            PolarAzimuthalAngles rotationAnglePair,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            var ux = currentDirection.Ux;
            var uy = currentDirection.Uy;
            var uz = currentDirection.Uz;

            // cosine and sine of theta and phi
            var cost = Math.Cos(rotationAnglePair.Theta);
            var sint = Math.Sin(rotationAnglePair.Theta);
            var cosp = Math.Cos(rotationAnglePair.Phi);            
            var sinp = Math.Sin(rotationAnglePair.Phi);

            currentDirection.Ux = ux * cost * cosp - uy * sinp + uz * sint * cosp;
            currentDirection.Uy = ux * cost * sinp + uy * cosp + uz * sint * sinp;
            currentDirection.Uz = -ux * sint + uz * cost;

            return currentDirection;
        }

        /// <summary>
        /// Update the direction and position after beam rotation, source axis rotation and translation
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="dir">Direction</param>
        /// <param name="sourceAxisRotation">Source Axis Rotation</param>       
        /// <param name="translate">Translation</param>
        /// <param name="beamRotation">Beam Rotation</param>
        /// <param name="flags">Flags</param>
        public static void UpdateDirectionPositionAfterGivenFlags(
            ref Position pos,
            ref Direction dir,
            PolarAzimuthalAngles sourceAxisRotation,
            Position translate,
            PolarAzimuthalAngles beamRotation,
            SourceFlags flags)
        {
            if (flags.BeamRotationFromInwardNormalFlag)
            {
                var thetaOnly = new PolarAzimuthalAngles(beamRotation.Theta, 0);
                var phiOnly = new PolarAzimuthalAngles(0, beamRotation.Phi);
                // X should have theta angles but phi = 0
                dir = UpdateDirectionAfterRotatingByGivenAnglePair(thetaOnly, dir); 
                if (beamRotation.Phi != 0.0)
                {
                    // Y should have only phi angles theta = 0
                    UpdateDirectionPositionAfterRotatingByGivenAnglePair(
                        phiOnly, ref dir, ref pos); 
                }
            }
            if (flags.RotationOfPrincipalSourceAxisFlag)
                UpdateDirectionPositionAfterRotatingByGivenAnglePair(sourceAxisRotation, ref dir, ref pos);
            if (flags.TranslationFromOriginFlag)
                pos = UpdatePositionAfterTranslation(pos, translate);
        }

        /// <summary>
        /// Update the direction and position after beam axis rotation and translation
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="dir">Direction</param>
        /// <param name="sourceAxisRotation">Source Axis Rotation</param>    
        /// <param name="translate">Translation</param>        
        /// <param name="flags">Flags</param>
        public static void UpdateDirectionPositionAfterGivenFlags(
            ref Position pos,
            ref Direction dir,
            PolarAzimuthalAngles sourceAxisRotation,
            Position translate,
            SourceFlags flags)
        {
            if (flags.RotationOfPrincipalSourceAxisFlag)
                UpdateDirectionPositionAfterRotatingByGivenAnglePair(sourceAxisRotation, ref dir, ref pos);
            if (flags.TranslationFromOriginFlag)
                pos = UpdatePositionAfterTranslation(pos, translate);      
        }              
        
        /// <summary>
        /// Update the direction and position of the source after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void UpdateDirectionPositionAfterRotatingAroundXAxis(
            double xRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            var uy = currentDirection.Uy;
            var uz = currentDirection.Uz;
            var y = currentPosition.Y;
            var z = currentPosition.Z;

            var cost = Math.Cos(xRotation); // cosine of rotation angle
            var sint = Math.Sin(xRotation); // sine of rotation angle

            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;

            currentPosition.Y = y * cost - z * sint;
            currentPosition.Z = y * sint + z * cost;
        }

        /// <summary>
        /// Update the direction and position of the source after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void UpdateDirectionPositionAfterRotatingAroundYAxis(
            double yRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            var ux = currentDirection.Ux;
            var uz = currentDirection.Uz;
            var x = currentPosition.X;
            var z = currentPosition.Z;

            var cost = Math.Cos(yRotation); // cosine of rotation angle
            var sint = Math.Sin(yRotation); // sine of rotation angle

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uz = -ux * sint + uz * cost;

            currentPosition.X = x * cost + z * sint;
            currentPosition.Z = -x * sint + z * cost;
        }

        /// <summary>
        /// Update the direction and position of the source after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void UpdateDirectionPositionAfterRotatingAroundZAxis(
            double zRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            var ux = currentDirection.Ux;
            var uy = currentDirection.Uy;
            var x = currentPosition.X;
            var y = currentPosition.Y;

            var cost = Math.Cos(zRotation);  // cosine of rotation angle
            var sint = Math.Sin(zRotation);  // sine of rotation angle

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uy * cost;

            currentPosition.X = x * cost - y * sint;
            currentPosition.Y = x * sint + y * cost;
        }

        /// <summary>
        /// Update the direction and position of the source after rotating by a given polar and azimuthal angle
        /// </summary>
        /// <param name="rotationAnglePair">polar and azimuthal angle angle pair</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void UpdateDirectionPositionAfterRotatingByGivenAnglePair(
            PolarAzimuthalAngles rotationAnglePair,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;
            double x = currentPosition.X;
            double y = currentPosition.Y;
            double z = currentPosition.Z;

            var cost = Math.Cos(rotationAnglePair.Theta); // cos(theta)
            var sint = Math.Sqrt(1.0 - cost * cost); // sin(theta)
            var cosp = Math.Cos(rotationAnglePair.Phi);  // cos(phi)     
            var sinp = Math.Sin(rotationAnglePair.Phi); // sin(phi)

            currentDirection.Ux = ux * cost * cosp - uy * sinp + uz * sint * cosp;
            currentDirection.Uy = ux * cost * sinp + uy * cosp + uz * sint * sinp;
            currentDirection.Uz = -ux * sint + uz * cost;

            currentPosition.X = x * cost * cosp - y * sinp + z * sint * cosp;
            currentPosition.Y = x * cost * sinp + y * cosp + z * sint * sinp;
            currentPosition.Z = -x * sint + z * cost;
        }

        /// <summary>
        /// Update the polar angle based on incident location
        /// </summary>
        /// <param name="fullLength">Maximum length</param>
        /// <param name="curLength">Current Length</param>
        /// <param name="thetaConvOrDiv">Convergence or Divergence Angle</param>
        /// <returns>polar angle</returns>
        public static double UpdatePolarAngleForDirectionalSources(
            double fullLength,
            double curLength,
            double thetaConvOrDiv)
        {
            if (thetaConvOrDiv == 0.0)
                return thetaConvOrDiv;
            var height = fullLength / Math.Tan(thetaConvOrDiv);
            return Math.Atan(curLength) / height;
        }

        /// <summary>
        /// Update the position after translation
        /// </summary>
        /// <param name="currentPosition">The old location</param>
        /// <param name="translation">Translation coordinates relative to the origin</param>
        /// <returns>position</returns>
        public static Position UpdatePositionAfterTranslation(
            Position currentPosition,
            Position translation)
        {
            currentPosition.X += translation.X;
            currentPosition.Y += translation.Y;
            currentPosition.Z += translation.Z;

            return currentPosition;
        }           
        
    }
}
