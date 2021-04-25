using System;
using System.Runtime.InteropServices;
using Vts.Common;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// ZRDRay Data to be read in from Zemax ZRD file in uncompressed
    /// full Data format (UFD)
    /// </summary>
    public class ZRDRayDataPoint
    {        
        /// <summary>
        /// Defines ZRDRay Data class
        /// This Zemax structure size is 208 bytes for each ray segment
        public int count = 208;
        /// <summary>
        /// This constructor allows for easy instantiation from MCCL local
        /// class RayDataPoint.  RayDataPoint contains the data from ZRDRayDataPoint
        /// that is necessary to MCCL.
        /// </summary>
        /// <param name="rayDataPoint"></param>
        public ZRDRayDataPoint(RayDataPoint rayDataPoint)
        {
            X = rayDataPoint.Position.X;
            Y = rayDataPoint.Position.Y;
            Z = rayDataPoint.Position.Z;
            Ux = rayDataPoint.Direction.Ux;
            Uy = rayDataPoint.Direction.Uy;
            Uz = rayDataPoint.Direction.Uz;
            Weight = rayDataPoint.Weight;
        }

        public ZRDRayDataPoint() : this(new RayDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1), 1.0))
        { }

        //[StructLayout(LayoutKind.Explicit)]
        //public struct ZRDRayDataPoint
        //{
            //[FieldOffset(0)]
            public uint status;
            //[FieldOffset(4)]
            public int level;
            //[FieldOffset(8)]
            public int hitObject;
            //[FieldOffset(12)]
            public int hitFace;
            //[FieldOffset(16)]
            public int unused;
            //[FieldOffset(20)]
            public int inObject;
            //[FieldOffset(24)]
            public int parent;
            //[FieldOffset(28)]
            public int storage;
            //[FieldOffset(32)]
            public int xyBin;
            //[FieldOffset(36)]
            public int lmBin;
            //[FieldOffset(40)]
            public double index;
            //[FieldOffset(48)]
            public double startingPhase;
            //[FieldOffset(56)]
            public double X { get; set; }
            //[FieldOffset(64)]
            public double Y { get; set; }
            //[FieldOffset(72)]
            public double Z { get; set; }
            //[FieldOffset(80)]
            public double Ux { get; set; } // this is "l" in Zemax
            //[FieldOffset(88)]
            public double Uy { get; set; } // this is "m" in Zemax
            //[FieldOffset(96)]
            public double Uz { get; set; } // this is "n" in Zemax
            //[FieldOffset(104)]
            public double nx;
            //[FieldOffset(112)]
            public double ny;
            //[FieldOffset(120)]
            public double nz;
            //[FieldOffset(128)]
            public double pathTo;
            //[FieldOffset(136)]
            public double Weight { get; set; } // this is "intensity" in Zemax
            //[FieldOffset(144)]
            public double phaseOf;
            //[FieldOffset(152)]
            public double phaseAt;
            //[FieldOffset(160)]
            public double exr;
            //[FieldOffset(168)]
            public double exi;
            //[FieldOffset(176)]
            public double eyr;
            //[FieldOffset(184)]
            public double eyi;
            //[FieldOffset(192)]
            public double ezr;
            //[FieldOffset(200)]
            public double ezi;    

    }
}
