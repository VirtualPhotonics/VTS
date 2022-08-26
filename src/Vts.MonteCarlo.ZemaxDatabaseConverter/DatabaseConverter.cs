using System;
using System.IO;
using Vts.Common;
using Vts.MonteCarlo.RayData;
using Vts.Zemax;

namespace Vts.MonteCarlo.ZemaxDatabaseConverter
{
    public static class DatabaseConverter
    {
        /// <summary>
        /// method to verify input arguments
        /// </summary>
        /// <param name="inputFile">Database to be converted</param>
        /// <param name="outputFile">Converted database</param>
        /// <returns>Boolean indicating inputs are valid</returns>
        public static bool VerifyInputs(string inputFile, string outputFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inputFile))
                {
                    Console.WriteLine("\nNo input file specified");
                    return false;
                }
                if (string.IsNullOrEmpty(outputFile))
                {
                    Console.WriteLine("\nNo output file specified");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// method to convert zemax ray database to MCCL source database
        /// </summary>
        /// <param name="inputFile">Database to be converted</param>
        /// <param name="outputFile">Converted database</param>
        public static void ConvertZemaxDatabaseToMCCLSourceDatabase(string inputFile, string outputFile)
        {
            try
            {
                //get the full path for the input file
                var fullFilePath = Path.GetFullPath(inputFile);

                var fileToConvert = ZrdRayDatabase.FromFile(fullFilePath);

                using (var dbWriter = new RayDatabaseWriter(
                    VirtualBoundaryType.DiffuseReflectance, outputFile))
                {
                    // enumerate through the elements 
                    var enumerator = fileToConvert.DataPoints.GetEnumerator();
                    for (int i = 0; i < fileToConvert.NumberOfElements; i++)
                    {
                        // advance to the next ray data
                        enumerator.MoveNext();
                        var dp = enumerator.Current;
                        // excise Position,Direction,Weight from Zemax struct
                        dbWriter.Write(new RayDataPoint(
                            new Position(dp.X, dp.Y, dp.Z),
                            new Direction(dp.Ux, dp.Uy, dp.Uz),
                            dp.Weight));
                    }
                    dbWriter.Close();
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// method to convert zemax ray database to MCCL source database
        /// </summary>
        /// <param name="inputFile">Database to be converted</param>
        /// <param name="outputFile">Converted database</param>
        public static void ConvertMCCLDatabaseToZemaxSourceDatabase(string inputFile, string outputFile)
        {
            try
            {
                //get the full path for the input file
                var fullFilePath = Path.GetFullPath(inputFile);

                var fileToConvert = RayDatabase.FromFile(fullFilePath);

                using (var dbWriter = new ZrdRayDatabaseWriter(
                    VirtualBoundaryType.DiffuseReflectance, outputFile))
                {
                    // enumerate through the elements 
                    var enumerator = fileToConvert.DataPoints.GetEnumerator();
                    for (int i = 0; i < fileToConvert.NumberOfElements; i++)
                    {
                        // advance to the next ray data
                        enumerator.MoveNext();
                        var dp = enumerator.Current;
                        var zrdRayDataPoint = new ZrdRayDataPoint();
                        // set Position,Direction,Weight in Zemax struct
                        zrdRayDataPoint.X = dp.Position.X;
                        zrdRayDataPoint.Y = dp.Position.Y;
                        zrdRayDataPoint.Z = dp.Position.Z;
                        zrdRayDataPoint.Ux = dp.Direction.Ux;
                        zrdRayDataPoint.Uy = dp.Direction.Uy;
                        zrdRayDataPoint.Uz = dp.Direction.Uz;
                        zrdRayDataPoint.Weight = dp.Weight;
                        dbWriter.Write(zrdRayDataPoint);
                    }
                    dbWriter.Close();
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
