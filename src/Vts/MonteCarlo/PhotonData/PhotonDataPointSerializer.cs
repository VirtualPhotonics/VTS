using System.IO;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements ICustomBinaryReader&lt;PhotonDataPoint&gt; and 
    /// ICustomBinaryWriter&lt;PhotonDataPoint&gt;.
    /// </summary>
    public class PhotonDataPointSerializer : 
        ICustomBinaryReader<PhotonDataPoint>, 
        ICustomBinaryWriter<PhotonDataPoint>
    {
        public void WriteToBinary(BinaryWriter bw, PhotonDataPoint item)
        {
            item.Position.WriteBinary(bw);
            item.Direction.WriteBinary(bw);
            bw.Write(item.Weight);
            bw.Write(item.TotalTime);
            bw.Write((byte)item.StateFlag);
        }

        public PhotonDataPoint ReadFromBinary(BinaryReader br)
        {
            var dataPoint = new PhotonDataPoint(
                Position.ReadBinary(br), // Position
                Direction.ReadBinary(br), // Direction
                br.ReadDouble(), // Weight
                br.ReadDouble(), // TotalTime
                (PhotonStateType)br.ReadByte()); // PhotonStateType

            return dataPoint;
        }
    }
}
