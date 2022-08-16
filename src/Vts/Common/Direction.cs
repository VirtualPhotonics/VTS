using System.IO;
using System.Runtime.CompilerServices;

namespace Vts.Common
{
    /// <summary>
    /// The <see cref="Common"/> namespace contains the common classes for the Virtual Tissue Simulator
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Class describes unit directional vector.
    /// </summary>
    public class Direction
    {
        private readonly int _hashCode;

        /// <summary>
        /// Constructor for direction, passes x, y and z values
        /// </summary>
        /// <param name="ux">The x direction</param>
        /// <param name="uy">The y direction</param>
        /// <param name="uz">The z direction</param>
        public Direction(double ux, double uy, double uz)
        {
            Ux = ux;
            Uy = uy;
            Uz = uz;
            _hashCode = new {ux, uy, uz}.GetHashCode();
        }

        /// <summary>
        /// Default constructor for direction (0,0,1)
        /// </summary>
        public Direction()
            : this(0.0, 0.0, 1.0)
        {
        }

        /// <summary>
        /// x direction
        /// </summary>
        public double Ux { get; set; }

        /// <summary>
        /// y direction
        /// </summary>
        public double Uy { get; set; }

        /// <summary>
        /// z direction
        /// </summary>
        public double Uz { get; set; }

        /// <summary>
        /// Equality overload for two 3D direction cosines
        /// </summary>
        /// <param name="d1">The first direction cosine</param>
        /// <param name="d2">The second direction cosine</param>
        /// <returns>A Boolean indicating whether d1 equals d2</returns>
        public static bool operator ==(Direction d1, Direction d2)
        {
            if (object.ReferenceEquals(d1, d2))
            {
                // handles if both are null as well as object identity
                return true;
            }

            if ((object)d1 == null || (object)d2 == null)
            {
                return false;
            }
            return d1.Equals(d2);
        }

        /// <summary>
        /// Inequality overload for two 3D direction cosines
        /// </summary>
        /// <param name="d1">The first direction cosine</param>
        /// <param name="d2">The second direction cosine</param>
        /// <returns>A Boolean indicating whether d1 does not equal d2</returns>
        public static bool operator !=(Direction d1, Direction d2)
        {
            return !(d1 == d2);
        }

        /// <summary>
        /// Instance member for equality comparison
        /// </summary>
        /// <param name="obj">The object of comparison</param>
        /// <returns>A Boolean indicating if object is equal to direction</returns>
        public override bool Equals(object obj)
        {
            if (obj is Direction)
            {
                var p = obj as Direction;
                return
                    Ux == p.Ux &&
                    Uy == p.Uy &&
                    Uz == p.Uz;
            }
            return false;
        }
        /// <summary>
        /// Method to determine dot product between two unit directionals.
        /// </summary>
        /// <param name="d1">The first direction</param>
        /// <param name="d2">The second direction</param>
        /// <returns>The dot product</returns>
        public static double GetDotProduct(Direction d1, Direction d2)
        {
            return (d1.Ux * d2.Ux +
                     d1.Uy * d2.Uy +
                     d1.Uz * d2.Uz);
        }
        /// <summary>
        /// Method to write binary version of unit directional
        /// </summary>
        /// <param name="bw">The binary writer</param>
        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Ux);
            bw.Write(Uy);
            bw.Write(Uz);
        }
        /// <summary>
        /// Method to read binary version of unit directional
        /// </summary>
        /// <param name="br">The binary reader</param>
        /// <returns>A Direction instance</returns>
        public static Direction ReadBinary(BinaryReader br)
        {
            return new Direction(
                br.ReadDouble(), // Ux
                br.ReadDouble(), // Uy
                br.ReadDouble()); // Uz
        }

        /// <summary>
        /// Property that returns unit directional along positive x-axis
        /// </summary>
        public static Direction AlongPositiveXAxis => new Direction(1.0, 0.0, 0.0);

        /// <summary>
        /// Property that returns unit directional along positive y-axis
        /// </summary>
        public static Direction AlongPositiveYAxis => new Direction(0.0, 1.0, 0.0);

        /// <summary>
        /// Property that returns unit directional along positive z-axis
        /// </summary>
        public static Direction AlongPositiveZAxis => new Direction(0.0, 0.0, 1.0);

        /// <summary>
        /// Property that returns unit directional along positive x-axis
        /// </summary>
        public static Direction AlongNegativeXAxis => new Direction(-1.0, 0.0, 0.0);

        /// <summary>
        /// Property that returns unit directional along positive y-axis
        /// </summary>
        public static Direction AlongNegativeYAxis => new Direction(0.0, -1.0, 0.0);

        /// <summary>
        /// Property that returns unit directional along positive z-axis
        /// </summary>
        public static Direction AlongNegativeZAxis => new Direction(0.0, 0.0, -1.0);

        /// <summary>
        /// Method to clone unit directional
        /// </summary>
        /// <returns>A direction</returns>
        public Direction Clone()
        {
            return new Direction(Ux, Uy, Uz);
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
