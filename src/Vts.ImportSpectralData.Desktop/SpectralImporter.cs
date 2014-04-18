using System;
using System.IO;
using System.Linq;
using Vts.Common.Logging;
using Vts.IO;
using Vts.SpectralMapping;

namespace Vts.ImportSpectralData.Desktop
{
    public static class SpectralImporter
    {
        private static ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(SpectralImporter));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importFiles">the name(s) of the file(s) to import</param>
        /// <param name="importPath">the path of the files to import (relative or absolute)</param>
        /// <param name="outname">the name of the resulting output xml spectral dictionary</param>
        /// <param name="outpath">the output directory of the generated xml dictionary (relative or absolute)</param>
        public static ChromophoreSpectrumDictionary ImportSpectraFromFile(
            string[] importFiles = null,
            string importPath = "",
            string outname = "SpectralDictionary",
            string outpath = "")
        {
            if (importFiles == null || importFiles.Length == 0 || string.IsNullOrEmpty(importFiles[0]))
            {
                importFiles = new string[] { "absorber-*.txt" };
            }

            var allFiles = importFiles.SelectMany(file => Directory.GetFiles(
                importPath ?? Directory.GetCurrentDirectory(), file));

            var chromophoreDictionary = new ChromophoreSpectrumDictionary();

            logger.Info(() => "Importing spectral data files");
            foreach (var file in allFiles)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        logger.Info("Importing file: " + file);
                        var stream = StreamFinder.GetFileStream(file, FileMode.Open);

                        SpectralDatabase.AppendDatabaseFromFile(chromophoreDictionary, stream);
                    }
                    catch (Exception e)
                    {
                        logger.Info("****  An error occurred while importing file: " + file);
                        logger.Info("Detailed error: " + e.Message);
                    }
                }
            }

            return chromophoreDictionary;
        }
    }
}
