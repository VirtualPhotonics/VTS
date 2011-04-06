using System.IO;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements ICustomBinaryReader<CollisionInfo> and 
    /// ICustomBinaryWriter<CollisionInfo>.
    /// </summary>
    public class CollisionInfoSerializer :
        ICustomBinaryReader<CollisionInfo>,
        ICustomBinaryWriter<CollisionInfo>
    {
        private int _numberofSubRegions;

        public CollisionInfoSerializer(int numberOfSubRegions)
        {
            _numberofSubRegions = numberOfSubRegions;
        }

        public void WriteToBinary(BinaryWriter bw, CollisionInfo item)
        {
            for (int i = 0; i < item.Count; i++)
            {
                bw.Write(item[i].PathLength);
                bw.Write(item[i].NumberOfCollisions);
            }
        }

        public CollisionInfo ReadFromBinary(BinaryReader br)
        {
            var collisionInfo = new CollisionInfo(_numberofSubRegions);

            for (int i = 0; i < collisionInfo.Count; i++)
            {
                collisionInfo[i] = new SubRegionCollisionInfo(
                    br.ReadDouble(), // pathLength
                    br.ReadInt64());  // numberOfCollisions); 
            }

            return collisionInfo;
        }
    }
}
