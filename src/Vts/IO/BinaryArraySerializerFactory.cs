using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Vts.IO;

/// <summary>
/// Factory class to get the correct serializer for the binary array
/// </summary>
public static class BinaryArraySerializerFactory
{
    /// <summary>
    /// Factory method that writes an array item to a binary file
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="item">The data array</param>
    /// <param name="name">The name of the array</param>
    /// <param name="fileTag">The file tag to append to the filename</param>
    /// <returns>An instance of BinaryArraySerializer</returns>
    public static BinaryArraySerializer GetSerializer<T>(T item, string name, string fileTag)
    {
        return new BinaryArraySerializer
        {
            Name = name,
            FileTag = fileTag,
            WriteData = GetWriteDataArrayAction(item),
            ReadData = GetReadDataArrayAction(item)
        };
    }

    internal static Action<BinaryWriter> GetWriteDataArrayAction<T>(T dataArray)
    {
        return dataArray switch
        {
            double[] d1 => bw => WriteDataArray(bw, d1),
            double[,] d2 => bw => WriteDataArray(bw, d2),
            double[,,] d3 => bw => WriteDataArray(bw, d3),
            double[,,,] d4 => bw => WriteDataArray(bw, d4),
            double[,,,,] d5 => bw => WriteDataArray(bw, d5),
            double[,,,,,] d6 => bw => WriteDataArray(bw, d6),
            Complex[] c1 => bw => WriteDataArray(bw, c1),
            Complex[,] c2 => bw => WriteDataArray(bw, c2),
            Complex[,,] c3 => bw => WriteDataArray(bw, c3),
            Complex[,,,] c4 => bw => WriteDataArray(bw, c4),
            Complex[,,,,] c5 => bw => WriteDataArray(bw, c5),
            Complex[,,,,,] c6 => bw => WriteDataArray(bw, c6),
            _ => throw new NotImplementedException()
        };
    }

    internal static Action<BinaryReader> GetReadDataArrayAction<T>(T dataArray)
    {
        return dataArray switch
        {
            double[] d1 => br => ReadIntoDataArray(br, d1),
            double[,] d2 => br => ReadIntoDataArray(br, d2),
            double[,,] d3 => br => ReadIntoDataArray(br, d3),
            double[,,,] d4 => br => ReadIntoDataArray(br, d4),
            double[,,,,] d5 => br => ReadIntoDataArray(br, d5),
            double[,,,,,] d6 => br => ReadIntoDataArray(br, d6),
            Complex[] c1 => br => ReadIntoDataArray(br, c1),
            Complex[,] c2 => br => ReadIntoDataArray(br, c2),
            Complex[,,] c3 => br => ReadIntoDataArray(br, c3),
            Complex[,,,] c4 => br => ReadIntoDataArray(br, c4),
            Complex[,,,,] c5 => br => ReadIntoDataArray(br, c5),
            Complex[,,,,,] c6 => br => ReadIntoDataArray(br, c6),
            _ => throw new NotImplementedException()
        };
    }

    internal static int[] GetAllDimensions(Array array)
        => Enumerable.Range(0, array.Rank).Select((_, i) => array.GetLength(i)).ToArray();

    internal static void WriteDataArray(BinaryWriter binaryWriter, double[] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            binaryWriter.Write(dataArray[i]);
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, double[,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                binaryWriter.Write(dataArray[i, j]);
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, double[,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    binaryWriter.Write(dataArray[i, j, k]);
                }
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, double[,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        binaryWriter.Write(dataArray[i, j, k, el]);
                    }
                }
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, double[,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            binaryWriter.Write(dataArray[i, j, k, el, m]);
                        }
                    }
                }
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, double[,,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            for (var n = 0; n < axesDimensions[5]; n++)
                            {
                                binaryWriter.Write(dataArray[i, j, k, el, m, n]);
                            }
                        }
                    }
                }
            }
        }
    }

    // Complex versions
    internal static void WriteDataArray(BinaryWriter binaryWriter, Complex[] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            binaryWriter.Write(dataArray[i].Real);
            binaryWriter.Write(dataArray[i].Imaginary);
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, Complex[,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                binaryWriter.Write(dataArray[i, j].Real);
                binaryWriter.Write(dataArray[i, j].Imaginary);
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, Complex[,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    binaryWriter.Write(dataArray[i, j, k].Real);
                    binaryWriter.Write(dataArray[i, j, k].Imaginary);
                }
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, Complex[,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        binaryWriter.Write(dataArray[i, j, k, el].Real);
                        binaryWriter.Write(dataArray[i, j, k, el].Imaginary);
                    }
                }
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, Complex[,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            binaryWriter.Write(dataArray[i, j, k, el, m].Real);
                            binaryWriter.Write(dataArray[i, j, k, el, m].Imaginary);
                        }
                    }
                }
            }
        }
    }

    internal static void WriteDataArray(BinaryWriter binaryWriter, Complex[,,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            for (var n = 0; n < axesDimensions[5]; n++)
                            {
                                binaryWriter.Write(dataArray[i, j, k, el, m, n].Real);
                                binaryWriter.Write(dataArray[i, j, k, el, m, n].Imaginary);
                            }
                        }
                    }
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, double[] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            dataArray[i] = binaryReader.ReadDouble();
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, double[,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                dataArray[i, j] = binaryReader.ReadDouble();
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, double[,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    dataArray[i, j, k] = binaryReader.ReadDouble();
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, double[,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        dataArray[i, j, k, el] = binaryReader.ReadDouble();
                    }
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, double[,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            dataArray[i, j, k, el, m] = binaryReader.ReadDouble();
                        }
                    }
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, double[,,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            for (var n = 0; n < axesDimensions[5]; n++)
                            {
                                dataArray[i, j, k, el, m, n] = binaryReader.ReadDouble();
                            }
                        }
                    }
                }
            }
        }
    }

    // Complex versions
    internal static void ReadIntoDataArray(BinaryReader binaryReader, Complex[] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            var real = binaryReader.ReadDouble();
            var imaginary = binaryReader.ReadDouble();
            dataArray[i] = new Complex(real, imaginary);
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, Complex[,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                var real = binaryReader.ReadDouble();
                var imaginary = binaryReader.ReadDouble();
                dataArray[i, j] = new Complex(real, imaginary);
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, Complex[,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    var real = binaryReader.ReadDouble();
                    var imaginary = binaryReader.ReadDouble();
                    dataArray[i, j, k] = new Complex(real, imaginary);
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, Complex[,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        var real = binaryReader.ReadDouble();
                        var imaginary = binaryReader.ReadDouble();
                        dataArray[i, j, k, el] = new Complex(real, imaginary);
                    }
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, Complex[,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            var real = binaryReader.ReadDouble();
                            var imaginary = binaryReader.ReadDouble();
                            dataArray[i, j, k, el, m] = new Complex(real, imaginary);
                        }
                    }
                }
            }
        }
    }

    internal static void ReadIntoDataArray(BinaryReader binaryReader, Complex[,,,,,] dataArray)
    {
        var axesDimensions = GetAllDimensions(dataArray);
        for (var i = 0; i < axesDimensions[0]; i++)
        {
            for (var j = 0; j < axesDimensions[1]; j++)
            {
                for (var k = 0; k < axesDimensions[2]; k++)
                {
                    for (var el = 0; el < axesDimensions[3]; el++)
                    {
                        for (var m = 0; m < axesDimensions[4]; m++)
                        {
                            for (var n = 0; n < axesDimensions[5]; n++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imaginary = binaryReader.ReadDouble();
                                dataArray[i, j, k, el, m, n] = new Complex(real, imaginary);
                            }
                        }
                    }
                }
            }
        }
    }
}
