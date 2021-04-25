using System.IO;
using System.Runtime.InteropServices;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements ICustomBinaryReader&lt;RayDataPoint&gt; and 
    /// ICustomBinaryWriter&lt;ZRDDataPoint&gt;.
    /// </summary>
    public class ZRDRayDataPointSerializer : 
        ICustomBinaryReader<ZRDRayDataPoint>, 
        ICustomBinaryWriter<ZRDRayDataPoint>
    {
        //            // read header information
        //            int version = br.ReadInt32();
        //            int maxSegments = br.ReadInt32();
        int count = 208;
        /// <summary>
        /// method to write ZRDDataPoint to binary HOW TO WRITE HEADER?
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">ZRDDataPoint</param>
        public void WriteToBinary(BinaryWriter bw, ZRDRayDataPoint item)
        {
            var rayDP = new ZRDRayDataPoint();
            int numSegments = 1;  // number of segments in ray_i
            bw.Write(numSegments);
            // rest of these until x,y,z set to 0
            rayDP.status = 0;
            bw.Write(rayDP.status);
            rayDP.level = 0;
            bw.Write(rayDP.level);
            rayDP.hitObject = 0;
            bw.Write(rayDP.hitObject);
            rayDP.hitFace = 0;
            bw.Write(rayDP.hitFace);
            rayDP.unused = 0;
            bw.Write(rayDP.unused);
            rayDP.inObject = 0;
            bw.Write(rayDP.inObject);
            rayDP.parent = 0;
            bw.Write(rayDP.parent);
            rayDP.storage = 0;
            bw.Write(rayDP.storage);
            rayDP.xyBin = 0;
            bw.Write(rayDP.xyBin);
            rayDP.lmBin = 0;
            bw.Write(rayDP.lmBin);
            rayDP.index = 0;
            bw.Write(rayDP.index);
            rayDP.startingPhase = 0;
            bw.Write(rayDP.startingPhase);
            // write data from rayDataPointsList
            bw.Write(item.X);
            bw.Write(item.Y);
            bw.Write(item.Z);
            bw.Write(item.Ux);
            bw.Write(item.Uy);
            bw.Write(item.Uz);
            rayDP.nx = 0.0;
            bw.Write(rayDP.nx);
            rayDP.ny = 0.0;
            bw.Write(rayDP.ny);
            rayDP.nz = 0.0;
            bw.Write(rayDP.nz);
            rayDP.pathTo = 0.0;
            bw.Write(rayDP.pathTo);
            // write weight into intensity field
            bw.Write(item.Weight);
            rayDP.phaseOf = 0.0;
            bw.Write(rayDP.phaseOf);
            rayDP.phaseAt = 0.0;
            bw.Write(rayDP.phaseAt);
            rayDP.exr = 0.0;
            bw.Write(rayDP.exr);
            rayDP.exi = 0.0;
            bw.Write(rayDP.exi);
            rayDP.eyr = 0.0;
            bw.Write(rayDP.eyr);
            rayDP.eyi = 0.0;
            bw.Write(rayDP.eyi);
            rayDP.ezr = 0.0;
            bw.Write(rayDP.ezr);
            rayDP.ezi = 0.0;
            bw.Write(rayDP.ezi);
        }
        /// <summary>
        /// method to read PhotonDataPoint from binary.  HOW TO READ HEADER?
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>RayDataPoint</returns>
        public ZRDRayDataPoint ReadFromBinary(BinaryReader br)
        {
            int numSegments = br.ReadInt32();  // number of segments in this ray
            var skipData = br.ReadBytes(56); // skip down to x,y,z,ux,uy,uz            
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double z = br.ReadDouble();
            double ux = br.ReadDouble();
            double uy = br.ReadDouble();
            double uz = br.ReadDouble();
            skipData = br.ReadBytes(32); // skip to intensity
            double weight = br.ReadDouble();
            skipData = br.ReadBytes(64); // skip to end of ZRDRayDataPoint
                                         // skip rest of rays in segment
            skipData = br.ReadBytes((numSegments - 1) * count);
            return new ZRDRayDataPoint()
            {
                X = x,
                Y = y,
                Z = z,
                Ux = ux,
                Uy = uy,
                Uz = uz,
                Weight = weight
            };
        }
    }
}
