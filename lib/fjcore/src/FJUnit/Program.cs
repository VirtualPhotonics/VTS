using System;
using System.IO;
using FluxJpeg.Core.Decoder;
using FluxJpeg.Core;
using FluxJpeg.Core.Filtering;
using System.Diagnostics;
using FluxJpeg.Core.Encoder;

namespace FJUnit
{
    class Program
    {
        static string output = "output.jpg", input = "../../geneserath.jpg";
        static int resizeTo = 500;

        static void Main(string[] args)
        {
            Console.WriteLine("FJCore Unit Test");

            PrintHeader();

            BasicResize();
        }

        private static void BasicResize()
        {
            Image resized = Resize(input, resizeTo);

            System.Drawing.Bitmap bmp = resized.ToBitmap();
            bmp.Save("out.jpg");

            // Save to disk.
            File.WriteAllBytes(output, Encode(resized).ToArray());

            // Show results
            Process.Start(new ProcessStartInfo(output));
        }

        static Image Resize(string pathIn, int edge)
        {
            JpegDecoder decoder = new JpegDecoder(File.Open(pathIn, FileMode.Open));
            DecodedJpeg jpeg = decoder.Decode();
            ImageResizer resizer = new ImageResizer(jpeg.Image);
            return resizer.Resize(edge, ResamplingFilters.LowpassAntiAlias);
        }

        static MemoryStream Encode(Image image)
        {
            MemoryStream outStream = new MemoryStream();
            JpegEncoder encoder = new JpegEncoder(image, 85, outStream);
            encoder.Encode();
            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }

        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            #if DYNAMIC_IDCT
            Console.WriteLine("IDCT: Dynamic CIL");
            #else
            Console.WriteLine("IDCT: Pure C#");
            #endif
            Console.ForegroundColor = ConsoleColor.Gray;
        }

    }
}
