using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Vts.IO;
using Vts.Common;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning source Zemax ZRD ray data points 
    /// (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints 
    /// list of RayDataPoint items
    /// </summary>
    public class ZRDRayDatabase  : Database<RayDataPoint> // CKH: do I need to inherit here?
    {
        /// <summary>
        /// Returns an instance of SourceDatabase
        /// </summary>
        public ZRDRayDatabase()
        {
        }
        public static List<RayDataPoint> RayDataPoints { get; private set; }
        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of SourceDatabase</returns>
        public static List<RayDataPoint> FromFile(string fileName)
        {
            RayDataPoints = new List<RayDataPoint>();
            // open stream
            using (Stream s = StreamFinder.GetFileStream(fileName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(s))
                {
                    // read header information
                    int version = br.ReadInt32();
                    int maxSegments = br.ReadInt32();
                    int count = Marshal.SizeOf(typeof(ZRDRayDataInUFD.ZRDRayDataPoint));
                    for (int i = 0; i < 1000; i++)
                    {
                        int numSegments = br.ReadInt32();  // number of segments 
                        var skipData = br.ReadBytes(56);
                        double X = br.ReadDouble();
                        double Y = br.ReadDouble();
                        double Z = br.ReadDouble();
                        double Ux = br.ReadDouble();
                        double Uy = br.ReadDouble();
                        double Uz = br.ReadDouble();
                        skipData = br.ReadBytes(104); // skip to end of ZRDRayDataPoint
                        // skip rest of rays in segment
                        skipData = br.ReadBytes((numSegments - 1) * count);
                        //var rayData = br.ReadBytes(count);
                        //byte[] readBuffer = new byte[count];
                        //GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                        //var zrdRayData = (ZRDRayDataInUFD.ZRDRayDataPoint)
                        //    Marshal.PtrToStructure(handle.AddrOfPinnedObject(), 
                        //    typeof(ZRDRayDataInUFD.ZRDRayDataPoint));
                        RayDataPoints.Add(new RayDataPoint(
                            new Position(X, Y, Z), new Direction(Ux, Uy, Uz), 1));
                    }
                }
            }
            return RayDataPoints;
        }
        /// <summary>
        /// Static helper method to simplify writing to file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of SourceDatabase</returns>
        public static void ToFile(string fileName)
        {
            // to be coded
        }
    }
}
