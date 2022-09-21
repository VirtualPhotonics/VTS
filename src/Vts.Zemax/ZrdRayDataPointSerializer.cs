using System.IO;
using Vts.IO;

namespace Vts.Zemax
{
    /// <summary>
    /// This is code that would be executed to convert Zrd DB to/from MCCL compatible DB
    /// Implements ICustomBinaryReader&lt;RayDataPoint&gt; and 
    /// ICustomBinaryWriter&lt;ZrdDataPoint&gt;.
    /// </summary>
    public class ZrdRayDataPointSerializer : 
        ICustomBinaryReader<ZrdRayDataPoint>, 
        ICustomBinaryWriter<ZrdRayDataPoint>
    {
        private int count = 208;
        private static bool headerIsWritten = false;
        private static bool headerIsRead = false;
        /// <summary>
        /// Method to write ZrdDataPoint to binary. Header is written only first time through.
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">ZrdDataPoint</param>
        public void WriteToBinary(BinaryWriter bw, ZrdRayDataPoint item)
        {
            if (!headerIsWritten)
            {
                int version = 2002;
                bw.Write(version);
                int maxNumberOfSegments = 500;
                bw.Write(maxNumberOfSegments);
                headerIsWritten = true;
            }
            int numSegments = 2;  // number of segments in ray_i
            bw.Write(numSegments);
            for (int i = 0; i < numSegments; i++) // write same rayDP twice to make ray
            {
                var rayDP = new ZrdRayDataPoint();
                // rest of these until x,y,z set to 0
                rayDP.status = 0;
                bw.Write(rayDP.status);
                bw.Write(i); // level: should match segment#
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
        }
        /// <summary>
        /// Method to read PhotonDataPoint from binary.  Header info is only read first time through.
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>ZrdRayDataPoint, data comsistent with zrd file ray</returns>
        public ZrdRayDataPoint ReadFromBinary(BinaryReader br)
        {
            if (!headerIsRead)
            {
                br.ReadInt32();
                br.ReadInt32();
                headerIsRead = true;
            }
            int numSegments = br.ReadInt32();  // number of segments in this ray
            // skip until last  segment
            br.ReadBytes((numSegments - 1) * count);
            br.ReadBytes(56); // skip down to x,y,z,ux,uy,uz            
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double z = br.ReadDouble();
            double ux = br.ReadDouble();
            double uy = br.ReadDouble();
            double uz = br.ReadDouble();
            br.ReadBytes(32); // skip to intensity
            double weight = br.ReadDouble();
            br.ReadBytes(64); // skip to end of ZrdRayDataPoint
            return new ZrdRayDataPoint()
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
