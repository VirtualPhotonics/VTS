using System;
using System.IO;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Utilities shared by Sources.
    /// </summary>
    public class SourceToolbox
    {
        /// <summary>
        /// Update the direction and position after beam rotation, source axis rotation and translation
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="dir">Direction</param>
        /// <param name="sourceAxisRotation">Source Axis Rotation</param>       
        /// <param name="translate">Translation</param>
        /// <param name="beamRotation">Beam Rotation</param>
        /// <param name="flags">Flags</param>
        public static void UpdateDirectionAndPositionAfterGivenFlags(
            ref Position pos,
            ref Direction dir,
            PolarAzimuthalAngles sourceAxisRotation,
            Position translate,
            PolarAzimuthalAngles beamRotation,
            SourceFlags flags)
        {
            if (flags.RotationOfPrincipalSourceAxisFlag)
                UpdateDirectionAfterRotatingByGivenPolarAndAzimuthalAnglePair(beamRotation, ref dir); 
            if (flags.BeamRotationFromInwardNormalFlag)
                UpdateDireactionAndPositionAfterRotatingByGivenPolarAndAzimuthalAngle(sourceAxisRotation, ref dir, ref pos);            
            if (flags.TranslationFromOriginFlag)
                UpdatePositionAfterTranslation(ref pos, translate);
        }

        /// <summary>
        /// Update the direction and position after beam axis rotation and translation
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="dir">Direction</param>
        /// <param name="sourceAxisRotation">Source Axis Rotation</param>    
        /// <param name="translate">Translation</param>        
        /// <param name="flags">Flags</param>
        public static void UpdateDirectionAndPositionAfterGivenFlags(
            Position pos,
            ref Direction dir,
            PolarAzimuthalAngles sourceAxisRotation,
            Position translate,
            SourceFlags flags)
        {
            if (flags.BeamRotationFromInwardNormalFlag)
                UpdateDireactionAndPositionAfterRotatingByGivenPolarAndAzimuthalAngle(sourceAxisRotation, ref dir, ref pos); 
            if (flags.TranslationFromOriginFlag)
                UpdatePositionAfterTranslation(ref pos, translate);
        }

                
        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInALineRandomFlat(
            Position center,
            double lengthX,
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }

            return (new Position(
            center.X + GetLocationOfASymmetricalLineRandomFlat(lengthX, rng),
            center.Y,
            center.Z));
        }

        
        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>   
        /// <param name="beamDiaFWHM">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInALineRandomGaussian(
            Position center,
            double lengthX,
            double beamDiaFWHM,
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }
            else
            {
                double factor = lengthX / beamDiaFWHM;
                return (
                    new Position(center.X + 0.8493218 * beamDiaFWHM * GetSingleNormallyDistributedRandomNumber(
                           GetLowerLimit(factor), 
                           rng),
                        center.Y,
                        center.Z));
            }

        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInARectangleRandomFlat(
            Position center,
            double lengthX,
            double lengthY,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0))
            {
                return (center);
            }

            var position = new Position { Z = center.Z };
            position.X = center.X + GetLocationOfASymmetricalLineRandomFlat(lengthX, rng);
            position.Y = center.Y + GetLocationOfASymmetricalLineRandomFlat(lengthY, rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="beamDiaFWHM">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInARectangleRandomGaussian(
            Position center,
            double lengthX,
            double lengthY,
            double beamDiaFWHM,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0))
            {
                return (center);
            }

            var position = new Position { Z = center.Z };
            double factor = lengthX/beamDiaFWHM;

            position.X = center.X + 0.8493218 * beamDiaFWHM * GetSingleNormallyDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);

            factor = lengthY/beamDiaFWHM;
            position.Y = center.Y + 0.8493218 * beamDiaFWHM * GetSingleNormallyDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a cuboid volume (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInACuboidRandomFlat(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            var position = new Position { };

            position.X = center.X + GetLocationOfASymmetricalLineRandomFlat(lengthX, rng);
            position.Y = center.Y + GetLocationOfASymmetricalLineRandomFlat(lengthY, rng);
            position.Z = center.Z + GetLocationOfASymmetricalLineRandomFlat(lengthZ, rng);
            return position;
        }


        /// <summary>
        /// Returns a random position in a cuboid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="beamDiaFWHM">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInACuboidRandomGaussian(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            double beamDiaFWHM,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            Position position = new Position(0, 0, 0);

            double factor = lengthX / beamDiaFWHM;
            position.X = center.X + 0.8493218 * beamDiaFWHM * GetSingleNormallyDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);

            factor = lengthY / beamDiaFWHM;
            position.Y = center.Y + 0.8493218 * beamDiaFWHM * GetSingleNormallyDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);

            factor = lengthZ / beamDiaFWHM;
            position.Z = center.Z + 0.8493218 * beamDiaFWHM * GetSingleNormallyDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in an ellipsoid volume (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="a2Parameter">2 x aParameter</param>        
        /// <param name="b2Parameter">2 x bParameter</param>
        /// <param name="c2Parameter">2 x cParameter</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInAnEllipsoidRandomFlat(
            Position center,
            double a2Parameter,
            double b2Parameter,
            double c2Parameter,
            Random rng)
        {
            if ((a2Parameter == 0.0) && (b2Parameter == 0.0) && (c2Parameter == 0.0))
            {
                return (center);
            }

            var position = new Position { };
            double radius;
            do
            {
                position = GetPositionInACuboidRandomFlat(center,
                    a2Parameter,
                    b2Parameter,
                    c2Parameter,
                    rng);
                radius = (4.0 * position.X * position.X / (a2Parameter * a2Parameter) +
                          4.0 * position.Y * position.Y / (b2Parameter * b2Parameter) +
                          4.0 * position.Z * position.Z / (c2Parameter * c2Parameter));
            } while (radius <= 1.0);
            return position;
        }


        /// <summary>
        /// Returns a random position in an ellipsoid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipsoid</param>
        /// <param name="a2Parameter">2 x aParameter</param>        
        /// <param name="b2Parameter">2 x bParameter</param>
        /// <param name="c2Parameter">2 x cParameter</param>
        /// <param name="beamDiaFWHM">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInAnEllipsoidRandomGaussian(
            Position center,
            double a2Parameter,
            double b2Parameter,
            double c2Parameter,
            double beamDiaFWHM,
            Random rng)
        {
            if ((a2Parameter == 0.0) && (b2Parameter == 0.0) && (c2Parameter == 0.0))
            {
                return (center);
            }

            var position = new Position { };
            double radius;
            do
            {
                position = GetPositionInACuboidRandomGaussian(center,
                    a2Parameter,
                    b2Parameter,
                    c2Parameter,
                    beamDiaFWHM,
                    rng);

                radius = (4.0 * position.X * position.X / (a2Parameter * a2Parameter) +
                          4.0 * position.Y * position.Y / (b2Parameter * b2Parameter) +
                          4.0 * position.Z * position.Z / (c2Parameter * c2Parameter));
            } while (radius <= 1.0);
            return position;
        }

        /// <summary>
        /// Provides a random position in a circle (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="innerRadius">The inner radius of the circle</param>
        /// <param name="outerRadius">The outer radius of the circle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositionInACircleRandomFlat(
            Position center,
            double innerRadius,
            double outerRadius,
            Random rng)
        {
            if (outerRadius == 0.0)
            {
                return (center);
            }
            double RN1 = 2 * Math.PI * rng.NextDouble();
            double RN2 = Math.Sqrt(innerRadius * innerRadius + (outerRadius * outerRadius - innerRadius * innerRadius) * rng.NextDouble());
            return (new Position(
                center.X + RN2 * Math.Cos(RN1),
                center.Y + RN2 * Math.Sin(RN1),
                0.0));
        }

        /// <summary>
        /// Provides a random position in a circle (Gaussisan distribution)
        /// <summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="outerRadius">The outer radius of the circle</param>
        /// <param name="beamDiaFWHM">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>       
        public static Position GetPositonInACircleRandomGaussian(
            Position center,
            double outerRadius,
            double beamDiaFWHM,
            Random rng)
        {
            if (outerRadius == 0.0)
            {
                return (center);
            }

            double x = 0.0;
            double y = 0.0;
            double factor = outerRadius / beamDiaFWHM;

            GetDoubleNormallyDistributedRandomNumbers(
                ref x,
                ref y,
                GetLowerLimit(factor),
                rng);

            return (new Position(
                center.X + 0.8493218 * beamDiaFWHM * x,
                center.Y + 0.8493218 * beamDiaFWHM * y,
                center.Z));
        }

        /// <summary>
        /// Provides a random position in an ellipse (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetPositonInAnEllipseRandomFlat(
            Position center,
            double a,
            double b,
            Random rng)
        {
            if ((a == 0.0) && (b == 0.0))
            {
                return (center);
            }

            double x, y;
            /*eliminate points outside the ellipse */
            do
            {
                x = a * (2.0 * rng.NextDouble() - 1);
                y = b * (2.0 * rng.NextDouble() - 1);
            }
            while ((x * x / (a * a)) + (y * y / (b * b)) <= 1.0);
            return (new Position(
                center.X + a * x,
                center.Y + b * y,
                0.0));
        }

        /// <summary>
        /// Provides a random position in an ellipse (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="beamDiaFWHM">Beam diameter at FWHM</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>   
        public static Position GetPositonInAnEllipseRandomGaussian(
            Position center,
            double a,
            double b,
            double beamDiaFWHM,
            Random rng)
        {
            if ((a == 0.0) && (b == 0.0))
            {
                return (center);
            }

            double x, y;
            double factor1 = 2 * a / beamDiaFWHM;
            double factor2 = 2 * b / beamDiaFWHM;


            /*eliminate points outside the ellipse */
            do
            {
                x = GetSingleNormallyDistributedRandomNumber(
                    GetLowerLimit(factor1),
                    rng);
                y = GetSingleNormallyDistributedRandomNumber(
                    GetLowerLimit(factor2),
                    rng);
            }
            while ((x * x / (a * a)) + (y * y / (b * b)) <= 1.0);

            return (new Position(
                center.X + a * x,
                center.Y + b * y,
                center.Z));
        }


        /// <summary>
        /// Provides a direction for a given two dimensional position and a polar angle
        /// </summary>
        /// <param name="polarAngle">Constant polar angle</param>
        /// <param name="position">The position </param>
        /// <returns></returns>
        public static Direction GetDirectionForGiven2DPositionAndGivenPolarAngle(
            double polarAngle,
            Position position)
        {
            double radius = Math.Sqrt(position.X * position.X + position.Y * position.Y);

            if (radius == 0.0)
                return(new Direction(
                    0.0,
                    0.0,
                    Math.Cos(polarAngle)));
            else
                return (new Direction(
                    position.X / radius,
                    position.Y / radius,
                    Math.Cos(polarAngle)));
        }

        /// <summary>
        /// Provides a direction after uniform sampling of given polar angle range and azimuthal angle range 
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Direction GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            double cost, sint, phi, cosp, sinp;

            if ((polarAngleEmissionRange.Start == polarAngleEmissionRange.Stop) && (azimuthalAngleEmissionRange.Start == azimuthalAngleEmissionRange.Stop))
                return (new Direction(0.0, 0.0, 1.0));
            else
            {
                //sampling cost           
                cost = rng.NextDouble(Math.Cos(polarAngleEmissionRange.Start), Math.Cos(polarAngleEmissionRange.Stop));
                sint = Math.Sqrt(1.0 - cost * cost);

                //sampling phi
                phi = rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop);
                cosp = Math.Cos(phi);
                sinp = Math.Sin(phi);

                return (new Direction(
                    sint * cosp,
                    sint * sinp,
                    cost));
            }
        }

        
        /// <summary>
        /// Provides polar azimuthal angle pair after uniform sampling of given polar angle range and azimuthal angle range
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static PolarAzimuthalAngles GetRandomPolarAzimuthalPairForGivenPolarAndAzimuthalAngleRangeRandom(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            return (new PolarAzimuthalAngles(
                Math.Acos(rng.NextDouble(Math.Cos(polarAngleEmissionRange.Start), Math.Cos(polarAngleEmissionRange.Stop))),
                rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop)));
        }

        /// <summary>
        /// Provides a random direction for a isotropic point source
        /// </summary>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static Direction GetDirectionForIsotropicDistributionRandom(Random rng)
        {
            double cost, sint, phi, cosp, sinp;

            //sampling cost           
            cost = rng.NextDouble(0, Math.PI);
            sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            phi = rng.NextDouble(0, 2 * Math.PI);
            cosp = Math.Cos(phi);
            sinp = Math.Sin(phi);

            return (new Direction(
                sint * cosp,
                sint * sinp,
                cost));
        }

        /// <summary>
        /// Update the direction and position of the source after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void UpdateDireactionAndPositionAfterRotatingAroundXAxis(
            double xRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;
            double y = currentPosition.Y;
            double z = currentPosition.Z;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(xRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

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
        public static void UpdateDireactionAndPositionAfterRotatingAroundYAxis(
            double yRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uz = currentDirection.Uz;
            double x = currentPosition.X;
            double z = currentPosition.Z;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(yRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

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
        public static void UpdateDireactionAndPositionAfterRotatingAroundZAxis(
            double zRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double x = currentPosition.X;
            double y = currentPosition.Y;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(zRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

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
        public static void UpdateDireactionAndPositionAfterRotatingByGivenPolarAndAzimuthalAngle(
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

            double cost, sint, cosp, sinp;    /* cosine and sine of theta and phi */

            cost = Math.Cos(rotationAnglePair.Theta);
            cosp = Math.Cos(rotationAnglePair.Phi);
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sin(rotationAnglePair.Phi);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp + uz * cost;

            currentPosition.X = x * cosp * cost - y * sint + z * sinp * cost;
            currentPosition.Y = x * cosp * sint + y * cost + z * sinp * sint;
            currentPosition.Z = -x * sinp + z * cost;
        }

        /// <summary>
        /// Update the direction after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>       
        public static Direction UpdateDirectionAfterRotatingAroundXAxis(
            double xRotation,
            ref Direction currentDirection)
        {
            // readability eased with local copies of following
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(xRotation);
            sint = Math.Sin(xRotation);

            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Update the direction after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        public static Direction UpdateDirectionAfterRotatingAroundYAxis(
            double yRotation,
            ref Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(yRotation);
            sint = Math.Sin(yRotation);

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uz = -ux * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Update the direction after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static Direction UpdateDirectionAfterRotatingAroundZAxis(
            double zRotation,
            ref Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(zRotation);
            sint = Math.Sin(zRotation);

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uy * cost;
            return currentDirection;
        }

        /// <summary>
        /// Update the direction after rotating around three axis
        /// </summary>
        /// <param name="rotationAngles">x, y, z rotation angles </param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static void UpdateDirectionAfterRotatingAroundThreeAxisClockwiseLeftHanded(
            ThreeAxisRotation rotationAngles,
            ref Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cosx, sinx, cosy, siny, cosz, sinz;    /* cosine and sine of rotation angles */

            cosx = Math.Cos(rotationAngles.XRotation);
            cosy = Math.Cos(rotationAngles.YRotation);
            cosz = Math.Cos(rotationAngles.ZRotation);
            sinx = Math.Sin(rotationAngles.XRotation);
            siny = Math.Sin(rotationAngles.YRotation);
            sinz = Math.Sin(rotationAngles.ZRotation);

            currentDirection.Ux = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
            currentDirection.Uy = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
            currentDirection.Uz = ux * siny + uy * sinx * cosy + uz * cosx * cosy;
        }

        /// <summary>
        /// Update the direction after rotating by given polar and azimuthal angle
        /// </summary>
        /// <param name="rotationAnglePair">polar and azimuthal angle pair</param>        
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static void UpdateDirectionAfterRotatingByGivenPolarAndAzimuthalAnglePair(
            PolarAzimuthalAngles rotationAnglePair,
            ref Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint, cosp, sinp;    /* cosine and sine of theta and phi */

            cost = Math.Cos(rotationAnglePair.Theta);
            cosp = Math.Cos(rotationAnglePair.Phi);
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sin(rotationAnglePair.Phi);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp + uz * cost;
        }

        /// <summary>
        /// Provide corresponding Polar Azimuthal Angle pair for a given direction
        /// </summary>
        /// <param name="direction">Current direction</param>
        /// <returns></returns>
        public static PolarAzimuthalAngles GetPolarAndAzimuthalAnglesFromDirection(
            Direction direction)
        {
            if (direction == SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone().Clone())
            {
                return new PolarAzimuthalAngles(0.0, 0.0);
            }

            double x, y, z, r, theta, phi;
            x = direction.Ux;
            y = direction.Uy;
            z = direction.Uz;

            theta = Math.Acos(z);

            if ((x != 0.0) || (y != 0.0))
            {
                r = Math.Sqrt(x * x + y * y);

                if (y >= 0.0)
                    phi = Math.Acos(x / r);
                else
                    phi = 2 * Math.PI - Math.Acos(x / r);
            }
            else
                phi = 0;        

            PolarAzimuthalAngles polarAzimuthalAngles = new PolarAzimuthalAngles(
                theta,
                phi);

            return polarAzimuthalAngles;
        }
              
        /// <summary>
        /// Provides a flat random location of a symmetrical line
        /// </summary>
        /// <param name="length">The length of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetLocationOfASymmetricalLineRandomFlat(
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
        public static double GetSingleNormallyDistributedRandomNumber(
            double lowerLimit,
            Random rng)
        {
            return Math.Sqrt(-2 * Math.Log(rng.NextDouble(lowerLimit, 1.0))) * Math.Cos(2 * Math.PI * rng.NextDouble());
        }

        /// <summary>
        /// Generate two normally (Gaussian) distributed random numbers by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="lowerLimit">lower limit of the uniform random number</param>
        /// <param name="rng">The random number generator</param>
        public static void GetDoubleNormallyDistributedRandomNumbers(
            ref double nrng1,
            ref double nrng2,
            double lowerLimit,
            Random rng)
        {
            double RN1, RN2;

            RN1 = 2 * Math.PI * rng.NextDouble();
            RN2 = Math.Sqrt(-2 * Math.Log(rng.NextDouble(lowerLimit, 1.0)));

            nrng1 = RN2 * Math.Cos(RN1);
            nrng2 = RN2 * Math.Sin(RN1);
        }


        /// <summary>
        /// Update the position after translation
        /// </summary>
        /// <param name="oldPosition">The old location</param>
        /// <param name="translation">Translation coordinats relative to the origin</param>
        /// <returns></returns>
        public static void UpdatePositionAfterTranslation(
            ref Position oldPosition,
            Position translation)
        {
            oldPosition.X += translation.X;
            oldPosition.Y += translation.Y;
            oldPosition.Z += translation.Z;
        }
        
        /// <summary>
        /// Provides the lower limit of a flat random distribution between 0 and 1
        /// </summary>
        /// <param name="factor">factor</param>
        /// <returns></returns>
        public static double GetLowerLimit(double factor)
        {
            return (Math.Exp(-0.5*factor*factor));
        }
    }
}
