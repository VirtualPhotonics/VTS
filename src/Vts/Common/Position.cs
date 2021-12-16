using System.IO;

namespace Vts.Common
{
    /// <summary>
    /// Represents a 3-dimensional cartesian point in space
    /// </summary>
    public class Position
    {
        private readonly int _hashCode;

        /// <summary>
        /// Returns a 3-dimensional cartesian point in space, based on an x, y, and z position
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <param name="z">The z position</param>
        public Position(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            _hashCode = new { x, y, z }.GetHashCode();
        }

        /// <summary>
        /// Returns a default 3-dimensional cartesian point, located at the origin (x=0, y=0, z=0)
        /// </summary>
        public Position() : this(0.0, 0.0, 0.0) { }

        /// <summary>
        /// The X component of the position
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y component of the position
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The Z component of the position
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Static helper method for calculating the distance between two 3-dimensional points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(Position p1, Position p2)
        {
            return System.Math.Sqrt(
                (p1.X - p2.X)*(p1.X - p2.X) +
                (p1.Y - p2.Y)*(p1.Y - p2.Y) +
                (p1.Z - p2.Z)*(p1.Z - p2.Z));
        }

        /// <summary>
        /// Operator overload for adding two 3D cartesian positions
        /// </summary>
        /// <param name="p1">The first addend position</param>
        /// <param name="p2">The second addend position</param>
        /// <returns>The summed position in 3D cartesian coordinates</returns>
        public static Position operator +(Position p1, Position p2)
        {
            return
                new Position(
                    p1.X + p2.X,
                    p1.Y + p2.Y,
                    p1.Z + p2.Z);
        }

        /// <summary>
        /// Operator overload for subtracting two 3D cartesian positions
        /// </summary>
        /// <param name="p1">The minuend position</param>
        /// <param name="p2">The subtrahend position</param>
        /// <returns>The difference between the two positions, in 3D cartesian coordinates</returns>
        public static Position operator -(Position p1, Position p2)
        {
            return
                new Position(
                    p1.X - p2.X,
                    p1.Y - p2.Y,
                    p1.Z - p2.Z);
        }

        /// <summary>
        /// Equality overload for two 3D cartesian coordinates
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Position p1, Position p2)
        {
            if (object.ReferenceEquals(p1, p2))
            {
                // handles if both are null as well as object identity
                return true;
            }

            if ((object)p1 == null || (object)p2 == null)
            {
                return false;
            }

            return p1.Equals(p2);
        }            

        /// <summary>
        /// Inequality overload for two 3D cartesian coordinates
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Position p1, Position p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Method to write binary version of position
        /// </summary>
        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        /// <summary>
        /// Method to read binary version of position
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>a position</returns>
        public static Position ReadBinary(BinaryReader br)
        {
            return new Position(
                br.ReadDouble(), // X
                br.ReadDouble(), // Y
                br.ReadDouble()); // Z
        }

        /// <summary>
        /// Instance member for equality comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Position)
            {
                var p = obj as Position;
                return
                    X == p.X &&
                    Y == p.Y &&
                    Z == p.Z;
            }
            return false;
        }

        /// <summary>
        /// Method to clone position
        /// </summary>
        /// <returns>a position</returns>
        public Position Clone()
        {
            return new Position(X, Y, Z);
        }

        /// <summary>
        /// Override of GetHashCode to allow the type to work correctly in a hash table
        /// </summary>
        /// <returns>The hashcode as an integer</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
