using System.IO;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements ICustomBinaryReader<SubRegionCollisionInfo> and
    /// ICustomBinaryWriter<SubRegionCollisionInfo>.  Handles the serialization
    /// of SubRegionCollisionInfo to and from binary file.
    /// </summary>
    public class SubRegionCollisionInfoCustomBinarySerializer : 
        ICustomBinaryReader<SubRegionCollisionInfo>,
        ICustomBinaryWriter<SubRegionCollisionInfo>
    {
        public void WriteToBinary(BinaryWriter bw, SubRegionCollisionInfo info)
        {
            bw.Write(info.PathLength);
            bw.Write(info.NumberOfCollisions);
        }

        public SubRegionCollisionInfo ReadFromBinary(BinaryReader br)
        {
            return new SubRegionCollisionInfo(
                br.ReadDouble(), // pathLength
                br.ReadInt64());  // numberOfCollisions); 
        }
    }
}
