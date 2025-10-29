using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for Photon Database File Source implementation 
    /// including emitting position, direction, weight and initial tissue region index.
    /// </summary>
    public class PhotonDatabaseFileSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of class to read photons from database file 
        /// </summary>
        /// <param name="sourceFileName">Source file name</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public PhotonDatabaseFileSourceInput(
            string sourceFileName,
            int initialTissueRegionIndex)
        {
            SourceType = "PhotonDatabaseFile";
            SourceFileName = sourceFileName;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of class to read source photons from file
        /// </summary>
        public PhotonDatabaseFileSourceInput()
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
            rng ??= new Random();

            return new PhotonDatabaseFileSource(
                SourceFileName,
                InitialTissueRegionIndex)
            { Rng = rng };
        }
    }

    /// <summary>
    /// Implements PhotonDatabaseFileSource with file name, initial 
    /// tissue region index.
    /// </summary>
    public class PhotonDatabaseFileSource : ISource //: FromFileSourceBase
    {
        /// <summary>
        /// enumerator that iterates through database
        /// </summary>
        public IEnumerator<PhotonDataPoint> DatabaseEnumerator { get; set; }
        /// <summary>
        /// initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
        /// <summary>
        /// Returns an instance of photon from database
        /// </summary>
        /// <param name="sourceFileName">filename of MCCL source DB file</param> 
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public PhotonDatabaseFileSource(
            string sourceFileName,
            int initialTissueRegionIndex = 0)
        {
            var sourceDatabase = PhotonDatabase.FromFile(sourceFileName);
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

            if (dp == null) return null;
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