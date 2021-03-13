using System;
using System.IO;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for IGES FileSource implementation 
    /// including emitting position, direction, weight and initial tissue region index.
    /// </summary>
    public class IGESFileSourceInput  //:ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of ZemaxFileSourceInput class
        /// </summary>
        /// <param name="sourceFileName">Source file name</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IGESFileSourceInput(
            string sourceFileName,
            int initialTissueRegionIndex)
        {
            SourceType = "ZRDFileSource";
            SourceFileName = sourceFileName;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of ZemaxFileSourceInput class
        /// </summary>
        public IGESFileSourceInput()
            : this("", 0)
        {
        }

        /// <summary>
        /// Point source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// Source file name
        /// </summary>
        public string SourceFileName { get; set; }
        /// <summary>
        /// Database of SourceDataPoint
        /// </summary>
        public ZRDRayDatabase SourceDatabase { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        //public ISource CreateSource(Random rng = null)
        //{
        //    return new ZRDFileSource(
        //        this.SourceFileName,
        //        this.InitialTissueRegionIndex) { Rng = rng };
        //}
    }

    /// <summary>
    /// Implements ZemaxFileSource with file name, initial 
    /// tissue region index.
    /// </summary>
    //public class ZRDFileSource : FromFileSourceBase
    //{
    //    /// <summary>
    //    /// Returns an instance of Zemax File Source at a given location
    //    /// </summary>        
    //    /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
    //    public ZRDFileSource(
    //        string sourceFileName,
    //        int initialTissueRegionIndex = 0)
    //        : base(
    //            sourceFileName,
    //            initialTissueRegionIndex)
    //    {
    //    }

        //protected override List<ZRDRayDataInUFD.ZRDRayDataPoint> ReadFile(string fileName)
        //{
        //    //get the full path for the input file
        //    var fullFilePath = Path.GetFullPath(fileName);
        //    return ZRDRayDatabase.FromFile(fullFilePath);
        //}
    //}

}
