using System;
using Vts.Common;
using Vts.Extensions;


namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Utilities shared by Source implementations.
    /// </summary>
    public class SourceToolbox
    {
        
        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatLinePosition(Position center, 
            double lengthX, 
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }
            
            return(new Position(
            center.X + GetFlatRandomLocationForASymmetricalLine(lengthX, rng),
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
        public static Position GetRandomFlatLinePosition(Position center, 
            DoubleRange paraX, 
            Random rng)
        {
            if (paraX.Start == paraX.Stop)
            {
                return (center);
            }

            return (new Position(
            center.X + GetFlatRandomLocationForANonSymmetricalLine(paraX, rng),
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
        public static Position GetRandomGaussianLinePosition(Position center, 
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
                Get1DGaussianDistributedRandomNumberBoxMuller1(ref d, stdevX, rng);
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
        public static Position GetRandomFlatRectangularPosition(Position center, 
            double lengthX, 
            double lengthY, 
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0))
            {
                return (center);
            }

            var position = new Position { Z = center.Z };

            position.X = center.X + GetFlatRandomLocationForASymmetricalLine(lengthX, rng);
            position.Y = center.Y + GetFlatRandomLocationForASymmetricalLine(lengthY, rng);
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
        public static Position GetRandomGaussianRectangularPosition(Position center, 
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
                Get2DGaussianDistributedRandomNumberBoxMuller1(ref x, ref y, stdevX, stdevY, rng);
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
        public static Position GetRandomFlatCuboidPosition(Position center, 
            double lengthX, 
            double lengthY, 
            double lengthZ, 
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            var position = new Position {};

            position.X = center.X + GetFlatRandomLocationForASymmetricalLine(lengthX, rng);
            position.Y = center.Y + GetFlatRandomLocationForASymmetricalLine(lengthY, rng);
            position.Z = center.Z + GetFlatRandomLocationForASymmetricalLine(lengthZ, rng);
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
        public static Position GetRandomFlatCuboidPosition(Position center,
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

            position.X = center.X + GetFlatRandomLocationForANonSymmetricalLine(xAxisRange, rng);
            position.Y = center.Y + GetFlatRandomLocationForANonSymmetricalLine(yAxisRange, rng);
            position.Z = center.Z + GetFlatRandomLocationForANonSymmetricalLine(zAxisRange, rng);
            return position;
        }
              

     
        /// <summary>
        /// Provides a random position in a circle (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatCircularPosition(Position center, double radius, Random rng)
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
        public static Position GetRandomGaussianCircularPosition(Position center, double radius, double stdev, Random rng)
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
                Get2DGaussianDistributedRandomNumberBoxMuller1(ref x, ref y, stdev, stdev, rng);
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
        public static Position GetRandomFlatEllipsePosition(Position center, double a, double b, Random rng)
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
           while (RN1 * RN1  + RN2 * RN2  <= 1.0);
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
        public static Position GetRandomGaussianEllipsePosition(Position center, double a, double b, double stdevX, double stdevY, Random rng)
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
                Get2DGaussianDistributedRandomNumberBoxMuller1(ref x, ref y, stdevX, stdevY, rng);
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
        public static Direction GetRandomAzimuthalAngle(double polarAngle, 
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
        public static Direction GetRandomPolarAngle(DoubleRange polarAngleEmissionRange, 
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
        public static Direction GetRandomPolarAndRandomAzimuthalAngle(DoubleRange polarAngleEmissionRange,
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
        /// Provides the new direction after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection"></param>
        /// <returns></returns>
        public static Direction RotationAroundxAxis(
            double xRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;
            
            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(xRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux;
            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;

            return currentDirection;
        }

        /// <summary>
        /// Provides the new direction after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection"></param>
        /// <returns></returns>
        public static Direction RotationAroundyAxis(
            double yRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(yRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uy = uy;
            currentDirection.Uz =-ux * sint + uz * cost;

            return currentDirection;
        }

        /// <summary>
        /// Provides the new direction after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection"></param>
        /// <returns></returns>
        public static Direction RotationAroundzAxis(
            double zRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(zRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uz * cost;
            currentDirection.Uz = uz;

            return currentDirection;
        }

       /// <summary>
       /// Provides the neew direction after 3-axis rotation
       /// </summary>
       /// <param name="xRotation">rotation angle around the x-axis</param>
       /// <param name="yRotation">rotation angle around the y-axis</param>
       /// <param name="zRotation">rotation angle around the z-axis</param>
       /// <param name="currentDirection"></param>
       /// <returns></returns>
        public static Direction RotationAroundThreeAxisClockwiseLeftHanded(
            double xRotation,
            double yRotation,
            double zRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cosx, sinx, cosy, siny, cosz, sinz;    /* cosine and sine of rotation angles */

            cosx = Math.Cos(xRotation);            
            cosy = Math.Cos(yRotation);            
            cosz = Math.Cos(zRotation);
            sinx = Math.Sqrt(1.0 - cosx * cosx);
            siny = Math.Sqrt(1.0 - cosy * cosy);
            sinz = Math.Sqrt(1.0 - cosz * cosz);

            currentDirection.Ux = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
            currentDirection.Uy = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
            currentDirection.Uz = ux * siny + uy * sinx * cosy + uz * cosx * cosy;

            return currentDirection;
        }

        /// <summary>
        /// Returns a direction when rotation angles are given as polar (theta) and azimuthal (phi) angles
        /// This rotation is given by RotationAroundz * RotationAroundy * CurrentDirection
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        /// <param name="currentDirection"></param>
        /// <returns></returns>
        public static Direction RotateByGivenPolarAndAzimuthalAngle(
            double theta,
            double phi,            
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint, cosp, sinp;    /* cosine and sine of theta and phi */

            cost = Math.Cos(theta);
            cosp = Math.Cos(phi);            
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sqrt(1.0 - cosp * cosp);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp +  uz * cost;

            return currentDirection;
        }

        /// <summary>
        /// Provides a random location of a symmetrical line
        /// </summary>
        /// <param name="length">The length of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetFlatRandomLocationForASymmetricalLine(double length, Random rng)
        {
            return length * (rng.NextDouble() - 0.5);                
        }

        /// <summary>
        /// Provides a random location of a non - symmetrical line
        /// </summary>
        /// <param name="line">Start and end parameters of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetFlatRandomLocationForANonSymmetricalLine(DoubleRange line, Random rng)
        {
            return rng.NextDouble(line.Start, line.Stop);
        }


        /// <summary>
        /// Generate two Gaussian random numbers by using Box Muller Algorithm 1 (without sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>
        /// <param name="stdev2">standard deviation of the normally distributed random number 2</param>
        /// <param name="rng">The random number generator</param>
        public static void Get2DGaussianDistributedRandomNumberBoxMuller1(ref double nrng1, ref double nrng2, double stdev1, double stdev2, Random rng)
        {
            double RN1, RN2;
            double w;

            do
            {
                RN1 = 2.0 * rng.NextDouble() - 1.0;
                RN2 = 2.0 * rng.NextDouble() - 1.0;

                w = RN1 * RN1 + RN2 * RN2;
            } while (w >= 1.0);

            w = Math.Sqrt((-2.0 * Math.Log(w)) / w);

            nrng1 = stdev1 * w * RN1;
            nrng2 = stdev2 * w * RN2;
        }


        /// <summary>
        /// Generate two Gaussian random numbers by using Box Muller Algorithm 1 (without sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>
        /// <param name="rng">The random number generator</param>
        public static void Get1DGaussianDistributedRandomNumberBoxMuller1(ref double nrng1, double stdev1, Random rng)
        {
            double RN1, RN2;
            double w;

            do
            {
                RN1 = 2.0 * rng.NextDouble() - 1.0;
                RN2 = 2.0 * rng.NextDouble() - 1.0;

                w = RN1 * RN1 + RN2 * RN2;
            } while (w >= 1.0);

            w = Math.Sqrt((-2.0 * Math.Log(w)) / w);

            nrng1 = stdev1 * w * RN1;
        }

        /// <summary>
        /// Generate two Gaussian random numbers by using Box Muller Algorithm 1 (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>
        /// <param name="stdev2">standard deviation of the normally distributed random number 2</param>
        /// <param name="rng">The random number generator</param>
        public static void Get2DGaussianDistributedRandomNumberBoxMuller2(ref double nrng1, ref double nrng2, double stdev1, double stdev2, Random rng)
        {
            double RN1, RN2;
            double cosRN1, sinRN1;

            RN1 = rng.NextDouble();
            RN2 = rng.NextDouble();
            cosRN1 = Math.Cos(2 * Math.PI * RN1);
            sinRN1 = Math.Sin(2 * Math.PI * RN1);

            nrng1 = stdev1 * Math.Sqrt(RN2) * cosRN1;
            nrng2 = stdev2 * Math.Sqrt(RN2) * sinRN1;
        }

        /// <summary>
        /// Generate one Gaussian random numbers by using Box Muller Algorithm 1 (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="stdev1">standard deviation of the normally distributed random number 1</param>        
        /// <param name="rng">The random number generator</param>
        public static void Get1DGaussianDistributedRandomNumberBoxMuller2(ref double nrng1, double stdev1, Random rng)
        {
            double RN1, RN2;
            double cosRN1, sinRN1;

            RN1 = rng.NextDouble();
            RN2 = rng.NextDouble();
            cosRN1 = Math.Cos(2 * Math.PI * RN1);
            sinRN1 = Math.Sin(2 * Math.PI * RN1);

            nrng1 = stdev1 * Math.Sqrt(RN2) * cosRN1;
        }
    }
}
