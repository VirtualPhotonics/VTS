using System.IO;

namespace Vts.Common
{
    /// <summary>
    /// Class describes unit directional vector.
    /// </summary>
    public class Direction
    {
        private readonly int _hashCode;

        /// <summary>
        /// constructor for direction, passes x, y and z values
        /// </summary>
        /// <param name="ux">x direction</param>
        /// <param name="uy">y direction</param>
        /// <param name="uz">z direction</param>
        public Direction(double ux, double uy, double uz)
        {
            Ux = ux;
            Uy = uy;
            Uz = uz;
            _hashCode = new {ux, uy, uz}.GetHashCode();
        }

        /// <summary>
        /// default constructor for direction (0,0,1)
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
        /// <param name="d1">first direction cosine</param>
        /// <param name="d2">second direction cosine</param>
        /// <returns></returns>
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
        /// <param name="d1">first direction cosine</param>
        /// <param name="d2">second direction cosine</param>
        /// <returns></returns>
        public static bool operator !=(Direction d1, Direction d2)
        {
            return !(d1 == d2);
        }

        /// <summary>
        /// Instance member for equality comparison
        /// </summary>
        /// <param name="obj">object of comparision</param>
        /// <returns></returns>
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
        /// <param name="d1">first direction</param>
        /// <param name="d2">second direction</param>
        /// <returns>the dot product</returns>
        public static double GetDotProduct(Direction d1, Direction d2)
        {
            return (d1.Ux * d2.Ux +
                     d1.Uy * d2.Uy +
                     d1.Uz * d2.Uz);
        }
        /// <summary>
        /// Method to write binary version of unit directional
        /// </summary>
        /// <param name="bw"></param>
        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Ux);
            bw.Write(Uy);
            bw.Write(Uz);
        }
        /// <summary>
        /// Method to read binary version of unit directional
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>a direction</returns>
        public static Direction ReadBinary(BinaryReader br)
        {
            return new Direction(
                br.ReadDouble(), // Ux
                br.ReadDouble(), // Uy
                br.ReadDouble()); // Uz
        }
        /// <summary>
        /// Method that returns unit directional along positive x-axis
        /// </summary>
        public static Direction AlongPositiveXAxis => new Direction(1.0, 0.0, 0.0);

        /// <summary>
        /// Method that returns unit directional along positive y-axis
        /// </summary>
        public static Direction AlongPositiveYAxis => new Direction(0.0, 1.0, 0.0);

        /// <summary>
        /// Method that returns unit directional along positive z-axis
        /// </summary>
        public static Direction AlongPositiveZAxis => new Direction(0.0, 0.0, 1.0);

        /// <summary>
        /// Method that returns unit directional along positive x-axis
        /// </summary>
        public static Direction AlongNegativeXAxis => new Direction(-1.0, 0.0, 0.0);

        /// <summary>
        /// Method that returns unit directional along positive y-axis
        /// </summary>
        public static Direction AlongNegativeYAxis => new Direction(0.0, -1.0, 0.0);

        /// <summary>
        /// Method that returns unit directional along positive z-axis
        /// </summary>
        public static Direction AlongNegativeZAxis => new Direction(0.0, 0.0, -1.0);

        /// <summary>
        /// Method to clone unit directional
        /// </summary>
        /// <returns>a direction</returns>
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
