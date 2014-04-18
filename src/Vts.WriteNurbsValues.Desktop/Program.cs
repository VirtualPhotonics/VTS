using Vts.IO;
using Vts.Modeling.ForwardSolvers;

namespace Vts.WriteNurbsValues.Desktop
{
    /// <summary>
    /// This class menages the reading of binary files and the writing to XML of a NurbsValues class.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string readPath = @"Modeling\Resources\ReferenceNurbs\";
            string writePath = @"..\..\..\Vts\";
            string[] domain = { @"SpatialFrequencyDomain\", @"RealDomain\" };
            string folder = @"v0p1\";

            for (int dInd = 0; dInd < domain.Length; dInd++)
            {
                ReadBinaryAndWriteXML(readPath, writePath, domain[dInd], folder);
            }
        }
        /// <summary>
        /// Reads binary files generated in Matlab from the respective folder and writes XML. 
        /// </summary>
        /// <param name="readPath"></param>
        /// <param name="writePath"></param>
        /// <param name="domain"></param>
        /// <param name="folder"></param>
        private static void ReadBinaryAndWriteXML(string readPath, string writePath, string domain, string folder)
        {
            ushort[] dims = (ushort[])FileIO.ReadArrayFromBinaryInResources<ushort>(readPath + domain + folder + "dims", "Vts", 2);
            ushort[] degrees = (ushort[])FileIO.ReadArrayFromBinaryInResources<ushort>(readPath + domain + folder + "degrees", "Vts", 2);
            double[] maxValues = (double[])FileIO.ReadArrayFromBinaryInResources<double>(readPath + domain + folder + "maxValues", "Vts", 2);
            var timeKnots = (double[])FileIO.ReadArrayFromBinaryInResources<double>(readPath + domain + folder + "timeKnots", "Vts", dims[0]);
            var spaceKnots = (double[])FileIO.ReadArrayFromBinaryInResources<double>(readPath + domain + folder + "spaceKnots", "Vts", dims[1]);
            NurbsValues timeValues = new NurbsValues(NurbsValuesDimensions.time, timeKnots, maxValues[0], degrees[0]);
            NurbsValues spaceValues = new NurbsValues(NurbsValuesDimensions.space, spaceKnots, maxValues[1], degrees[1]);
            timeValues.WriteToXML<NurbsValues>(writePath + readPath + domain + folder + "timeNurbsValues.xml");
            spaceValues.WriteToXML<NurbsValues>(writePath + readPath + domain + folder + "spaceNurbsValues.xml");  
        }
    }
}
