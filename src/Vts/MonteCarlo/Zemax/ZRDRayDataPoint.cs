using Vts.Common;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Zemax
{
    /// <summary>
    /// ZRDRay Data to be read in from Zemax ZRD file in uncompressed
    /// full Data format (UFD)
    /// </summary>
    public class ZRDRayDataPoint
    {
        /// <summary>
        /// total number of bytes in a ZRD Ray data point
        /// </summary>
        public int count = 208;

        /// <summary>
        /// Defines ZRDRay Data class.
        /// This Zemax structure size is 208 bytes for each ray segment
        /// This constructor allows for easy instantiation from MCCL local
        /// class RayDataPoint.  RayDataPoint contains the data from ZRDRayDataPoint
        /// that is necessary to MCCL.
        /// </summary>
        /// <param name="rayDataPoint">ray data point</param>
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
        /// <summary>
        /// default constructor
        /// </summary>
        public ZRDRayDataPoint() : this(new RayDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1), 1.0))
        { }

        //[StructLayout(LayoutKind.Explicit)]
        //public struct ZRDRayDataPoint
        //{
            /// <summary>
            /// [FieldOffset(0)] status : bitwise flags indicating status of the ray
            /// </summary>
            public uint status;
            /// <summary>
            /// [FieldOffset(4)] level : number of ray segments bw ray segment and original source
            /// </summary>
            public int level;
            /// <summary>
            /// [FieldOffset(8)] hitObject: the object number the ray intercepted (0=hit nothing)
            /// </summary>
            public int hitObject;
            /// <summary>
            /// [FieldOffset(12)] hitFace: the face number the ray intercepted (valid if hitObject!=0)
            /// </summary>
            public int hitFace;
            /// <summary>
            /// [FieldOffset(16)] unused
            /// </summary>
            public int unused;
            /// <summary>
            /// [FieldOffset(20)] inObject: object number of ray is propating inside of
            /// </summary>
            public int inObject;
        /// <summary>
        /// [FieldOffset(24)] parent: the prior ray segment from which the ray originated
        /// </summary>
        public int parent;
        /// <summary>
        /// [FieldOffset(28)] storage: temp buffer used by OpticStudio for buffering
        /// </summary>
        public int storage;
        /// <summary>
        /// [FieldOffset(32)] xyBin: pixel number on a detector object which the ray struck *spatial*
        /// </summary>
        public int xyBin;
        /// <summary>
        /// [FieldOffset(36)] lmBin: pizel number on a detector object which the ray struck *angular*
        /// </summary>
        public int lmBin;
        /// <summary>
        /// [FieldOffset(40)] index: index of refraction of the media
        /// </summary>
        public double index;
        /// <summary>
        /// [FieldOffset(48)] startingPhase: initial optical path length the ray starts with
        /// </summary>
        public double startingPhase;
        /// <summary>
        /// [FieldOffset(56)] x coordinate of ray position
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// [FieldOffset(64)] y coordinate of ray position
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// [FieldOffset(72)] z coordinate of ray position
        /// </summary>
        public double Z { get; set; }
        /// <summary>
        /// [FieldOffset(80)] direction Ux
        /// </summary>
        public double Ux { get; set; } // this is "l" in Zemax
        /// <summary>
        /// [FieldOffset(88)]direction Uy
        /// </summary>
        
        public double Uy { get; set; } // this is "m" in Zemax
        /// <summary>
        /// [FieldOffset(96)]direction Uz
        /// </summary>
        public double Uz { get; set; } // this is "n" in Zemax
        /// <summary>
        /// [FieldOffset(104)] nx: global normal vector of the object at the intercept point
        /// for segment 0 the nx values stores the wavelength of the ray being launched
        /// </summary>
        public double nx;
        /// <summary>
        /// [FieldOffset(112)] ny: global normal vector of the object at the intercept point
        /// </summary>
        public double ny;
        /// <summary>
        /// [FieldOffset(120)] nz: global normal vector of the object at the intercept point
        /// </summary>
        public double nz;
        /// <summary>
        /// [FieldOffset(128)] pathTo: the physical (not optical) path length of the ray segment
        /// </summary>
        public double pathTo;
        /// <summary>
        /// [FieldOffset(136)] Weight: "intensity" in Zemax
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// FieldOffset(144)] phaseOf: the pahse of the obect
        /// </summary>
        public double phaseOf;
        /// <summary>
        /// [FieldOffset(152)] phaseAt: the accumulated total phase of the ray (modulo 2pi)
        /// </summary>
        public double phaseAt;
        /// <summary>
        /// [FieldOffset(160)] exr: electric field in global x,y,z coordinates, real 
        /// </summary>
        public double exr;
        /// <summary>
        /// [FieldOffset(168)] exi: electric field in global x,y,z coordinates, imaginary
        /// </summary>
        public double exi;
        /// <summary>
        /// [FieldOffset(176)] eyr: electric field in global x,y,z coordinates, real 
        /// </summary>
        public double eyr;
        /// <summary>
        /// [FieldOffset(184)] eyi: electric field in global x,y,z coordinates, imaginary
        /// </summary>
        public double eyi;
        /// <summary>
        /// [FieldOffset(192)] ezr: electric field in global x,y,z coordinates, real 
        /// </summary>
        public double ezr;
        /// <summary>
        /// [FieldOffset(200)] ezi: electric field in global x,y,z coordinates, imaginary
        /// </summary>
        public double ezi;    

    }
}
