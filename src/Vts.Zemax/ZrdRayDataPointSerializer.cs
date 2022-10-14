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
        private readonly int _count = 208;
        private bool _headerIsWritten = false;
        private bool _headerIsRead = false;
        /// <summary>
        /// Method to write ZrdDataPoint to binary. Header is written only first time through.
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">ZrdDataPoint</param>
        public void WriteToBinary(BinaryWriter bw, ZrdRayDataPoint item)
        {
            if (!_headerIsWritten)
            {
                const int version = 2002;
                bw.Write(version);
                const int maxNumberOfSegments = 500;
                bw.Write(maxNumberOfSegments);
                _headerIsWritten = true;
            }
            int numSegments = 2;  // number of segments in ray_i
            bw.Write(numSegments);
            for (var i = 0; i < numSegments; i++) // write same rayDP twice to make ray
            {
                var rayDp = new ZrdRayDataPoint
                {
                    // rest of fields until x,y,z set to 0
                    Status = 0
                };
                bw.Write(rayDp.Status);
                bw.Write(i); // level: should match segment#
                rayDp.HitObject = 0;
                bw.Write(rayDp.HitObject);
                rayDp.HitFace = 0;
                bw.Write(rayDp.HitFace);
                rayDp.Unused = 0;
                bw.Write(rayDp.Unused);
                rayDp.InObject = 0;
                bw.Write(rayDp.InObject);
                rayDp.Parent = 0;
                bw.Write(rayDp.Parent);
                rayDp.Storage = 0;
                bw.Write(rayDp.Storage);
                rayDp.XyBin = 0;
                bw.Write(rayDp.XyBin);
                rayDp.LmBin = 0;
                bw.Write(rayDp.LmBin);
                rayDp.Index = 0;
                bw.Write(rayDp.Index);
                rayDp.StartingPhase = 0;
                bw.Write(rayDp.StartingPhase);
                // write data from rayDataPointsList
                bw.Write(item.X);
                bw.Write(item.Y);
                bw.Write(item.Z);
                bw.Write(item.Ux);
                bw.Write(item.Uy);
                bw.Write(item.Uz);
                rayDp.Nx = 0.0;
                bw.Write(rayDp.Nx);
                rayDp.Ny = 0.0;
                bw.Write(rayDp.Ny);
                rayDp.Nz = 0.0;
                bw.Write(rayDp.Nz);
                rayDp.PathTo = 0.0;
                bw.Write(rayDp.PathTo);
                // write weight into intensity field
                bw.Write(item.Weight);
                rayDp.PhaseOf = 0.0;
                bw.Write(rayDp.PhaseOf);
                rayDp.PhaseAt = 0.0;
                bw.Write(rayDp.PhaseAt);
                rayDp.Exr = 0.0;
                bw.Write(rayDp.Exr);
                rayDp.Exi = 0.0;
                bw.Write(rayDp.Exi);
                rayDp.Eyr = 0.0;
                bw.Write(rayDp.Eyr);
                rayDp.Eyi = 0.0;
                bw.Write(rayDp.Eyi);
                rayDp.Ezr = 0.0;
                bw.Write(rayDp.Ezr);
                rayDp.Ezi = 0.0;
                bw.Write(rayDp.Ezi);
            }
        }
        /// <summary>
        /// Method to read PhotonDataPoint from binary.  Header info is only read first time through.
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>ZrdRayDataPoint, data consistent with zrd file ray</returns>
        public ZrdRayDataPoint ReadFromBinary(BinaryReader br)
        {
            if (!_headerIsRead)
            {
                br.ReadInt32();
                br.ReadInt32();
                _headerIsRead = true;
            }
            var numSegments = br.ReadInt32();  // number of segments in this ray
            // skip until last  segment
            br.ReadBytes((numSegments - 1) * _count);
            br.ReadBytes(56); // skip down to x,y,z,ux,uy,uz            
            var x = br.ReadDouble(); 
            var y = br.ReadDouble();
            var z = br.ReadDouble();
            var ux = br.ReadDouble();
            var uy = br.ReadDouble();
            var uz = br.ReadDouble();
            br.ReadBytes(32); // skip to intensity
            var weight = br.ReadDouble();
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
