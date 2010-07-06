using System.IO;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements ICustomBinaryReader<PhotonDataPoint> and 
    /// ICustomBinaryWriter<PhotonDataPoint>.
    /// </summary>
    public class PhotonTerminationDataPointCustomBinarySerializer : 
        ICustomBinaryReader<PhotonDataPoint>, 
        ICustomBinaryWriter<PhotonDataPoint>
    {
        private long _numberOfSubRegions;
        private ICustomBinaryReader<SubRegionCollisionInfo> _subRegionInfoReader;
        private ICustomBinaryWriter<SubRegionCollisionInfo> _subRegionInfoWriter;
        private bool _tallyMomentumTransfer;

        public PhotonTerminationDataPointCustomBinarySerializer(long numberOfSubRegions, bool tallyMomentumTransfer)
        {
            _tallyMomentumTransfer = tallyMomentumTransfer;
            _numberOfSubRegions = numberOfSubRegions;
            var serializer = new SubRegionCollisionInfoCustomBinarySerializer(_tallyMomentumTransfer);
            _subRegionInfoWriter = serializer;
            _subRegionInfoReader = serializer;
        }

        public void WriteToBinary(BinaryWriter bw, PhotonDataPoint item)
        {
            item.Position.WriteBinary(bw);
            item.Direction.WriteBinary(bw);
            bw.Write(item.Weight);
            bw.Write((byte)item.StateFlag);

            for (int i = 0; i < item.SubRegionInfoList.Count; i++)
            {
                _subRegionInfoWriter.WriteToBinary(bw, item.SubRegionInfoList[i]);
            }
        }

        public PhotonDataPoint ReadFromBinary(BinaryReader br)
        {
            var dataPoint = new PhotonDataPoint(
                Position.ReadBinary(br),
                Direction.ReadBinary(br),
                br.ReadDouble(), // Weight
                (PhotonStateType)br.ReadByte(),
                new SubRegionCollisionInfo[_numberOfSubRegions]);

            for (int i = 0; i < _numberOfSubRegions; i++)
            {
                dataPoint.SubRegionInfoList[i] = _subRegionInfoReader.ReadFromBinary(br);
            }

            return dataPoint;
        }
    }
}
