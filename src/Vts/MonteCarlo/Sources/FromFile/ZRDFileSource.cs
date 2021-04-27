using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for ZRD FileSource implementation 
    /// including emitting position, direction, weight and initial tissue region index.
    /// </summary>
    public class ZRDFileSourceInput :ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of ZemaxFileSourceInput class
        /// </summary>
        /// <param name="sourceFileName">Source file name</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public ZRDFileSourceInput(
            string sourceFileName,
            int initialTissueRegionIndex)
        {
            SourceType = "ZRDFile";
            SourceFileName = sourceFileName;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of ZemaxFileSourceInput class
        /// </summary>
        public ZRDFileSourceInput()
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
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new ZRDFileSource(
                this.SourceFileName,
                this.InitialTissueRegionIndex)
            { Rng = rng };
        }
    }

    /// <summary>
    /// Implements ZemaxFileSource with file name, initial 
    /// tissue region index.
    /// </summary>
    public class ZRDFileSource : ISource //: FromFileSourceBase
    {
        public IEnumerator<ZRDRayDataPoint> _databaseEnumerator;
        public int _initialTissueRegionIndex;
        /// <summary>
        /// Returns an instance of Zemax File Source at a given location
        /// </summary>        
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public ZRDFileSource(
            string sourceFileName,
            int initialTissueRegionIndex = 0)
            //: base(
            //      ZRDRayDatabase.FromFile(sourceFileName).DataPoints.GetEnumerator(),
            //      initialTissueRegionIndex)
        {
            var sourceDatabase = ZRDRayDatabase.FromFile(sourceFileName);
            _databaseEnumerator = sourceDatabase.DataPoints.GetEnumerator();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // read next source data point
            _databaseEnumerator.MoveNext();
            var dp = _databaseEnumerator.Current;

            var photon = new Photon(new Position(dp.X, dp.Y, dp.Z),
                new Direction(dp.Ux, dp.Uy, dp.Uz),
                dp.Weight, tissue, _initialTissueRegionIndex, Rng); 

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
