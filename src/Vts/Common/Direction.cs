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
    }
}
