using System;
using System.IO;
using Vts.IO;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// Class that handles IO for IDetectors.
    /// </summary>
    public static class DetectorIO
    {
        /// <summary>
        /// Writes Detector xml for scalar detectors, writes Detector xml and 
        /// binary for 1D and larger detectors.  Detector.Name is used for filename.
        /// </summary>
        /// <param name="detector">IDetector being written.</param>
        /// <param name="folderPath">location of written file.</param>
        public static void WriteDetectorToFile(IDetector detector, string folderPath)
        {
            try
            {
                // allow null folderPath in case writing to isolated storage
                string filePath = folderPath;
                if (folderPath == "")
                {
                    filePath = detector.Name;
                }
                else
                {
                    filePath = folderPath + @"/" + detector.Name;

                    // desktop folder
                    FileIO.CreateDirectory(folderPath);
                }

                FileIO.WriteToJson(detector, filePath + ".txt");
                var binaryArraySerializers = detector.GetBinarySerializers();
                if (binaryArraySerializers == null)
                {
                    return;
                }

                foreach (var binaryArraySerializer in binaryArraySerializers)
                {
                    if (binaryArraySerializer == null)
                        continue;

                    // Create a file to write binary data 
                    using (Stream s = StreamFinder.GetFileStream(filePath + binaryArraySerializer.FileTag, FileMode.OpenOrCreate))
                    {
                        if (s == null)
                            continue;

                        using (BinaryWriter bw = new BinaryWriter(s))
                        {
                            binaryArraySerializer.WriteData(bw);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem writing detector information to file.\n\nDetails:\n\n" + e + "\n");
            }
        }

        /// <summary>
        /// Reads Detector from File with given fileName.
        /// </summary>
        /// <param name="fileName">filename string of file to be read</param>
        /// <param name="folderPath">path string where file resides</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFile(string fileName, string folderPath)
        {
            try
            {
                // allow null filePaths in case writing to isolated storage
                string filePath;
                if (folderPath == "")
                {
                    filePath = fileName;
                }
                else
                {
                    filePath = folderPath + @"/" + fileName;
                }
                var detector = FileIO.ReadFromJson<IDetector>(filePath + ".txt");

                var binaryArraySerializers = detector.GetBinarySerializers();

                if (binaryArraySerializers == null)
                {
                    return detector;
                }

                foreach (var binaryArraySerializer in binaryArraySerializers)
                {
                    if (binaryArraySerializer == null)
                        continue;

                    using ( Stream s = StreamFinder.GetFileStream(filePath + binaryArraySerializer.FileTag, FileMode.Open))
                    {
                        if (s == null)
                            continue;

                        using (BinaryReader br = new BinaryReader(s))
                        {
                            binaryArraySerializer.ReadData(br);
                        }
                    }
                }

                return detector;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }

        /// <summary>
        /// Reads Detector from a file in resources using given fileName.
        /// </summary>
        /// <param name="fileName">filename string of file to be read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <param name="projectName">project name string where file resides in resources</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFileInResources(string fileName, string folderPath, string projectName)
        {
            try
            {
                string filePath = folderPath + fileName;
                // allow null filePaths in case writing to isolated storage
                //string filePath;
                //if (folderPath == "")
                //{
                //    filePath = fileName;
                //}
                //else
                //{
                //    filePath = folderPath + @"/" + fileName;
                //}
                var detector = FileIO.ReadFromJsonInResources<IDetector>(filePath + ".txt", projectName);

                var binaryArraySerializers = detector.GetBinarySerializers();

                if (binaryArraySerializers == null)
                {
                    return detector;
                }

                foreach (var binaryArraySerializer in binaryArraySerializers)
                {
                    if (binaryArraySerializer == null)
                        continue;

                    using (Stream s = StreamFinder.GetFileStreamFromResources(filePath + binaryArraySerializer.FileTag, projectName))
                    {
                        if (s == null)
                            continue;
                        
                        using (BinaryReader br = new BinaryReader(s))
                        {
                            binaryArraySerializer.ReadData(br);
                        }
                    }
                }

                return detector;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from resource file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
    }
}
