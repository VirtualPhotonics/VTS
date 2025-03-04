using System.IO;
using System.Numerics;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO
{
    class ArrayCustomBinaryWriterTests
    {
        /// <summary>
        /// Validate that a byte array can be written and read
        /// </summary>
        [Test]
        public void validate_write_read_byte_array_to_binary()
        {
            var array = new byte[5] { 1, 0, 0, 1, 0 };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[1] { 5 };
                        var byteArray = (byte[])new ArrayCustomBinaryReader<byte>(dims).ReadFromBinary(br);
                        Assert.That(byteArray[4], Is.EqualTo(array[4]));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a double array can be written and read
        /// </summary>
        [Test]
        public void validate_write_read_double_array_to_binary()
        {
            var array = new double[3] { 1.5, 6.2, 5.1 };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[1] { 3 };
                        var doubleArray = (double[])new ArrayCustomBinaryReader<double>(dims).ReadFromBinary(br);
                        Assert.That(doubleArray[0], Is.EqualTo(array[0]));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a float array can be written and read
        /// </summary>
        [Test]
        public void validate_write_read_float_array_to_binary()
        {
            var array = new float[3] { 1.4F, 0.5F, 6.2F };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[1] { 3 };
                        var floatArray = (float[])new ArrayCustomBinaryReader<float>(dims).ReadFromBinary(br);
                        Assert.That(floatArray[2], Is.EqualTo(array[2]));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a ushort array can be written and read
        /// </summary>
        [Test]
        public void validate_write_read_ushort_array_to_binary()
        {
            var array = new ushort[4] { 1, 2, 3, 4 };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[1] { 4 };
                        var ushortArray = (ushort[])new ArrayCustomBinaryReader<ushort>(dims).ReadFromBinary(br);
                        Assert.That(ushortArray[1], Is.EqualTo(array[1]));
                    }
                }
            }
        }

        /// <summary>
        /// Validate that a complex number array can be written and read
        /// </summary>
        [Test]
        public void validate_write_read_complex_array_to_binary()
        {
            var array = new Complex[2] { new Complex(4.0, 0.0), new Complex(1.3, 0.85) };
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, array);
                    bw.Flush();
                    s.Position = 0;
                    using (var br = new BinaryReader(s))
                    {
                        var dims = new int[1] { 2 };
                        var complexArray = (Complex[])new ArrayCustomBinaryReader<Complex>(dims).ReadFromBinary(br);
                        Assert.That(complexArray[0], Is.EqualTo(array[0]));
                    }
                }
            }
        }
    }
}
