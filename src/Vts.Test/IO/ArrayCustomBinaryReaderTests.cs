using System.IO;
using System.Numerics;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO
{
    class ArrayCustomBinaryReaderTests
    {
        /// <summary>
        /// Validate that a multi-dimensional double array can be read
        /// </summary>
        [Test]
        public void validate_read_multi_double_array_to_binary()
        {
            var array = new double[3,2] { { 1.5, 6.2 }, {5.1, 6.7 }, { 4.0, 3.2 } };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[2] { 3, 2 };
                        var doubleArray = (double[,])new ArrayCustomBinaryReader<double>(dims).ReadFromBinary(br);
                        Assert.That(doubleArray[0, 1], Is.EqualTo(array[0,1]));
                        Assert.That(doubleArray[0, 1], Is.EqualTo(6.2));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a multi-dimensional ushort array can be read
        /// </summary>
        [Test]
        public void validate_read_multi_ushort_array_to_binary()
        {
            var array = new ushort[3, 2, 2] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[3] { 3, 2, 2 };
                        var ushortArray = (ushort[,,])new ArrayCustomBinaryReader<ushort>(dims).ReadFromBinary(br);
                        Assert.That(ushortArray[0, 1, 0], Is.EqualTo(array[0, 1, 0]));
                        Assert.That(ushortArray[2, 1, 0], Is.EqualTo(11));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a multi-dimensional byte array can be read
        /// </summary>
        [Test]
        public void validate_read_multi_byte_array_from_binary()
        {
            var array = new byte[2,2] { { 1, 0 }, { 0, 1 } };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[2] { 2, 2 };
                        var byteArray = (byte[,])new ArrayCustomBinaryReader<byte>(dims).ReadFromBinary(br);
                        Assert.That(byteArray[1,0], Is.EqualTo(array[1,0]));
                        Assert.That(byteArray[1,1], Is.EqualTo(1));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a multi-dimensional complex array can be read
        /// </summary>
        [Test]
        public void validate_read_multi_complex_array_from_binary()
        {
            var array = new Complex[2, 2] { { new Complex(5.0, 2.1), new Complex(4.1, 3.2) }, { new Complex(1.1, 9.0), new Complex(3.3, 6.1) } };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[2] { 2, 2 };
                        var complexArray = (Complex[,])new ArrayCustomBinaryReader<Complex>(dims).ReadFromBinary(br);
                        Assert.That(complexArray[0, 0], Is.EqualTo(array[0, 0]));
                        Assert.That(new Complex(1.1, 9.0), Is.EqualTo(complexArray[1, 0]));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a multi-dimensional float array can be read
        /// </summary>
        [Test]
        public void validate_read_multi_float_array_to_binary()
        {
            var array = new float[2, 3] { { 1.4F, 0.5F, 6.2F }, { 1.03F, 0.85F, 5.1F } };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[2] { 2, 3 };
                        var floatArray = (float[,])new ArrayCustomBinaryReader<float>(dims).ReadFromBinary(br);
                        Assert.That(floatArray[0, 2], Is.EqualTo(array[0, 2]));
                        Assert.That(floatArray[1, 1], Is.EqualTo(0.85F));
                    }
                }
            }
        }
    }
}
