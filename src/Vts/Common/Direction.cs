using System.IO;

namespace Vts.Common
{
    public class Direction 
    {
        private double _Ux;
        private double _Uy;
        private double _Uz;
                      
        public Direction(double ux, double uy, double uz)
        {
            _Ux = ux;
            _Uy = uy;
            _Uz = uz;
        }

        public Direction() : this(0.0, 0.0, 1.0)
        {
        }

        public double Ux { get { return _Ux; } set { _Ux = value; } }
        public double Uy { get { return _Uy; } set { _Uy = value; } }
        public double Uz { get { return _Uz; } set { _Uz = value; } }

        /// <summary>
        /// Equality overload for two 3D direction cosines
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator ==(Direction d1, Direction d2)
        {
            return d1.Equals(d2);
        }

        /// <summary>
        /// Inequality overload for two 3D direction cosines
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator !=(Direction d1, Direction d2)
        {
            return !d1.Equals(d2);
        }

        /// <summary>
        /// Instance member for equality comparison
        /// </summary>
        /// <param name="obj"></param>
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
        public static double GetDotProduct(Direction d1, Direction d2)
        {
            return  (d1.Ux * d2.Ux +
                     d1.Uy * d2.Uy +
                     d1.Uz * d2.Uz);
        }

        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Ux);
            bw.Write(Uy);
            bw.Write(Uz);
        }

        public static Direction ReadBinary(BinaryReader br)
        {
            return new Direction(
                br.ReadDouble(), // Ux
                br.ReadDouble(), // Uy
                br.ReadDouble()); // Uz
        }

        public Direction Clone()
        {
            return new Direction(this.Ux, this.Uy, this.Uz);
        }
    }
}
