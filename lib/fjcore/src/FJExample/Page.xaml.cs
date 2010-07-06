using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using FluxJpeg.Core.Decoder;
using FluxJpeg.Core;
using System.IO;
using FluxJpeg.Core.Filtering;
using FluxJpeg.Core.Encoder;
using System.Windows.Media.Imaging;

namespace FJExample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Image files (*.jpg)|*.jpg" };

            if (ofd.ShowDialog() != true) return;

            Stream fileStream = ofd.File.OpenRead();

            // Display input image
            Stream inStream = new MemoryStream(new BinaryReader(fileStream).ReadBytes((int)fileStream.Length));
            BitmapImage imageIn = new BitmapImage();
            imageIn.SetSource(inStream);
            InputImage.Source = imageIn;

            // Rewind
            fileStream.Seek(0, SeekOrigin.Begin);

            using (fileStream)
            {
                // Decode
                DecodedJpeg jpegIn = new JpegDecoder(fileStream).Decode();

                if (!ImageResizer.ResizeNeeded(jpegIn.Image, 320))
                {
                    OutputImage.Source = null;
                    OutputText.Text = "No resize necessary.";
                    return;
                }

                // Resize
                DecodedJpeg jpegOut = new DecodedJpeg(
                    new ImageResizer(jpegIn.Image)
                        .Resize(320, ResamplingFilters.NearestNeighbor),
                    jpegIn.MetaHeaders); // Retain EXIF details

                // Encode
                MemoryStream outStream = new MemoryStream();
                new JpegEncoder(jpegOut, 90, outStream).Encode();

                // Display 
                outStream.Seek(0, SeekOrigin.Begin);
                BitmapImage image = new BitmapImage();
                image.SetSource(outStream);
                OutputImage.Source = image;
            }

        }
    }
}
