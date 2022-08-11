using System;
using System.IO;
using Vts.Common;
using Vts.Common.Logging;
using Vts.MonteCarlo;
using Vts.MonteCarlo.RayData;
using Vts.MonteCarlo.Zemax;

namespace Vts.ZemaxDatabaseConverter
{
    public class DatabaseConverter
    {
        private static ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(DatabaseConverter));

        /// <summary>
        /// method to convert zemax ray database to MCCL source database
        /// </summary>
        public static RayDatabase ConvertZemaxDatabaseToMCCLSourceDatabase(string inputFile, string outputFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inputFile))
                {
                    logger.Info(" *** No input file specified ***\n\nDefine an input file e.g. database.zrd");
                    return null;
                }
                if (string.IsNullOrEmpty(outputFile))
                {
                    logger.Info(" *** No output file specified ***\n\nDefine an output file e.g. database.mcs");
                    return null;
                }

                //get the full path for the input file
                var fullFilePath = Path.GetFullPath(inputFile);

                if (File.Exists(fullFilePath + ".zrd")) // binary ZRD source file
                {
                    var fileToConvert =  ZRDRayDatabase.FromFile(fullFilePath);

                    using (var dbWriter = new RayDatabaseWriter(
                        VirtualBoundaryType.DiffuseReflectance, outputFile))
                    {
                        for (int i = 0; i < fileToConvert.NumberOfElements; i++)
                        {
                            // enumerate through the elements 
                            var enumerator = fileToConvert.DataPoints.GetEnumerator();
                            // advance to the next ray data
                            enumerator.MoveNext();
                            var dp = enumerator.Current;
                            // excise Position,Direction,Weight from Zemax struct
                            dbWriter.Write(new RayDataPoint(
                                new Position(dp.X, dp.Y, dp.Z),
                                new Direction(dp.Ux, dp.Uy, dp.Uz),
                                dp.Weight));
                        }
                    }
                }

                //throw a file not found exception
                throw new FileNotFoundException("\nThe following input file could not be found: " + fullFilePath + " - type mc help=infile for correct syntax");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

    }
}
