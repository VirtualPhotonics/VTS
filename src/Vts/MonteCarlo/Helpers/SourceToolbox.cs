using System;
using System.IO;
using Vts.Common;
using Vts.Extensions;

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
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="rotateBeam"></param>
        /// <param name="rotateAxis"></param>
        /// <param name="translate"></param>
        /// <param name="flags"></param>
        public static void DoRotationandTranslationForGivenFlags(
            ref Position pos, 
            ref Direction dir,
            Position translate,
            PolarAzimuthalAngles rotateBeam, 
            ThreeAxisRotation rotateAxis,             
            SourceFlags flags)
        {
            if(flags.rotationFromInwardNormalFlag)
                dir = GetDirectionAfterRotationByGivenPolarAndAzimuthalAngle(rotateBeam, dir);
            if(flags.RotationOfPrincipalSourceAxisFlag)
                DoSourceRotationAroundThreeAxisClockwiseLeftHanded (rotateAxis, ref dir, ref pos);
            if(flags.TranslationFromOriginFlag)
                pos = GetPositionafterTranslation(pos, translate);
        }

        /// <summary>
        /// Update the direction and position after beam rotation and translation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="rotateBeam"></param>
        /// <param name="translate"></param>
        /// <param name="flags"></param>
        public static void DoRotationandTranslationForGivenFlags(
            ref Position pos,
            ref Direction dir,
            Position translate,
            PolarAzimuthalAngles rotateBeam,    
            SourceFlags flags)
        {
            if (flags.rotationFromInwardNormalFlag)
                dir = GetDirectionAfterRotationByGivenPolarAndAzimuthalAngle(rotateBeam, dir);            
            if (flags.TranslationFromOriginFlag)
                pos = GetPositionafterTranslation(pos, translate);
        }

        /// <summary>
        /// Update the direction and position after source axis rotation and translation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="translate"></param>
        /// <param name="rotateAxis"></param>
        /// <param name="flags"></param>
        public static void DoRotationandTranslationForGivenFlags(
            ref Position pos,
            ref Direction dir,
            Position translate,
            ThreeAxisRotation rotateAxis,
            SourceFlags flags)
        {
            if (flags.RotationOfPrincipalSourceAxisFlag)
                dir = GetDirectionAfterRotationAroundThreeAxisClockwiseLeftHanded(rotateAxis, dir);
            if (flags.TranslationFromOriginFlag)
                pos = GetPositionafterTranslation(pos, translate);
        }

        

        /// <summary>
        /// Update the direction and position after translation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="translate"></param>
        /// <param name="rotateAxis"></param>
        /// <param name="flags"></param>
        public static void DoRotationandTranslationForGivenFlags(
            ref Position pos,
            ref Direction dir,
            PolarAzimuthalAngles rotateBeam,
            ThreeAxisRotation rotateAxis,
            SourceFlags flags)
        {
            if (flags.rotationFromInwardNormalFlag)
                dir = GetDirectionAfterRotationByGivenPolarAndAzimuthalAngle(rotateBeam, dir);
            if (flags.RotationOfPrincipalSourceAxisFlag)
                DoSourceRotationAroundThreeAxisClockwiseLeftHanded(rotateAxis, ref dir, ref pos);
        }   


        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatLinePosition(
            Position center,
            double lengthX,
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }

            return (new Position(
            center.X + GetRandomFlatLocationOfSymmetricalLine(lengthX, rng),
            center.Y,
            center.Z));
        }

        /// <summary>
        /// Returns a random position in a line (Flat distribution)    {overload}    
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="paraX">Minmum and maximum parameters of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatLinePosition(
            Position center,
            DoubleRange paraX,
            Random rng)
        {
            if (paraX.Start == paraX.Stop)
            {
                return (center);
            }

            return (new Position(
            center.X + GetRandomFlatLocationOfAnyLine(paraX, rng),
            center.Y,
            center.Z));
        }


        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomSphericalSurfacePosition(
            Position center,
            double radius,
            Random rng)
        {
            double cost, sint, phi, cosp, sinp;
            double temp;

            if (radius == 0.0)
            {
                return (center);
            }

            //sampling cost           
            cost = rng.NextDouble(0, Math.PI);
            sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            phi = rng.NextDouble(0, 2 * Math.PI);
            cosp = Math.Cos(phi);
            sinp = Math.Sin(phi);

            return (new Position(
            center.X + radius * sinp,
            center.Y,
            center.Z));
        }

        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>   
        /// <param name="stdevX">The standard deviation of the distribution along the x-axis</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianLinePosition(
            Position center,
            double lengthX,
            double stdevX,
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }

            double d = 0.0;
            do
            {
                OneGaussianDistributedRandomNumber(ref d, stdevX, rng);
            }
            while ((d >= -0.5 * lengthX) && (d <= 0.5 * lengthX));
            return (new Position(
            center.X + d,
            center.Y,
            center.Z));

        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatRectangulePosition(
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

            position.X = center.X + GetRandomFlatLocationOfSymmetricalLine(lengthX, rng);
            position.Y = center.Y + GetRandomFlatLocationOfSymmetricalLine(lengthY, rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="stdevX">The standard deviation of the distribution along the x-axis</param>
        /// <param name="stdevY">The standard deviation of the distribution along the y-axis</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianRectangulePosition(
            Position center,
            double lengthX,
            double lengthY,
            double stdevX,
            double stdevY,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0))
            {
                return (center);
            }

            var position = new Position { Z = center.Z };
            double x = 0.0;
            double y = 0.0;

            do
            {
                TwoGaussianDistributedRandomNumbers(ref x, ref y, stdevX, stdevY, rng);
            }
            while ((x >= -0.5 * lengthX) && (x <= 0.5 * lengthX) && (y >= -0.5 * lengthY) && (y <= 0.5 * lengthY));
            position.X = center.X + x;
            position.Y = center.Y + y;
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
        public static Position GetRandomFlatCuboidPosition(
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

            position.X = center.X + GetRandomFlatLocationOfSymmetricalLine(lengthX, rng);
            position.Y = center.Y + GetRandomFlatLocationOfSymmetricalLine(lengthY, rng);
            position.Z = center.Z + GetRandomFlatLocationOfSymmetricalLine(lengthZ, rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a cuboid volume (Flat distribution) {overload}
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="xAxisRange">The minimum and maximum parameters of the x-length</param>
        /// <param name="yAxisRange">The minimum and maximum parameters of the y-length</param>
        /// <param name="zAxisRange">The minimum and maximum parameters of the z-length</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatCuboidPosition(
            Position center,
            DoubleRange xAxisRange,
            DoubleRange yAxisRange,
            DoubleRange zAxisRange,
            Random rng)
        {
            if ((xAxisRange.Start == xAxisRange.Stop) && (yAxisRange.Start == yAxisRange.Stop) && (zAxisRange.Start == zAxisRange.Stop))
            {
                return (center);
            }

            var position = new Position { };

            position.X = center.X + GetRandomFlatLocationOfAnyLine(xAxisRange, rng);
            position.Y = center.Y + GetRandomFlatLocationOfAnyLine(yAxisRange, rng);
            position.Z = center.Z + GetRandomFlatLocationOfAnyLine(zAxisRange, rng);
            return position;
        }


        /// <summary>
        /// Returns a random position in a cuboid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>
        /// <param name="stdevX">The standard deviation of the distribution along the x-axis</param>
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="stdevY">The standard deviation of the distribution along the y-axis</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="stdevZ">The standard deviation of the distribution along the z-axis</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianCuboidPosition(
            Position center,
            double lengthX,
            double stdevX,
            double lengthY,
            double stdevY,
            double lengthZ,
            double stdevZ,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            Position position = new Position (0, 0, 0);

            position.X = center.X + GetRandomGaussianLocationOfSymmetricalLine(lengthX, stdevX, rng);
            position.Y = center.X + GetRandomGaussianLocationOfSymmetricalLine(lengthY, stdevY, rng);
            position.Z = center.X + GetRandomGaussianLocationOfSymmetricalLine(lengthZ, stdevZ, rng);
            return position;
        }

        /// <summary>
        /// Provides a random position in a circle (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatCircularPosition(
            Position center, 
            double radius, 
            Random rng)
        {
            if (radius == 0.0)
            {
                return (center);
            }

            double RN1 = rng.NextDouble();
            double RN2 = rng.NextDouble();
            double cosRN1 = Math.Cos(2 * Math.PI * RN1);
            double sinRN1 = Math.Sin(2 * Math.PI * RN1);
            return (new Position(
                center.X + radius * Math.Sqrt(RN2) * cosRN1,
                center.Y + radius * Math.Sqrt(RN2) * sinRN1,
                0.0));
        }
        /// <summary>
        /// Provides a random position in a circle (Gaussisan distribution)
        /// <summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="stdev">The standard deviation of the distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>       
        public static Position GetRandomGaussianCircularPosition(
            Position center,
            double radius,
            double stdev,
            Random rng)
        {
            if (radius == 0.0)
            {
                return (center);
            }

            double x = 0.0;
            double y = 0.0;
            double r;

            /*eliminate points outside the circle */
            do
            {
                TwoGaussianDistributedRandomNumbers(ref x, ref y, stdev, stdev, rng);
                r = Math.Sqrt(x * x + y * y);
            }
            while (r <= radius);

            return (new Position(
                center.X + radius * x,
                center.Y + radius * y,
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
        public static Position GetRandomFlatEllipsePosition(
            Position center,
            double a,
            double b,
            Random rng)
        {
            if ((a == 0.0) && (b == 0.0))
            {
                return (center);
            }

            double RN1, RN2;
            /*eliminate points outside the ellipse */
            do
            {
                RN1 = 2 * rng.NextDouble() - 1;
                RN2 = 2 * rng.NextDouble() - 1;
            }
            while (RN1 * RN1 + RN2 * RN2 <= 1.0);
            return (new Position(
                center.X + a * RN1,
                center.Y + b * RN2,
                0.0));
        }

        /// <summary>
        /// Provides a random position in an ellipse (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="stdevX">The standard deviation of the distribution along the x-axis</param>
        /// <param name="stdevY">The standard deviation of the distribution along the y-axis</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>   
        public static Position GetRandomGaussianEllipsePosition(
            Position center,
            double a,
            double b,
            double stdevX,
            double stdevY,
            Random rng)
        {
            if ((a == 0.0) && (b == 0.0))
            {
                return (center);
            }

            double x = 0.0;
            double y = 0.0;
            double r;
            /*eliminate points outside the ellipse */
            do
            {
                TwoGaussianDistributedRandomNumbers(
                    ref x, 
                    ref y, 
                    stdevX, 
                    stdevY, 
                    rng);
                r = x * x + y * y;
            }
            while (r <= 1.0);

            return (new Position(
                center.X + a * x,
                center.Y + b * y,
                center.Z));
        }


        /// <summary>
        /// Provides a direction for a given random azimuthal angle range and constant polar angle
        /// </summary>
        /// <param name="polarAngle">Constant polar angle</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Direction GetRandomAzimuthalAngle(
            double polarAngle,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            double cost, sint, phi, cosp, sinp;

            cost = Math.Cos(polarAngle);
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

        /// <summary>
        /// Provides a direction for a given random polar angle range and constant azimuthal angle
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngle">Constant azimuthal angle</param>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static Direction GetRandomDirectionForAPolarAngle(
            DoubleRange polarAngleEmissionRange,
            double azimuthalAngle,
            Random rng)
        {
            double cost, sint, cosp, sinp;

            //sampling cost           
            cost = rng.NextDouble(Math.Cos(polarAngleEmissionRange.Start), Math.Cos(polarAngleEmissionRange.Stop));
            sint = Math.Sqrt(1.0 - cost * cost);

            cosp = Math.Cos(azimuthalAngle);
            sinp = Math.Sin(azimuthalAngle);

            return (new Direction(
                sint * cosp,
                sint * sinp,
                cost));
        }


        /// <summary>
        /// Provides a direction for a given random polar angle range and random azimuthal angle range
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static Direction GetRandomDirectionForPolarAndAzimuthalAngleRange(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            double cost, sint, phi, cosp, sinp;

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

        
        /// <summary>
        /// Provides a polarazimuthal angle pair for a given uniform random polar angle range and random azimuthal angle range
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static PolarAzimuthalAngles GetRandomPolarAzimuthalForUniformPolarAndAzimuthalAngleRange(
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
        public static Direction GetRandomDirectionForIsotropicDistribution(Random rng)
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
        public static void DoSourceRotationAroundxAxis(
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
        public static void DoSourceRotationAroundyAxis(
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
        public static void DoSourceRotationAroundzAxis(
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
        /// Update the direction and position of the source after 3-axis rotation
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void DoSourceRotationAroundThreeAxisClockwiseLeftHanded(
            ThreeAxisRotation threeAxisRotation,
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

            double cosx, sinx, cosy, siny, cosz, sinz;    /* cosine and sine of rotation angles */

            cosx = Math.Cos(threeAxisRotation.XRotation);
            cosy = Math.Cos(threeAxisRotation.YRotation);
            cosz = Math.Cos(threeAxisRotation.ZRotation);
            sinx = Math.Sqrt(1.0 - cosx * cosx);
            siny = Math.Sqrt(1.0 - cosy * cosy);
            sinz = Math.Sqrt(1.0 - cosz * cosz);

            currentDirection.Ux = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
            currentDirection.Uy = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
            currentDirection.Uz = ux * siny + uy * sinx * cosy + uz * cosx * cosy;

            currentPosition.X = x * cosy * cosz + y * (-cosx * sinz + sinx * siny * cosz) + z * (sinx * sinz + cosx * siny * cosz);
            currentPosition.Y = x * cosy * sinz + y * (cosx * cosz + sinx * siny * sinz) + z * (-sinx * cosz + cosx * siny * sinz);
            currentPosition.Z = x * siny + y * sinx * cosy + z * cosx * cosy;
        }

        /// <summary>
        /// Update the direction and position of the source after rotating by a given polar and azimuthal angle
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void DoSourceRotationByGivenPolarAndAzimuthalAngle(
            PolarAzimuthalAngles rotationAngle,
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

            cost = Math.Cos(rotationAngle.Theta);
            cosp = Math.Cos(rotationAngle.Phi);
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sqrt(1.0 - cosp * cosp);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp + uz * cost;

            currentPosition.X = x * cosp * cost - y * sint + z * sinp * cost;
            currentPosition.Y = x * cosp * sint + y * cost + z * sinp * sint;
            currentPosition.Z = -x * sinp + z * cost;
        }

        /// <summary>
        /// Provide the direction after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static Direction GetDirectionAfterRotationAroundxAxis(
            double xRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(xRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Provide the direction after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static Direction GetDirectionAfterRotationAroundyAxis(
            double yRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(yRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uz = -ux * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Provide the direction after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static Direction GetDirectionAfterRotationAroundzAxis(
            double zRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(zRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uy * cost;
            return currentDirection;
        }

        /// <summary>
        /// Provide the direction after rotating around three axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static Direction GetDirectionAfterRotationAroundThreeAxisClockwiseLeftHanded(
            ThreeAxisRotation rotationAngles,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cosx, sinx, cosy, siny, cosz, sinz;    /* cosine and sine of rotation angles */

            cosx = Math.Cos(rotationAngles.XRotation);
            cosy = Math.Cos(rotationAngles.YRotation);
            cosz = Math.Cos(rotationAngles.ZRotation);
            sinx = Math.Sqrt(1.0 - cosx * cosx);
            siny = Math.Sqrt(1.0 - cosy * cosy);
            sinz = Math.Sqrt(1.0 - cosz * cosz);

            currentDirection.Ux = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
            currentDirection.Uy = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
            currentDirection.Uz = ux * siny + uy * sinx * cosy + uz * cosx * cosy;
            return currentDirection;
        }

        /// <summary>
        /// Provide the direction after rotating by given polar and azimuthal angle
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static Direction GetDirectionAfterRotationByGivenPolarAndAzimuthalAngle(
            PolarAzimuthalAngles rotationAngle,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint, cosp, sinp;    /* cosine and sine of theta and phi */

            cost = Math.Cos(rotationAngle.Theta);
            cosp = Math.Cos(rotationAngle.Phi);
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sqrt(1.0 - cosp * cosp);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Provides a random location of a non - symmetrical line
        /// </summary>
        /// <param name="linepara">Start and end parameters of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetRandomFlatLocationOfAnyLine(
            DoubleRange linepara,
            Random rng)
        {
            return rng.NextDouble(linepara.Start, linepara.Stop);
        }

        /// <summary>
        /// Provides a flat random location of a symmetrical line
        /// </summary>
        /// <param name="length">The length of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetRandomFlatLocationOfSymmetricalLine(
            double length, 
            Random rng)
        {            
            return length * (rng.NextDouble() - 0.5);
        }

        /// <summary>
        /// Provides a Gaussian random location of a symmetrical line
        /// </summary>
        /// <param name="length">The length of the line</param>
        /// <param name="stdev">Standard deviation</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetRandomGaussianLocationOfSymmetricalLine(
            double length, 
            double stdev, 
            Random rng)
        {
            double nrng = 0.0;
            do { OneGaussianDistributedRandomNumber(ref nrng, 0.5 * length * stdev, rng); }
            while(Math.Abs(nrng) <= 0.5*length);
            return nrng;
        }

                              

        /// <summary>
        /// Generate one normally (Gaussian) distributed random number by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>        
        /// <param name="rng">The random number generator</param>
        public static void OneGaussianDistributedRandomNumber(
            ref double nrng1, 
            double stdev1, 
            Random rng)
        {
            double RN1, RN2;

            RN1 = rng.NextDouble();
            RN2 = rng.NextDouble();
            nrng1 = stdev1 * Math.Sqrt(-2 * Math.Log(RN2)) * Math.Cos(2 * Math.PI * RN1);
        }

        /// <summary>
        /// Generate two normally (Gaussian) distributed random numbers by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>
        /// <param name="stdev2">standard deviation of the normally distributed random number 2</param>
        /// <param name="rng">The random number generator</param>
        public static void TwoGaussianDistributedRandomNumbers(
            ref double nrng1, 
            ref double nrng2, 
            double stdev1, 
            double stdev2, 
            Random rng)
        {
            double RN1, RN2;
            double cosRN1, sinRN1;

            RN1 = rng.NextDouble();
            RN2 = rng.NextDouble();
            cosRN1 = Math.Cos(2 * Math.PI * RN1);
            sinRN1 = Math.Sin(2 * Math.PI * RN1);

            nrng1 = stdev1 * Math.Sqrt(-2 * Math.Log(RN2)) * cosRN1;
            nrng2 = stdev2 * Math.Sqrt(-2 * Math.Log(RN2)) * sinRN1;
        }


        /// <summary>
        /// Generate three normally (Gaussian) distributed random numbers by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="nrng3">normally distributed random number 3</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>
        /// <param name="stdev2">standard deviation of the normally distributed random number 2</param>
        /// <param name="stdev3">standard deviation of the normally distributed random number 3</param>
        /// <param name="rng">The random number generator</param>
        public static void ThreeGaussianDistributedRandomNumbers(
            ref double nrng1,
            ref double nrng2,
            ref double nrng3,
            double stdev1,
            double stdev2,
            double stdev3,
            Random rng)
        {
            OneGaussianDistributedRandomNumber(ref nrng1, stdev1, rng);
            TwoGaussianDistributedRandomNumbers(ref nrng2, ref nrng3, stdev2, stdev3, rng);
        }


        /// <summary>
        /// Returns the new position after translation
        /// </summary>
        /// <param name="oldPosition">The old location</param>
        /// <param name="translation">Translation coordinats relative to the origin</param>
        /// <returns></returns>
        public static Position GetPositionafterTranslation(
            Position oldPosition, 
            Position translation)
        {
            return (new Position(
                oldPosition.X + translation.X,
                oldPosition.Y + translation.Y,
                oldPosition.Z + translation.Z));
        }


        /// <summary>
        /// Update the direction and position in the cuboid surface after sampling
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="position"></param>
        /// <param name="l"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="rng"></param>
        public static void DoDirectionAndPositionForCuboidSurface(
            ref Direction direction,
            ref Position position,
            double l,
            double w,
            double h,
            Random rng)
        {
            double lw, hw, lh;
            double temp1;
            Position tempPosition = new Position();

            lw = l * w;
            hw = h * w;
            lh = l * h;

            temp1= 2 * (lw + hw + lh) *rng.NextDouble();

            if (temp1 < lw)
            {
                position = GetRandomFlatRectangulePosition(tempPosition, l, w, rng);
                position.X = tempPosition.X;
                position.Y = tempPosition.Y;
                position.Z = 0.5*h;                
            }
            else if (temp1 < 2*lw)
            {
                position = GetRandomFlatRectangulePosition(tempPosition, l, w, rng);
                position.X = tempPosition.X;
                position.Y = tempPosition.Y;
                position.Z = -0.5*h;

                direction = GetDirectionAfterRotationAroundxAxis(Math.PI, direction);
            }
            else if (temp1 < (2 * lw + hw) )
            {
                position = GetRandomFlatRectangulePosition(tempPosition, h, w, rng);
                position.X = 0.5 * l;
                position.Y = tempPosition.Y;
                position.Z = tempPosition.X;

                direction = GetDirectionAfterRotationAroundyAxis(0.5*Math.PI, direction);
            }
            else if (temp1 < 2 * (lw + hw))
            {
                position = GetRandomFlatRectangulePosition(tempPosition, h, w, rng);
                position.X = -0.5 * l;
                position.Y = tempPosition.Y;
                position.Z = tempPosition.X;

                direction = GetDirectionAfterRotationAroundyAxis(-0.5 * Math.PI, direction);
            }
            else if (temp1 < (2 * (lw + hw) + lh))
            {
                position = GetRandomFlatRectangulePosition(tempPosition, l, h, rng);
                position.X = tempPosition.X;
                position.Y = 0.5 * w;
                position.Z = tempPosition.Y;

                direction = GetDirectionAfterRotationAroundxAxis(0.5 * Math.PI, direction);
            }
            else 
            {
                position = GetRandomFlatRectangulePosition(tempPosition, l, h, rng);
                position.X = tempPosition.X;
                position.Y = -0.5 * w;
                position.Z = tempPosition.Y;

                direction = GetDirectionAfterRotationAroundxAxis(-0.5 * Math.PI, direction);
            }
        }

        public static double NAToPolarAngle(double numericalAperture)
        {
            return Math.Asin(numericalAperture);
        }
    }
}
