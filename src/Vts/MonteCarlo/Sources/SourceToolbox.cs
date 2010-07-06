using System;
using Vts.Common;
using Vts.MonteCarlo.RNG.Extensions;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Utilities shared by Source implementations.
    /// </summary>
    public class SourceToolbox
    {
        /// <summary>
        /// AngleDistributed provides a Lambertian Direction selected from thetaRange and phiRange
        /// </summary>
        /// <param name="thetaRange"></param>
        /// <param name="phiRange"></param>
        /// <param name="rng"></param>
        /// <returns>Direction</returns>
        /// CKH TODO:  add in rotation to solidAngleAxis
        public static Direction GetAngleDistributedDirection(Direction solidAngleAxis,
            DoubleRange thetaRange, DoubleRange phiRange, Random rng)
        {
            double theta = rng.NextDouble(Math.Acos(thetaRange.Start), Math.Acos(thetaRange.Stop));
            double phi = rng.NextDouble(phiRange.Stop, phiRange.Start);
            return (new Direction(
                Math.Cos(theta) * Math.Sin(phi),
                Math.Sin(theta) * Math.Sin(phi),
                Math.Cos(phi)));
            // previous algorithm (uses rejection sampling)
        //public Direction AngleDistributed(double numericalAperture, double refractiveIndex,
        //    Generator rng)
            //double theta = 2.0 * Math.PI * rng.NextDouble();
            //double cost = Math.Cos(theta);
            //double sint = Math.Sin(theta);
            //double cosp, sinp;
            //do
            //{
            //    cosp = rng.NextDouble(); // has min max overload
            //    sinp = Math.Sqrt(1.0 - cosp * cosp);
            //} while (sinp > (numericalAperture / refractiveIndex));
            //return new Direction(cost * sinp, sint * sinp, cosp);
        }
        public static Position GetFlatCircularPosition(Position center, double radius, Random rng)
        {
            if (radius == 0.0)
            {
                return (center);
            }
            else
            {
                double RN1 = rng.NextDouble();
                double RN2 = rng.NextDouble();
                double cosRN2 = Math.Cos(2 * Math.PI * RN2);
                double sinRN2 = Math.Sin(2 * Math.PI * RN2);
                return(new Position(
                    center.X + radius * Math.Sqrt(RN1) * cosRN2,
                    center.Y + radius * Math.Sqrt(RN1) * sinRN2,
                    0.0));
            }
        }
        public static Position GetGaussianCircularPosition(Position center, double radius, Random rng)
        {
            if (radius == 0.0)
            {
                return (center);
            }
            else
            {
                double RN1 = rng.NextDouble();
                double RN2 = rng.NextDouble();
                double cosRN2 = Math.Cos(2 * Math.PI * RN2);
                double sinRN2 = Math.Sin(2 * Math.PI * RN2);
                /* Gaussian beam starting coordinates as given on p24 in Ch4 of AJW book  */
                if (RN1 == 1.0) RN1 = rng.NextDouble();
                return (new Position(
                    center.X + radius * Math.Sqrt(-Math.Log(1.0 - RN1) / 2.0) * cosRN2,
                    center.Y + radius * Math.Sqrt(-Math.Log(1.0 - RN1) / 2.0) * sinRN2,
                    0.0));
            }
        }
    }
}
