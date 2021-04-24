using System;
using System.Runtime.InteropServices;
using Vts.Common;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// ZRDRay Data to be read in from Zemax ZRD file in uncompressed
    /// full Data format (UFD)
    /// </summary>
    public class ZRDRayDataInUFD
    {
        /// <summary>
        /// Defines ZRDRay Data class
        /// This structure size is 208 bytes for each ray segment

        [StructLayout(LayoutKind.Explicit)]
        public struct ZRDRayDataPoint
        {
            [FieldOffset(0)]
            public uint status;
            [FieldOffset(4)]
            public int level;
            [FieldOffset(8)]
            public int hitObject;
            [FieldOffset(12)]
            public int hitFace;
            [FieldOffset(16)]
            public int unused;
            [FieldOffset(20)]
            public int inObject;
            [FieldOffset(24)]
            public int parent;
            [FieldOffset(28)]
            public int storage;
            [FieldOffset(32)]
            public int xyBin;
            [FieldOffset(36)]
            public int lmBin;
            [FieldOffset(40)]
            public double index;
            [FieldOffset(48)]
            public double startingPhase;
            [FieldOffset(56)]
            public double x;
            [FieldOffset(64)]
            public double y;
            [FieldOffset(72)]
            public double z;
            [FieldOffset(80)]
            public double l;
            [FieldOffset(88)]
            public double m;
            [FieldOffset(96)]
            public double n;
            [FieldOffset(104)]
            public double nx;
            [FieldOffset(112)]
            public double ny;
            [FieldOffset(120)]
            public double nz;
            [FieldOffset(128)]
            public double pathTo;
            [FieldOffset(136)]
            public double intensity;
            [FieldOffset(144)]
            public double phaseOf;
            [FieldOffset(152)]
            public double phaseAt;
            [FieldOffset(160)]
            public double exr;
            [FieldOffset(168)]
            public double exi;
            [FieldOffset(176)]
            public double eyr;
            [FieldOffset(184)]
            public double eyi;
            [FieldOffset(192)]
            public double ezr;
            [FieldOffset(200)]
            public double ezi;
        }

    }
}
