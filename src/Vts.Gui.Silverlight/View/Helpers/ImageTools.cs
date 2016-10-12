using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using Vts.IO;

namespace Vts.Gui.Silverlight.View.Helpers
{
    /// <summary>
    /// Contains static helper methods to generate images of UIElements
    /// </summary>
    public static class ImageTools
    {
        public static void SaveUIElementToJpegImage(UIElement element)
        {
            // todo: pass myChart as a command parameter declaratively to be handled in PlotViewModel
            var b = new WriteableBitmap(element, null);
            using (var s = StreamFinder.GetLocalFilestreamFromSaveFileDialog("jpg"))
            {
                if (s != null)
                {
                    SaveBitmapToJpeg(b, s);
                }
            }
        }

        // as found here: http://blog.blueboxes.co.uk/2009/07/21/rendering-xaml-to-a-jpeg-using-silverlight-3/
        // also see attribution to SO post here: http://stackoverflow.com/questions/1139200/using-fjcore-to-encode-silverlight-writeablebitmap 
        private static void SaveBitmapToJpeg(WriteableBitmap bitmap, Stream fs)
        {
            //Convert the Image to pass into FJCore
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;

            byte[][,] raster = new byte[bands][,];

            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };

            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);

            //Encode the Image as a JPEG
            MemoryStream stream = new MemoryStream();
            FluxJpeg.Core.Encoder.JpegEncoder encoder = new FluxJpeg.Core.Encoder.JpegEncoder(img, 100, stream);

            encoder.Encode();

            //Move back to the start of the stream
            stream.Seek(0, SeekOrigin.Begin);

            //Get the Bytes and write them to the stream
            byte[] binaryData = new Byte[stream.Length];
            long bytesRead = stream.Read(binaryData, 0, (int)stream.Length);

            fs.Write(binaryData, 0, binaryData.Length);
        }


    }
}
