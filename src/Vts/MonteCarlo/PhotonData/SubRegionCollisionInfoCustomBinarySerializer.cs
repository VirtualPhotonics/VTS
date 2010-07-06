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
        private bool _tallyMomentumTransfer;

        public SubRegionCollisionInfoCustomBinarySerializer(bool tallyMomentumTransfer)
        {
            _tallyMomentumTransfer = tallyMomentumTransfer;
        }

        public void WriteToBinary(BinaryWriter bw, SubRegionCollisionInfo info)
        {
            bw.Write(info.PathLength);
            bw.Write(info.NumberOfCollisions);

            if (_tallyMomentumTransfer)
            {
                bw.Write(info.MomentumTransfer);
            }
        }

        public SubRegionCollisionInfo ReadFromBinary(BinaryReader br)
        {
            if(_tallyMomentumTransfer)
            {
                return new SubRegionCollisionInfo(
                    br.ReadDouble(), // pathLength
                    br.ReadInt64(),  // numberOfCollisions
                    true,
                    br.ReadDouble()); 
            }
            else
            {
                return new SubRegionCollisionInfo(
                    br.ReadDouble(), // pathLength
                    br.ReadInt64());  // numberOfCollisions); 
            }
        }
    }
}
