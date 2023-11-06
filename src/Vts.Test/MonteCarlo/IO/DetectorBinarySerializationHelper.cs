using System;
using System.IO;

namespace Vts.MonteCarlo.Detectors
{
    internal static class DetectorBinarySerializationHelper
    {
        /// <summary>
        /// detector input abstract class
        /// </summary>
        internal static void WriteClearAndReReadArrays(IDetector detector, params Array[] arrays)
        {
            var serializers = detector.GetBinarySerializers();
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            foreach (var serializer in serializers)
            {
                serializer.WriteData(writer);
            }

            foreach (var array in arrays)
            {
                Array.Clear(array);
            }

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new BinaryReader(stream);
            foreach (var serializer in serializers)
            {
                serializer.ReadData(reader);
            }
        }
    }
}