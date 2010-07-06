using System.IO;

namespace Vts.Common
{
    public class Position
    {
        private double _X;
        private double _Y;
        private double _Z;

        public Position(double x, double y, double z)
        {
            _X = x;
            _Y = y;
            _Z = z;
        }

        public Position()
            : this(0.0, 0.0, 0.0)
        {
        }

        public double X { get { return _X; } set { _X = value; } }
        public double Y { get { return _Y; } set { _Y = value; } }
        public double Z { get { return _Z; } set { _Z = value; } }

        public static Position operator +(Position p1, Position p2)
        {
            return
                new Position(
                    p1.X + p2.X,
                    p1.Y + p2.Y,
                    p1.Z + p2.Z);
        }

        public static Position operator -(Position p1, Position p2)
        {
            return
                new Position(
                    p1.X - p2.X,
                    p1.Y - p2.Y,
                    p1.Z - p2.Z);
        }

        public static bool operator ==(Position p1, Position p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Position p1, Position p2)
        {
            return !p1.Equals(p2);
        }

        public static double GetDistance(Position p1, Position p2)
        {
            return System.Math.Sqrt(
                (p1.X - p2.X) * (p1.X - p2.X) +
                (p1.Y - p2.Y) * (p1.Y - p2.Y) +
                (p1.Z - p2.Z) * (p1.Z - p2.Z));
        }

        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        public static Position ReadBinary(BinaryReader br)
        {
            return new Position(
                br.ReadDouble(), // X
                br.ReadDouble(), // Y
                br.ReadDouble()); // Z
        }

        public bool Equals(Position p)
        {
            return
                this.X == p.X &&
                this.Y == p.Y &&
                this.Z == p.Z;
        }
    }
}
