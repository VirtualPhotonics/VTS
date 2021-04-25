using System.Collections.Generic;
using System.IO;
using Vts.IO;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning source Zemax ZRD ray data points 
    /// (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints 
    /// list of RayDataPoint items
    /// </summary>
    public class ZRDRayDatabase  : Database<ZRDRayDataPoint> 
    {
        /// <summary>
        /// Returns an instance of SourceDatabase
        /// </summary>
        public ZRDRayDatabase()
        {
        }

        public static ZRDRayDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<ZRDRayDatabase, ZRDRayDataPoint>(
                db => new ZRDRayDataPointSerializer());

            return dbReader.FromFile(fileName);
        }

        ///// <summary>
        ///// Static helper method to simplify reading from file
        ///// </summary>
        ///// <param name="fileName">The base filename for the database with .ZRD extension</param>
        ///// <param name="numberOfRays">number of rays to be read</param>
        ///// <returns>A new instance of SourceDatabase</returns>
        //public static List<RayDataPoint> FromFile(string fileName, int numberOfRays)
        //{
        //    RayDataPoints = new List<RayDataPoint>();
        //    // open stream
        //    using (Stream s = StreamFinder.GetFileStream(fileName, FileMode.Open))
        //    {
        //        using (BinaryReader br = new BinaryReader(s))
        //        {
        //            // read header information
        //            int version = br.ReadInt32();
        //            int maxSegments = br.ReadInt32();
        //            int count = 208;
        //            for (int i = 0; i < numberOfRays; i++)
        //            {
        //                int numSegments = br.ReadInt32();  // number of segments in ray_i
        //                // read in ZRDRayDataPoint struct
        //                var skipData = br.ReadBytes(56); // skip down to x,y,z,ux,uy,uz
        //                double X = br.ReadDouble();
        //                double Y = br.ReadDouble();
        //                double Z = br.ReadDouble();
        //                double Ux = br.ReadDouble();
        //                double Uy = br.ReadDouble();
        //                double Uz = br.ReadDouble();
        //                skipData = br.ReadBytes(32); // skip to intensity
        //                double Weight = br.ReadDouble();
        //                skipData = br.ReadBytes(64); // skip to end of ZRDRayDataPoint
        //                // skip rest of rays in segment
        //                skipData = br.ReadBytes((numSegments - 1) * count);
        //                RayDataPoints.Add(new RayDataPoint(
        //                    new Position(X, Y, Z), new Direction(Ux, Uy, Uz), Weight));
        //            }
        //        }
        //    }
        //    return RayDataPoints;
        //}

        /// <summary>
        /// Static helper method to simplify writing to file
        /// </summary>
        /// <param name="fileName">The base filename with extension .ZRD</param>
        /// <returns>none</returns>
        //public static void ToFile(string fileName, IList<RayDataPoint> rayDataPoints)
        //{
            //var rayDP = new ZRDRayDataPoint();
            //using (Stream s = StreamFinder.GetFileStream(fileName, FileMode.Create))
            //{
            //    using (BinaryWriter bw = new BinaryWriter(s))
            //    {
            //        // write header information
            //        int version = 0;
            //        bw.Write(version);
            //        int maxSegments = 1;
            //        bw.Write(maxSegments);
            //        foreach (var ray in rayDataPoints)
            //        {
            //            int numSegments = 1;  // number of segments in ray_i
            //            bw.Write(numSegments);
            //            // rest of these until x,y,z set to 0
            //            rayDP.status = 0;
            //            bw.Write(rayDP.status);
            //            rayDP.level = 0;
            //            bw.Write(rayDP.level);
            //            rayDP.hitObject = 0;
            //            bw.Write(rayDP.hitObject);
            //            rayDP.hitFace = 0;
            //            bw.Write(rayDP.hitFace);
            //            rayDP.unused = 0;
            //            bw.Write(rayDP.unused);
            //            rayDP.inObject = 0;
            //            bw.Write(rayDP.inObject);
            //            rayDP.parent = 0;
            //            bw.Write(rayDP.parent);
            //            rayDP.storage = 0;
            //            bw.Write(rayDP.storage);
            //            rayDP.xyBin = 0;
            //            bw.Write(rayDP.xyBin);
            //            rayDP.lmBin = 0;
            //            bw.Write(rayDP.lmBin);
            //            rayDP.index = 0;
            //            bw.Write(rayDP.index);
            //            rayDP.startingPhase = 0;
            //            bw.Write(rayDP.startingPhase);
            //            // write data from rayDataPointsList
            //            bw.Write(ray.Position.X);
            //            bw.Write(ray.Position.Y);
            //            bw.Write(ray.Position.Z); 
            //            bw.Write(ray.Direction.Ux);
            //            bw.Write(ray.Direction.Uy);
            //            bw.Write(ray.Direction.Uz);
            //            rayDP.nx = 0.0;
            //            bw.Write(rayDP.nx);
            //            rayDP.ny = 0.0;
            //            bw.Write(rayDP.ny);
            //            rayDP.nz = 0.0;
            //            bw.Write(rayDP.nz);
            //            rayDP.pathTo = 0.0;
            //            bw.Write(rayDP.pathTo);
            //            // write weight into intensity field
            //            bw.Write(ray.Weight);
            //            rayDP.phaseOf = 0.0;
            //            bw.Write(rayDP.phaseOf);
            //            rayDP.phaseAt = 0.0;
            //            bw.Write(rayDP.phaseAt);
            //            rayDP.exr = 0.0;
            //            bw.Write(rayDP.exr);
            //            rayDP.exi = 0.0;
            //            bw.Write(rayDP.exi);
            //            rayDP.eyr = 0.0;
            //            bw.Write(rayDP.eyr); 
            //            rayDP.eyi = 0.0;
            //            bw.Write(rayDP.eyi); 
            //            rayDP.ezr = 0.0;
            //            bw.Write(rayDP.ezr); 
            //            rayDP.ezi = 0.0;
            //            bw.Write(rayDP.ezi);
            //        }
            //    }
            //}
        //}
    }
}
