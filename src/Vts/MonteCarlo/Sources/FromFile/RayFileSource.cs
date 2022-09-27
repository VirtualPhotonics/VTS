using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for Ray FileSource implementation 
    /// including emitting position, direction, weight and initial tissue region index.
    /// </summary>
    public class RayFileSourceInput :ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of ZemaxFileSourceInput class
        /// </summary>
        /// <param name="sourceFileName">Source file name</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public RayFileSourceInput(
            string sourceFileName,
            int initialTissueRegionIndex)
        {
            SourceType = "RayFile";
            SourceFileName = sourceFileName;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of ZemaxFileSourceInput class
        /// </summary>
        public RayFileSourceInput()
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
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new RayFileSource(
                this.SourceFileName,
                this.InitialTissueRegionIndex)
            { Rng = rng };
        }
    }

    /// <summary>
    /// Implements FileSource with file name, initial 
    /// tissue region index.
    /// </summary>
    public class RayFileSource : ISource //: FromFileSourceBase
    {
        /// <summary>
        /// enumerator that iterates through database
        /// </summary>
        public IEnumerator<RayDataPoint> DatabaseEnumerator { get; set; }
        /// <summary>
        /// initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
        /// <summary>
        /// Returns an instance of Zemax File Source at a given location
        /// </summary>
        /// <param name="sourceFileName">filename of MCCL source DB file</param> 
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public RayFileSource(
            string sourceFileName,
            int initialTissueRegionIndex = 0)
        {
            var sourceDatabase = RayDatabase.FromFile(sourceFileName);
            DatabaseEnumerator = sourceDatabase.DataPoints.GetEnumerator();
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// method to iterate through database and get next photon
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <returns></returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            // read next source data point
            DatabaseEnumerator.MoveNext();
            var dp = DatabaseEnumerator.Current;

            var photon = new Photon(new Position(
                dp.Position.X, 
                dp.Position.Y, 
                dp.Position.Z),
                new Direction(
                    dp.Direction.Ux, 
                    dp.Direction.Uy, 
                    dp.Direction.Uz),
                dp.Weight, tissue, InitialTissueRegionIndex, Rng); 

            return photon;
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion
    }
}
